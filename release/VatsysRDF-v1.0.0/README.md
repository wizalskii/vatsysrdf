# VATSYS RDF Plugin

Radio Direction Finder (RDF) plugin for VATSYS that provides visual indicators when aircraft transmit on radio frequencies.

## Features

- **Real-time Transmission Detection**: Detects when aircraft transmit on monitored frequencies
- **Visual Indicators**:
  - Custom label item showing transmission indicator (○ or ●)
  - Track color highlighting for transmitting aircraft
- **VHF & HF Support**: Works with both VHF (118-137 MHz) and HF (2-30 MHz) frequencies
- **Color Coding**:
  - White (○): Single transmission
  - Red (●): Multiple concurrent transmissions
- **Configurable**: Customizable colors via JSON configuration
- **No External Dependencies**: Works with VATSYS's built-in AFV - NO TrackAudio required

## How It Works

The plugin uses VATSYS's Audio API to detect when transmissions occur:
1. Monitors all active frequencies (VHF and HF)
2. When `ReceivingChanged` event fires, marks potential transmitters
3. Shows visual indicators on aircraft tracks using:
   - **Custom Label Item** (`RDF_TX`): Circle indicator next to callsign
   - **Track Color**: Highlights the entire track symbol

## Installation

### Quick Start

> **Note:** A pre-built DLL is not yet available. You'll need to build from source. See [BUILD.md](BUILD.md) for instructions.

1. **Build the plugin:**
   - See detailed instructions in [BUILD.md](BUILD.md)
   - Requires Visual Studio and VATSYS installed
   - Build in **Release** configuration

2. **Install the DLL:**
   - Copy `bin\Release\VatsysRDF.dll` to your VATSYS profile's `Plugins` folder:
   - Typical path: `Documents\vatSys Files\[ProfileName]\Plugins\`
   - Create the Plugins folder if it doesn't exist

3. **Add Label Item** (optional for label indicator):
   - Copy `Labels.xml` to your profile folder
   - Edit your aircraft label templates in VATSYS to include `RDF_TX`

4. **Restart VATSYS**

**Detailed Guide:** See [INSTALLATION.md](INSTALLATION.md)

## Configuration

The plugin creates a `RDFSettings.json` file in the Plugins directory:

```json
{
  "Enabled": true,
  "SingleTxColor": "#FFFFFF",
  "ConcurrentTxColor": "#FF0000",
  "RequireTxFrequency": false,
  "LowAltitudeFilter": 0
}
```

### Settings:
- **Enabled**: Enable/disable the plugin
- **SingleTxColor**: Color for single transmission (HTML color code, default: white)
- **ConcurrentTxColor**: Color for multiple concurrent transmissions (HTML color code, default: red)
- **RequireTxFrequency**: Only show transmissions on frequencies you're transmitting on
- **LowAltitudeFilter**: Minimum altitude in feet to display (0 = show all)

## Usage

Once installed, the plugin automatically:
1. Monitors all active VSCS frequencies
2. Detects transmission events via `ReceivingChanged`
3. Highlights transmitting aircraft with colored track symbols
4. Shows custom label indicators (if configured)

### Visual Indicators

- **Track Color**: Aircraft track symbols change to white (single TX) or red (multiple TX)
- **Label Item**: Circle indicator appears if `RDF_TX` is added to your label template:
  - `○` White circle for single transmission
  - `●` Red filled circle for concurrent transmissions

## How to Add the Label Item

To see the circle indicator next to callsigns:

1. Open VATSYS
2. Go to your profile folder: `Documents\vatSys Files\[ProfileName]\`
3. Edit `Labels.xml` or create it with:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Labels>
  <LabelItem Name="RDF_TX" Type="Custom" />
</Labels>
```

4. In VATSYS, edit your aircraft label template to include `RDF_TX`
5. The circle indicator will now appear when aircraft transmit

## How It Identifies Specific Aircraft

The plugin solves the VATSYS API limitation using **VATSIM's public data feed**:

1. **Fetches VATSIM Data**: Retrieves real-time data from `data.vatsim.net` every 15 seconds
2. **Builds Frequency Map**: Creates a mapping of which callsigns are tuned to which frequencies
3. **Correlates Transmissions**: When a transmission is detected on frequency X:
   - Queries which aircraft are on frequency X (from VATSIM data)
   - Matches those callsigns to visible tracks on your scope
   - **Only highlights those specific aircraft!**

### This Means:
- ✅ Shows **only the specific transmitting aircraft** (not all aircraft!)
- ✅ Works with both VHF and HF frequencies
- ✅ Automatic correlation using live VATSIM network data
- ✅ No TrackAudio or external dependencies needed
- ⏱️ Indicators clear automatically after 3 seconds

### Accuracy:
- **Highly accurate** when aircraft are on the VATSIM network and visible on your scope
- Works for all pilots connected to VATSIM (the data source is official)
- Handles frequency rounding (±5 kHz tolerance)

