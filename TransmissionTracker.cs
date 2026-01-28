using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using vatsys;

namespace VatsysRDF
{
    public class TransmissionData
    {
        public string Callsign { get; set; }
        public Track Track { get; set; }
        public DateTime StartTime { get; set; }
        public PointF ScreenPosition { get; set; }
        public Coordinate GeoPosition { get; set; }
        public double Radius { get; set; }

        public TransmissionData(string callsign, Track track)
        {
            Callsign = callsign;
            Track = track;
            StartTime = DateTime.UtcNow;
            Radius = 20; // Default radius in pixels

            if (track?.GetPosition() != null)
            {
                GeoPosition = track.GetPosition();
            }
        }
    }

    public class TransmissionTracker
    {
        private readonly ConcurrentDictionary<string, TransmissionData> activeTransmissions;
        private readonly RDFSettings settings;
        private readonly object lockObject = new object();

        public TransmissionTracker(RDFSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            activeTransmissions = new ConcurrentDictionary<string, TransmissionData>();
        }

        public void AddTransmission(string callsign, Track track)
        {
            if (string.IsNullOrWhiteSpace(callsign) || track == null)
                return;

            // Check altitude filter
            if (settings.LowAltitudeFilter > 0)
            {
                var radarTrack = track.GetRadarTrack();
                if (radarTrack != null && radarTrack.ActualAltitude < settings.LowAltitudeFilter)
                {
                    return; // Skip low-altitude aircraft
                }
            }

            var txData = new TransmissionData(callsign, track);
            activeTransmissions.AddOrUpdate(callsign, txData, (key, old) => txData);

            System.Diagnostics.Debug.WriteLine($"RDF: Added transmission from {callsign}");
        }

        public void RemoveTransmission(string callsign)
        {
            if (activeTransmissions.TryRemove(callsign, out _))
            {
                System.Diagnostics.Debug.WriteLine($"RDF: Removed transmission from {callsign}");
            }
        }

        public void UpdatePositions(PositionConverter converter)
        {
            if (converter == null) return;

            foreach (var tx in activeTransmissions.Values)
            {
                if (tx.Track != null)
                {
                    var pos = tx.Track.GetPosition();
                    if (pos != null)
                    {
                        tx.GeoPosition = pos;
                        tx.ScreenPosition = converter.ConvertToScreen(pos);
                    }
                }
            }
        }

        public List<TransmissionData> GetActiveTransmissions()
        {
            return activeTransmissions.Values.ToList();
        }

        public int Count => activeTransmissions.Count;

        public void Clear()
        {
            activeTransmissions.Clear();
        }

        // Clean up stale transmissions (e.g., older than 10 seconds without update)
        public void CleanupStale(TimeSpan maxAge)
        {
            var now = DateTime.UtcNow;
            var staleCallsigns = activeTransmissions
                .Where(kvp => now - kvp.Value.StartTime > maxAge)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var callsign in staleCallsigns)
            {
                RemoveTransmission(callsign);
                System.Diagnostics.Debug.WriteLine($"RDF: Removed stale transmission from {callsign}");
            }
        }
    }
}
