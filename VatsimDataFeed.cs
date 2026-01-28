using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VatsysRDF
{
    public class VatsimPilot
    {
        [JsonProperty("callsign")]
        public string Callsign { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("altitude")]
        public int Altitude { get; set; }
    }

    public class VatsimData
    {
        [JsonProperty("pilots")]
        public List<VatsimPilot> Pilots { get; set; }
    }

    public class VatsimDataFeed
    {
        private const string VATSIM_DATA_URL = "https://data.vatsim.net/v3/vatsim-data.json";
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly ConcurrentDictionary<string, HashSet<uint>> callsignFrequencies =
            new ConcurrentDictionary<string, HashSet<uint>>();

        private readonly ConcurrentDictionary<uint, HashSet<string>> frequencyCallsigns =
            new ConcurrentDictionary<uint, HashSet<string>>();

        private Timer updateTimer;
        private bool isRunning = false;

        public VatsimDataFeed()
        {
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public void Start()
        {
            if (isRunning) return;

            isRunning = true;

            // Initial fetch
            _ = UpdateDataAsync();

            // Update every 15 seconds (VATSIM updates their feed every 15 seconds)
            updateTimer = new Timer(async _ => await UpdateDataAsync(), null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));

            System.Diagnostics.Debug.WriteLine("RDF: VATSIM data feed started");
        }

        public void Stop()
        {
            isRunning = false;
            updateTimer?.Dispose();
            updateTimer = null;

            System.Diagnostics.Debug.WriteLine("RDF: VATSIM data feed stopped");
        }

        private async Task UpdateDataAsync()
        {
            try
            {
                var response = await httpClient.GetStringAsync(VATSIM_DATA_URL);
                var data = JsonConvert.DeserializeObject<VatsimData>(response);

                if (data?.Pilots == null) return;

                // Clear old data
                callsignFrequencies.Clear();
                frequencyCallsigns.Clear();

                // Build frequency-to-callsign mappings
                foreach (var pilot in data.Pilots)
                {
                    if (string.IsNullOrEmpty(pilot.Callsign) || string.IsNullOrEmpty(pilot.Frequency))
                        continue;

                    // Parse frequency string (e.g., "118.500") to Hz
                    if (double.TryParse(pilot.Frequency, out double freqMhz))
                    {
                        uint freqHz = (uint)(freqMhz * 1000000); // Convert MHz to Hz

                        // Add to callsign -> frequencies mapping
                        callsignFrequencies.AddOrUpdate(
                            pilot.Callsign,
                            new HashSet<uint> { freqHz },
                            (key, set) => { set.Add(freqHz); return set; }
                        );

                        // Add to frequency -> callsigns mapping
                        frequencyCallsigns.AddOrUpdate(
                            freqHz,
                            new HashSet<string> { pilot.Callsign },
                            (key, set) => { set.Add(pilot.Callsign); return set; }
                        );
                    }
                }

                System.Diagnostics.Debug.WriteLine($"RDF: Updated VATSIM data - {data.Pilots.Count} pilots, {frequencyCallsigns.Count} unique frequencies");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Error updating VATSIM data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all callsigns currently tuned to a specific frequency
        /// </summary>
        public HashSet<string> GetCallsignsOnFrequency(uint frequencyHz)
        {
            if (frequencyCallsigns.TryGetValue(frequencyHz, out var callsigns))
            {
                return new HashSet<string>(callsigns);
            }

            // Also check nearby frequencies (+/- 5 kHz tolerance for rounding)
            var nearby = new HashSet<string>();
            foreach (var kvp in frequencyCallsigns)
            {
                if (Math.Abs((int)kvp.Key - (int)frequencyHz) <= 5000) // 5 kHz tolerance
                {
                    nearby.UnionWith(kvp.Value);
                }
            }

            return nearby;
        }

        /// <summary>
        /// Check if a specific callsign is on a specific frequency
        /// </summary>
        public bool IsCallsignOnFrequency(string callsign, uint frequencyHz)
        {
            if (callsignFrequencies.TryGetValue(callsign, out var frequencies))
            {
                return frequencies.Any(f => Math.Abs((int)f - (int)frequencyHz) <= 5000);
            }

            return false;
        }

        /// <summary>
        /// Get the frequency a callsign is tuned to (returns 0 if not found)
        /// </summary>
        public uint GetCallsignFrequency(string callsign)
        {
            if (callsignFrequencies.TryGetValue(callsign, out var frequencies))
            {
                return frequencies.FirstOrDefault();
            }

            return 0;
        }

        /// <summary>
        /// Get all callsigns currently in VATSIM data (for observer mode)
        /// </summary>
        public HashSet<string> GetAllCallsigns()
        {
            return new HashSet<string>(callsignFrequencies.Keys);
        }
    }
}
