using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using vatsys;
using vatsys.Plugin;

namespace VatsysRDF
{
    /// <summary>
    /// Radio Direction Finder Plugin for VATSYS with Zoom-Responsive Rings
    /// Shows visual indicators and rings that scale with zoom level
    /// </summary>
    [Export(typeof(IPlugin))]
    public class RDFPlugin : IPlugin
    {
        public string Name => "Radio Direction Finder (RDF) - Zoom Responsive";

        private RDFSettings settings;
        private VatsimDataFeed vatsimData;
        private Timer updateTimer;
        private RDFOverlay overlay;

        // Track currently transmitting callsigns with timestamp
        private readonly ConcurrentDictionary<string, DateTime> highlightedAircraft =
            new ConcurrentDictionary<string, DateTime>();

        // Track aircraft positions for overlay drawing
        private readonly ConcurrentDictionary<string, Coordinate> aircraftPositions =
            new ConcurrentDictionary<string, Coordinate>();

        public RDFPlugin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RDF: Plugin initializing (Zoom Responsive)...");

                // Load settings
                settings = RDFSettings.Load();

                // Initialize VATSIM data feed
                vatsimData = new VatsimDataFeed();
                vatsimData.Start();

                // Create overlay window for drawing rings
                overlay = new RDFOverlay(settings, highlightedAircraft, aircraftPositions);
                overlay.Show();

                // Set up update timer
                updateTimer = new Timer
                {
                    Interval = 100 // 100ms for smooth updates
                };
                updateTimer.Tick += UpdateTimer_Tick;
                updateTimer.Start();

                System.Diagnostics.Debug.WriteLine("RDF: Plugin initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error during initialization: {ex}");
                MessageBox.Show($"RDF Plugin initialization error: {ex.Message}",
                    "RDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Auto-highlight aircraft on controller's frequencies
                if (settings.Enabled && Network.Me != null)
                {
                    AutoHighlightAircraftOnFrequencies();
                }

                // Update aircraft positions for overlay
                UpdateAircraftPositions();

                // Clean up old highlights (after 5 seconds)
                var cutoff = DateTime.UtcNow.AddSeconds(-5);
                var stale = highlightedAircraft.Where(kvp => kvp.Value < cutoff)
                    .Select(kvp => kvp.Key).ToList();

                foreach (var callsign in stale)
                {
                    highlightedAircraft.TryRemove(callsign, out _);
                    aircraftPositions.TryRemove(callsign, out _);
                }

                // Update overlay window position and trigger redraw
                if (MMI.FPASD != null)
                {
                    overlay.UpdatePosition(MMI.FPASD.Bounds);
                }

                overlay.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error in update timer: {ex}");
            }
        }

        private void UpdateAircraftPositions()
        {
            try
            {
                // Get all FDRs and update positions for highlighted aircraft
                var fdrs = FDP2.GetFDRs;
                if (fdrs == null) return;

                foreach (var fdr in fdrs)
                {
                    if (highlightedAircraft.ContainsKey(fdr.Callsign))
                    {
                        var location = fdr.GetLocation();
                        if (location != null)
                        {
                            aircraftPositions[fdr.Callsign] = location;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error updating aircraft positions: {ex}");
            }
        }

        private void AutoHighlightAircraftOnFrequencies()
        {
            try
            {
                if (Network.Me == null)
                    return;

                // Observer mode: show ALL aircraft when no frequencies assigned
                if (settings.ObserverMode &&
                    (Network.Me.Frequencies == null || Network.Me.Frequencies.Length == 0))
                {
                    var allCallsigns = vatsimData.GetAllCallsigns();
                    foreach (var callsign in allCallsigns)
                    {
                        highlightedAircraft[callsign] = DateTime.UtcNow;
                    }
                    return;
                }

                if (Network.Me.Frequencies == null)
                    return;

                // Normal mode: Get aircraft on our frequencies
                foreach (var freqInt in Network.Me.Frequencies)
                {
                    uint freqHz = (uint)(freqInt * 1000);
                    var callsignsOnFreq = vatsimData.GetCallsignsOnFrequency(freqHz);

                    foreach (var callsign in callsignsOnFreq)
                    {
                        highlightedAircraft[callsign] = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error auto-highlighting aircraft: {ex}");
            }
        }

        // IPlugin interface methods

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            // Update position when FDR updates
            if (updated != null && highlightedAircraft.ContainsKey(updated.Callsign))
            {
                var location = updated.GetLocation();
                if (location != null)
                {
                    aircraftPositions[updated.Callsign] = location;
                }
            }
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            // Track radar updates if needed
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            if (!settings.Enabled || itemType != "RDF_TX")
                return null;

            if (track == null)
                return null;

            try
            {
                var fdr = track.GetFDR();
                if (fdr == null)
                    return null;

                var callsign = fdr.Callsign;
                if (string.IsNullOrEmpty(callsign))
                    return null;

                // Check if this aircraft is highlighted
                if (highlightedAircraft.ContainsKey(callsign))
                {
                    int count = highlightedAircraft.Count;
                    string indicator = count > 1 ? "●" : "○";
                    Color highlightColor = count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;

                    return new CustomLabelItem
                    {
                        Text = indicator,
                        ForeColourIdentity = Colours.Identities.Custom,
                        ForeColour = new CustomColour(highlightColor.R, highlightColor.G, highlightColor.B)
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error in GetCustomLabelItem: {ex.Message}");
            }

            return null;
        }

        public CustomColour SelectASDTrackColour(Track track)
        {
            if (!settings.Enabled || track == null)
                return null;

            try
            {
                var fdr = track.GetFDR();
                if (fdr == null)
                    return null;

                var callsign = fdr.Callsign;
                if (string.IsNullOrEmpty(callsign))
                    return null;

                // Check if this aircraft is highlighted
                if (highlightedAircraft.ContainsKey(callsign))
                {
                    int count = highlightedAircraft.Count;
                    Color highlightColor = count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;

                    return new CustomColour(highlightColor.R, highlightColor.G, highlightColor.B);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error in SelectASDTrackColour: {ex.Message}");
            }

            return null;
        }

        public CustomColour SelectGroundTrackColour(Track track)
        {
            return SelectASDTrackColour(track);
        }
    }
}