## Building from Source

### Requirements
- Visual Studio 2017 or later
- .NET Framework 4.7.2
- VATSYS installation (for vatSys.exe reference)

### Build Steps
1. Clone this repository
2. Open `VatsysRDF.sln` in Visual Studio
3. Update the vatSys.exe reference path in `VatsysRDF.csproj` if needed
4. Build the solution (Release configuration)
5. DLL will be in `bin\Release\VatsysRDF.dll`

## File Structure

```
VatsysRDF/
├── RDFPlugin.cs           # Main plugin implementing IPlugin
├── VatsimDataFeed.cs      # Fetches VATSIM data for frequency correlation
├── RDFSettings.cs         # Configuration management
├── TransmissionTracker.cs # Manages active transmissions
├── PositionConverter.cs   # Position utilities (for future use)
├── RDFOverlay.cs          # Overlay window (not currently used)
├── Labels.xml             # Custom label definition
└── README.md              # This file
```

## Debugging

The plugin writes debug output to the debug console:

1. Download [DebugView](https://learn.microsoft.com/en-us/sysinternals/downloads/debugview) from Microsoft
2. Run DebugView as Administrator
3. Start VATSYS
4. Watch for lines starting with "RDF:"
5. You'll see events like:
   ```
   RDF: Plugin initialized successfully
   RDF: Subscribed to frequency 118.500 MHz
   RDF: Receiving changed on 118.500 MHz - Receiving: True
   RDF: Marking BAW123 as potentially transmitting
   ```

## Troubleshooting

**Plugin not loading:**
- Check that `VatsysRDF.dll` is in the correct Plugins folder
- Ensure vatSys.exe reference path is correct for your installation
- Check DebugView for initialization errors

**No visual indicators appearing:**
- Verify plugin is enabled in `RDFSettings.json`
- For label indicators: Ensure `RDF_TX` is in your Labels.xml and label template
- For track colors: They should appear automatically when transmissions are detected
- Check DebugView to confirm transmission detection is working

**Label item not showing:**
- Edit your label template in VATSYS to include the `RDF_TX` item
- Make sure Labels.xml is in your profile directory
- Restart VATSYS after changing Labels.xml

## Future Enhancements

Potential improvements:
1. ~~Integration with VATSIM network data~~ ✅ **IMPLEMENTED!**
2. Configuration UI within VATSYS (currently JSON file)
3. Per-frequency filtering options in settings
4. Transmission history/log window
5. Multiple concurrent transmission detection (show count)
6. Custom sounds on transmission detection
7. Aircraft proximity filtering (only show nearby transmitters)

## Technical Details

### Plugin Architecture
- Uses VATSYS's `IPlugin` interface for plugin integration
- Implements `GetCustomLabelItem()` for label indicators
- Implements `SelectASDTrackColour()` for track highlighting
- Monitors VATSYS Audio API for transmission events
- Fetches VATSIM public data feed for frequency-to-callsign correlation
- Thread-safe concurrent dictionaries for transmission tracking

### How It Works Technically

1. **Audio Event Detection** (`RDFPlugin.cs`):
   - Subscribes to `Audio.VSCSFrequency.ReceivingChanged` events
   - Detects when transmission starts/ends on monitored frequencies

2. **VATSIM Data Correlation** (`VatsimDataFeed.cs`):
   - Fetches `https://data.vatsim.net/v3/vatsim-data.json` every 15 seconds
   - Builds mapping: `frequency → [callsigns]` and `callsign → [frequencies]`
   - Provides fast lookup of which aircraft are on a given frequency

3. **Specific Aircraft Identification** (`DetectTransmittingAircraft()`):
   - When transmission detected on frequency X:
     - Queries VATSIM data: "Which callsigns are on frequency X?"
     - Gets visible tracks from `MMI.Tracks`
     - Matches callsigns from VATSIM to visible tracks
     - **Only marks matched aircraft as transmitting**

4. **Visual Indication**:
   - `GetCustomLabelItem()` returns circle indicator for transmitting aircraft
   - `SelectASDTrackColour()` returns custom color for transmitting tracks
   - Clears after 3 seconds (cleanup timer)

### Why No TrackAudio?
This plugin works entirely with VATSYS's internal AFV system and VATSIM public data. No external dependencies required!

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

- 🐛 [Report a bug](https://github.com/wizalskii/vatsysrdf/issues/new?template=bug_report.md)
- 💡 [Request a feature](https://github.com/wizalskii/vatsysrdf/issues/new?template=feature_request.md)
- 🔧 [Submit a pull request](https://github.com/wizalskii/vatsysrdf/pulls)

## License

MIT License - See [LICENSE](LICENSE) for details

Copyright (c) 2026 wizalskii

## Credits

Inspired by the EuroScope RDF plugin by [KingfuChan](https://github.com/KingfuChan/RDF).
Built for VATSYS using only internal APIs.
