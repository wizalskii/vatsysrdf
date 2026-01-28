using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace VatsysRDF
{
    public class RDFSettings
    {
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public Color SingleTxColor { get; set; } = Color.White;

        [JsonProperty("SingleTxColor")]
        public string SingleTxColorHex
        {
            get => ColorTranslator.ToHtml(SingleTxColor);
            set => SingleTxColor = ColorTranslator.FromHtml(value);
        }

        [JsonIgnore]
        public Color ConcurrentTxColor { get; set; } = Color.Red;

        [JsonProperty("ConcurrentTxColor")]
        public string ConcurrentTxColorHex
        {
            get => ColorTranslator.ToHtml(ConcurrentTxColor);
            set => ConcurrentTxColor = ColorTranslator.FromHtml(value);
        }

        public int CircleRadius { get; set; } = 20;  // pixels
        public int RandomOffset { get; set; } = 0;    // nautical miles
        public bool RequireTxFrequency { get; set; } = false;
        public int LowAltitudeFilter { get; set; } = 0; // feet
        public int PenWidth { get; set; } = 2; // Circle line width

        private static string SettingsPath => Path.Combine(
            Path.GetDirectoryName(typeof(RDFSettings).Assembly.Location) ?? "",
            "RDFSettings.json"
        );

        public static RDFSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonConvert.DeserializeObject<RDFSettings>(json) ?? new RDFSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading RDF settings: {ex.Message}");
            }

            return new RDFSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving RDF settings: {ex.Message}");
            }
        }
    }
}
