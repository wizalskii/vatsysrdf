# VATSYS RDF Plugin v1.0.0

## 🎉 First Official Release

Radio Direction Finder (RDF) plugin for VATSYS that highlights aircraft based on VATSIM network data correlation.

## ✨ Features

- **Automatic Aircraft Highlighting**: Identifies and highlights aircraft on your frequencies
- **VATSIM Data Correlation**: Innovative solution using VATSIM public data feed to correlate specific transmitting aircraft
- **VHF and HF Support**: Works with both VHF (118-137 MHz) and HF (2-30 MHz) frequencies
- **Visual Indicators**: Custom label items display circles (○ for single transmission, ● for multiple)
- **Real-time Updates**: Refreshes every second with VATSIM data fetched every 15 seconds
- **No External Dependencies**: Uses VATSYS internal AFV, no TrackAudio required

## 📦 What's Included

- `VatsysRDF.dll` - Main plugin binary (16 KB)
- `Newtonsoft.Json.dll` - JSON parsing dependency
- `Labels.xml` - Custom label definition with RDF_TX indicator
- `RDFSettings.json` - Example configuration file
- `README.md` - Usage documentation
- `INSTALLATION.txt` - Step-by-step installation instructions

## 🚀 Installation

1. **Download** `VatsysRDF-v1.0.0.zip` from this release
2. **Extract** all files to your VATSYS Plugins folder:
   ```
   Documents\vatSys Files\[YourProfile]\Plugins\
   ```
3. **(Optional)** Copy `Labels.xml` to your profile folder and add "RDF_TX" to your aircraft label templates
4. **Restart** VATSYS

## 🔧 Configuration

Edit `RDFSettings.json` to customize:
- `Enabled`: Turn plugin on/off
- `SingleTxColor`: Color for single transmission highlighting
- `ConcurrentTxColor`: Color for multiple concurrent transmissions
- `RequireTxFrequency`: Require transmission detection (false = show all on frequency)
- `LowAltitudeFilter`: Minimum altitude in feet (0 = no filter)

## 🛠️ Technical Details

### How It Works

The plugin uses an innovative approach to identify specific transmitting aircraft:

1. **VATSIM Data Feed**: Fetches public VATSIM data every 15 seconds
2. **Frequency Mapping**: Builds frequency-to-callsign correlations from network data
3. **Controller Monitoring**: Tracks your active VHF and HF frequencies
4. **Smart Correlation**: Identifies which aircraft are on your frequencies
5. **Visual Feedback**: Highlights those aircraft with custom label indicators

### API Integration

- Uses VATSYS IPlugin interface with MEF `[Export(typeof(IPlugin))]`
- `Network.Me.Frequencies` to get controller frequencies
- `Track.GetFDR().Callsign` for aircraft callsign access
- `GetCustomLabelItem` for visual indicators
- Thread-safe concurrent collections for transmission tracking

## 📋 Requirements

- VATSYS (tested with current version)
- .NET Framework 4.7.2
- Active VATSIM connection
- Controller position with assigned frequencies

## 🐛 Troubleshooting

**Plugin not loading?**
1. Verify DLL is in correct Plugins folder
2. Ensure Newtonsoft.Json.dll is also present
3. Restart VATSYS completely
4. Check Windows Event Viewer for errors

**No indicators showing?**
1. Confirm `Enabled: true` in RDFSettings.json
2. Connect to VATSIM network
3. Ensure aircraft are on your frequencies
4. Wait 15 seconds for initial VATSIM data load
5. Add "RDF_TX" to your label templates

## 📝 License

MIT License - Free to use and modify
Copyright (c) 2026 wizalskii

## 🙏 Credits

- Inspired by the EuroScope RDF plugin by KingfuChan
- Built for VATSYS with VATSIM data correlation
- Powered by Newtonsoft.Json library

## 🔗 Links

- **GitHub Repository**: https://github.com/wizalskii/vatsysrdf
- **Issues**: https://github.com/wizalskii/vatsysrdf/issues
- **VATSIM**: https://www.vatsim.net

## 📈 Future Enhancements

- Track color highlighting (investigating CustomColour API)
- Configuration UI within VATSYS
- Performance optimizations for high-density airspace
- Additional visual indicator options

---

**Enjoy the plugin and happy controlling!** 🎮✈️
