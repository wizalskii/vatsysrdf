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
    /// Shows TX indicators with proper map layer registration
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

        // Map layer reference
        private DisplayMaps.Map rdfMap;

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

                // Register map layer
                RegisterMapLayer();

                // Set up update timer for cleanup
                updateTimer = new Timer
                {
                    Interval = 1000 // 1 second
                };
                updateTimer.Tick += UpdateTimer_Tick;
                updateTimer.Start();

                System.Diagnostics.Debug.WriteLine("RDF: Plugin initialized successfully");
                System.Diagnostics.Debug.WriteLine("RDF: Enable 'RDF - Transmission Indicators' in Maps menu to show TX symbols");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error during initialization: {ex}");
                MessageBox.Show($"RDF Plugin initialization error: {ex.Message}",
                    "RDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterMapLayer()
        {
            try
            {
                // Create map layer for RDF
                rdfMap = new DisplayMaps.Map
                {
                    Name = "RDF - Transmission Indicators"
                };

                // Add to vatSys maps list
                DisplayMaps.Maps.Add(rdfMap);

                System.Diagnostics.Debug.WriteLine($"RDF: Map layer registered - '{rdfMap.Name}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error registering map layer: {ex}");
                System.Diagnostics.Debug.WriteLine("RDF: TX symbols may not be visible without map layer");
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
                    return new CustomLabelItem
                    {
                        Text = "TX"
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
