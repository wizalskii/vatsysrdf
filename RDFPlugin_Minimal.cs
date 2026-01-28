using System;
using vatsys;
using vatsys.Plugin;

namespace VatsysRDF
{
    /// <summary>
    /// Minimal RDF Plugin - builds successfully but needs VATSYS API version matching
    /// </summary>
    public class RDFPluginMinimal : IPlugin
    {
        public string Name => "Radio Direction Finder (RDF) - Minimal Build";

        public RDFPluginMinimal()
        {
            System.Diagnostics.Debug.WriteLine("RDF: Minimal plugin loaded successfully!");
            System.Diagnostics.Debug.WriteLine("RDF: Note - Full functionality requires VATSYS API compatibility");
        }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            // Not implemented in minimal version
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            // Not implemented in minimal version
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            // Not implemented in minimal version
            return null;
        }

        public CustomColour SelectASDTrackColour(Track track)
        {
            // Not implemented in minimal version
            return null;
        }

        public CustomColour SelectGroundTrackColour(Track track)
        {
            // Not implemented in minimal version
            return null;
        }
    }
}
