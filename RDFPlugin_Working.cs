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
    /// Radio Direction Finder Plugin for VATSYS
    /// Shows visual indicators for aircraft based on VATSIM data correlation
    /// </summary>
    [Export(typeof(IPlugin))]
    public class RDFPlugin : IPlugin
    {
        public string Name => "Radio Direction Finder (RDF)";

        private RDFSettings settings;
        private VatsimDataFeed vatsimData;
        private Timer updateTimer;

        // Track currently transmitting callsigns with timestamp
        private readonly ConcurrentDictionary<string, DateTime> highlightedAircraft =
            new ConcurrentDictionary<string, DateTime>();

        public RDFPlugin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RDF: Plugin initializing...");

                // Load settings
                settings = RDFSettings.Load();

                // Initialize VATSIM data feed
                vatsimData = new VatsimDataFeed();
                vatsimData.Start();

                // Set up update timer for cleanup
                updateTimer = new Timer
                {
                    Interval = 1000 // 1 second
                };
                updateTimer.Tick += UpdateTimer_Tick;
                updateTimer.Start();

                System.Diagnostics.Debug.WriteLine("RDF: Plugin initialized successfully");
                System.Diagnostics.Debug.WriteLine("RDF: Monitoring VATSIM network for frequency data");
                System.Diagnostics.Debug.WriteLine("RDF: Use Ctrl+T hotkey to highlight aircraft on your frequency");
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
                if (Network.IsConnected && settings.Enabled)
                {
                    AutoHighlightAircraftOnFrequencies();
                }

                // Clean up old highlights (after 5 seconds)
                var cutoff = DateTime.UtcNow.AddSeconds(-5);
                var stale = highlightedAircraft.Where(kvp => kvp.Value < cutoff)
                    .Select(kvp => kvp.Key).ToList();

                foreach (var callsign in stale)
                {
                    highlightedAircraft.TryRemove(callsign, out _);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error in update timer: {ex}");
            }
        }

        private void AutoHighlightAircraftOnFrequencies()
        {
            try
            {
                if (Network.Me == null || Network.Me.Frequencies == null)
                    return;

                // Get all aircraft on our frequencies from VATSIM data
                foreach (var freqInt in Network.Me.Frequencies)
                {
                    // Convert frequency integer to Hz (VATSIM uses different format)
                    uint freqHz = (uint)(freqInt * 1000); // Convert to Hz

                    var callsignsOnFreq = vatsimData.GetCallsignsOnFrequency(freqHz);

                    if (callsignsOnFreq.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"RDF: Found {callsignsOnFreq.Count} aircraft on frequency {freqInt / 1000.0:F3} MHz");

                        foreach (var callsign in callsignsOnFreq)
                        {
                            // Highlight this aircraft
                            highlightedAircraft[callsign] = DateTime.UtcNow;
                        }
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
            // Track FDR updates if needed
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
                    string indicator = count > 1 ? "●" : "○";  // Filled for multiple, empty for single

                    return new CustomLabelItem
                    {
                        Text = indicator
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
                    // TODO: CustomColour constructor needs investigation
                    // For now, return null - label indicators will still work
                    // int count = highlightedAircraft.Count;
                    // Color highlightColor = count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;
                    // return new CustomColour with highlight color
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
            // Use same logic as ASD tracks
            return SelectASDTrackColour(track);
        }
    }
}
