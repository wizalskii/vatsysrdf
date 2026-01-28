using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using vatsys;
using vatsys.Plugin;
using Audio = vatsys.Audio;

namespace VatsysRDF
{
    public class RDFPlugin : IPlugin
    {
        public string Name => "Radio Direction Finder (RDF)";

        private RDFSettings settings;
        private Timer cleanupTimer;
        private VatsimDataFeed vatsimData;

        // Track which frequencies we've subscribed to (using object since VSCSFrequency type varies by VATSYS version)
        private readonly HashSet<object> subscribedFrequencies = new HashSet<object>();

        // Track currently transmitting callsigns with timestamp
        private readonly ConcurrentDictionary<string, DateTime> currentlyTransmitting = new ConcurrentDictionary<string, DateTime>();

        // Track receiving frequencies
        private readonly ConcurrentDictionary<uint, DateTime> receivingFrequencies = new ConcurrentDictionary<uint, DateTime>();

        public RDFPlugin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RDF: Plugin initializing...");

                // Load settings
                settings = RDFSettings.Load();

                // Initialize VATSIM data feed for frequency correlation
                vatsimData = new VatsimDataFeed();
                vatsimData.Start();

                // Subscribe to audio events
                SubscribeToAudioEvents();

                // Set up cleanup timer to remove old transmissions
                cleanupTimer = new Timer
                {
                    Interval = 500 // Check twice per second for responsive updates
                };
                cleanupTimer.Tick += CleanupTimer_Tick;
                cleanupTimer.Start();

                System.Diagnostics.Debug.WriteLine("RDF: Plugin initialized successfully");
                System.Diagnostics.Debug.WriteLine("RDF: Using VATSIM data feed to identify specific transmitting aircraft");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error during initialization: {ex}");
                MessageBox.Show($"RDF Plugin initialization error: {ex.Message}", "RDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubscribeToAudioEvents()
        {
            try
            {
                // Subscribe to frequency list changes
                Audio.VSCSFrequenciesChanged += Audio_VSCSFrequenciesChanged;

                // Subscribe to existing frequencies
                UpdateFrequencySubscriptions();

                System.Diagnostics.Debug.WriteLine("RDF: Audio events subscribed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error subscribing to audio events: {ex}");
            }
        }

        private void Audio_VSCSFrequenciesChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RDF: VSCS Frequencies changed");
            UpdateFrequencySubscriptions();
        }

        private void UpdateFrequencySubscriptions()
        {
            try
            {
                var currentFreqs = Audio.VSCSFrequencies?.Cast<object>().ToList() ?? new List<object>();

                // Subscribe to new frequencies
                foreach (dynamic freq in currentFreqs)
                {
                    if (!subscribedFrequencies.Contains(freq))
                    {
                        freq.ReceivingChanged += Frequency_ReceivingChanged;
                        subscribedFrequencies.Add(freq);

                        System.Diagnostics.Debug.WriteLine($"RDF: Subscribed to frequency {freq.Frequency / 1000000.0:F3} MHz");
                    }
                }

                // Unsubscribe from removed frequencies
                var toRemove = subscribedFrequencies.Where(f => !currentFreqs.Contains(f)).ToList();
                foreach (dynamic freq in toRemove)
                {
                    freq.ReceivingChanged -= Frequency_ReceivingChanged;
                    subscribedFrequencies.Remove(freq);

                    System.Diagnostics.Debug.WriteLine($"RDF: Unsubscribed from frequency {freq.Frequency / 1000000.0:F3} MHz");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error updating frequency subscriptions: {ex}");
            }
        }

        private void Frequency_ReceivingChanged(object sender, EventArgs e)
        {
            try
            {
                dynamic freq = sender;  // Use dynamic to avoid type resolution issues
                if (freq != null)
                {
                    System.Diagnostics.Debug.WriteLine($"RDF: Receiving changed on {freq.Frequency / 1000000.0:F3} MHz - Receiving: {freq.Receiving}");

                    if (freq.Receiving)
                    {
                        // Mark this frequency as receiving
                        receivingFrequencies[freq.Frequency] = DateTime.UtcNow;

                        // Try to detect which aircraft is transmitting
                        DetectTransmittingAircraft(freq);
                    }
                    else
                    {
                        // Remove frequency from receiving list
                        receivingFrequencies.TryRemove(freq.Frequency, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error handling receiving changed: {ex}");
            }
        }

        private void DetectTransmittingAircraft(dynamic freq)
        {
            try
            {
                // Get all callsigns currently tuned to this frequency from VATSIM data
                var callsignsOnFreq = vatsimData.GetCallsignsOnFrequency(freq.Frequency);

                if (callsignsOnFreq.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"RDF: Transmission on {freq.Frequency / 1000000.0:F3} MHz but no aircraft found on frequency in VATSIM data");
                    return;
                }

                // Get all visible tracks
                var tracks = MMI.InhibitedAirportMovements.Concat(MMI.Tracks).ToList();

                // Match VATSIM callsigns to our tracks
                int matchedCount = 0;
                foreach (var track in tracks)
                {
                    if (track == null) continue;

                    var callsign = track.GetCallsign();
                    if (string.IsNullOrEmpty(callsign)) continue;

                    // Only mark aircraft that are on this specific frequency
                    if (callsignsOnFreq.Contains(callsign))
                    {
                        currentlyTransmitting[callsign] = DateTime.UtcNow;
                        matchedCount++;
                        System.Diagnostics.Debug.WriteLine($"RDF: {callsign} transmitting on {freq.Frequency / 1000000.0:F3} MHz");
                    }
                }

                if (matchedCount == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"RDF: Transmission on {freq.Frequency / 1000000.0:F3} MHz - {callsignsOnFreq.Count} aircraft on frequency but none visible on scope");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"RDF: Matched {matchedCount} transmitting aircraft on {freq.Frequency / 1000000.0:F3} MHz");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error detecting transmitting aircraft: {ex}");
            }
        }

        private void CleanupTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Remove transmissions older than 3 seconds (longer to ensure visibility)
                var cutoff = DateTime.UtcNow.AddSeconds(-3);
                var stale = currentlyTransmitting.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();

                foreach (var callsign in stale)
                {
                    currentlyTransmitting.TryRemove(callsign, out _);
                    System.Diagnostics.Debug.WriteLine($"RDF: Cleared transmission indicator for {callsign}");
                }

                // Also clean up receiving frequencies
                var staleFreqs = receivingFrequencies.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();
                foreach (var freq in staleFreqs)
                {
                    receivingFrequencies.TryRemove(freq, out _);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error in cleanup timer: {ex}");
            }
        }

        // IPlugin interface methods

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            // Not used for this plugin
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            // Track updates - not used currently
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            // Return custom label for transmitting aircraft
            if (!settings.Enabled) return null;

            if (itemType == "RDF_TX" && track != null)
            {
                var callsign = track.GetCallsign();
                if (!string.IsNullOrEmpty(callsign) && currentlyTransmitting.ContainsKey(callsign))
                {
                    // Show TX indicator
                    int count = currentlyTransmitting.Count;
                    string indicator = count > 1 ? "●" : "○";  // Filled circle for multiple, empty for single

                    return new CustomLabelItem()
                    {
                        Text = indicator,
                        BackColour = count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor,
                        ForeColour = Color.Black
                    };
                }
            }

            return null;
        }

        public CustomColour SelectASDTrackColour(Track track)
        {
            // Highlight transmitting aircraft with custom color
            if (!settings.Enabled) return null;

            if (track != null)
            {
                var callsign = track.GetCallsign();
                if (!string.IsNullOrEmpty(callsign) && currentlyTransmitting.ContainsKey(callsign))
                {
                    // Return custom color based on transmission count
                    int count = currentlyTransmitting.Count;
                    Color highlightColor = count > 1 ? settings.ConcurrentTxColor : settings.SingleTxColor;

                    return new CustomColour()
                    {
                        Colour = highlightColor
                    };
                }
            }

            return null;
        }

        public CustomColour SelectGroundTrackColour(Track track)
        {
            // Same logic as ASD tracks for ground tracks
            return SelectASDTrackColour(track);
        }
    }
}
