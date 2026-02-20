# CLAUDE.md — VatSys RDF Plugin (Zoom-Responsive Rings Edition)

## What This Project Is

A C# Class Library (DLL) plugin for [vatSys](https://virtualairtrafficsystem.com/) that implements Radio Direction Finding (RDF) with **zoom-responsive visual rings**. Shows TX indicators on radar tags and draws dynamic rings around aircraft tuned to controller frequencies.

The plugin:
1. Fetches VATSIM data feed every 15 seconds
2. Correlates callsigns against controller frequencies
3. Displays `●`/`○` TX symbols on radar tags with custom colors
4. Draws zoom-responsive rings via transparent overlay window
5. Ring size scales automatically with map zoom level (constant nautical mile radius)

## Build Environment

- **Language:** C# targeting .NET Framework 4.7.2
- **Output:** `VatsysRDF.dll` (Class Library)
- **Build tool:** MSBuild via Visual Studio 2022 Community
- **Key reference:** `C:\Program Files (x86)\vatSys\bin\vatSys.exe` (x86 assembly)
- **Dependency:** Newtonsoft.Json 13.0.3

Build command:
```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' VatsysRDF.sln /p:Configuration=Release '/p:Platform=Any CPU' /v:minimal /t:Rebuild
```

Deploy:
```powershell
Copy-Item bin\Release\VatsysRDF.dll "C:\Users\<User>\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\"
Copy-Item bin\Release\Newtonsoft.Json.dll "C:\Users\<User>\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\"
```

## Current State (February 2026)

**Active Build:** `RDFPlugin_ZoomResponsive.cs` + `RDFOverlay_ZoomResponsive.cs`

### What Works Now ✅

1. **VATSIM Data Correlation** — `VatsimDataFeed.cs` fetches and indexes pilots by frequency
2. **Radar Tag TX Indicator** — `GetCustomLabelItem()` returns "TX" text on labels:
   - Shows "TX" on aircraft tuned to your frequency
   - Positioned based on `Labels.xml` configuration (recommended: top of tag)
3. **Track Coloring** — `SelectASDTrackColour()` returns colored track symbols:
   - Teal (`#008080`) for single transmission
   - Red (`#FF0000`) for concurrent transmissions
   - Uses `new CustomColour(R, G, B)` constructor
4. **Observer Mode** — Shows all aircraft when no frequencies assigned

### How Zoom-Responsive Rings Work 🎯

The overlay calculates pixel radius dynamically:

1. Get aircraft position as `Coordinate` from `FDR.GetLocation()`
2. Calculate offset position `radiusNM / 60.0` degrees latitude away
3. Convert both to screen coordinates via `MMI.FPASD.GetPaintPoint()`
4. Measure screen pixel distance = zoom-responsive radius
5. Ring maintains constant nautical mile size regardless of zoom level

Implementation: `RDFOverlay_ZoomResponsive.cs:111-143`

## Architecture

### File Map

| File | Role | In build? |
|---|---|---|
| `RDFPlugin_ZoomResponsive.cs` | **ACTIVE** plugin entry point | ✅ Yes |
| `RDFOverlay_ZoomResponsive.cs` | **ACTIVE** transparent overlay with zoom calc | ✅ Yes |
| `RDFSettings.cs` | JSON settings persistence | ✅ Yes |
| `VatsimDataFeed.cs` | VATSIM data fetch + frequency index | ✅ Yes |
| `RDFPlugin_Working.cs` | Previous working version (no overlay) | No |
| `RDFPlugin.cs` | Original attempt (API mismatches) | No |
| `RDFOverlay.cs` | Original overlay (fixed pixel radius) | No |

### Deployment Files

| File | Destination |
|---|---|
| `VatsysRDF.dll` | `Profiles\<Profile>\Plugins\` |
| `Newtonsoft.Json.dll` | `Profiles\<Profile>\Plugins\` |
| Label config | Edit `Profiles\<Profile>\Labels.xml` (see below) |

## Label Configuration

Must add `RDF_TX` custom item to profile's `Labels.xml`:

### Step 1: Define custom item (after `<Labels>` tag)

```xml
<LabelItem Name="RDF_TX" Type="Custom" Description="RDF Transmission Indicator">
  <Plugin>VatsysRDF</Plugin>
</LabelItem>
```

### Step 2: Add to label templates

```xml
<Label Type="Normal" MaximumWidth="11" LabelRectangle="Default">
  <DataLine>
    <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick="" LeftPadding="0"/>
    <Item Type="LABEL_ITEM_ACID" Colour="" .../>
    ...
  </DataLine>
```

See `LABEL_INSTALLATION.md` for complete guide (works with ATOP and NAT profiles).

## Settings (`RDFSettings.json`)

Auto-created in Plugins folder on first run:

```json
{
  "Enabled": true,
  "SingleTxColor": "#008080",      // Teal for single TX
  "ConcurrentTxColor": "#FF0000",  // Red for concurrent TX
  "CircleRadius": 20,              // Deprecated (overlay uses zoom calculation)
  "RandomOffset": 0,
  "RequireTxFrequency": false,
  "LowAltitudeFilter": 0,
  "PenWidth": 2,                   // Ring line width
  "ObserverMode": true             // Show all aircraft when no freqs assigned
}
```

**Note:** `CircleRadius` is no longer used in zoom-responsive version. Ring size determined by `CalculateZoomResponsiveRadius()` using 5 NM constant.

## vatSys API Knowledge (v0.4.9305)

Confirmed working APIs:

| API | Usage |
|---|---|
| `new CustomColour(R, G, B)` | Track/label colors |
| `ForeColourIdentity = Colours.Identities.Custom` | Enable custom label colors |
| `FDR.GetLocation()` | Returns `Coordinate` |
| `FDR.Callsign` | Aircraft callsign |
| `FDR.CoupledTrack` | Returns `RDP.RadarTrack` |
| `MMI.FPASD` | Main ASD window reference |
| `MMI.FPASD.GetPaintPoint(Coordinate)` | Coord → screen pixel |
| `MMI.FPASD.Bounds` | Window rectangle |
| `Network.Me.Frequencies` | int[] in kHz (multiply by 1000 for Hz) |
| `FDP2.GetFDRs` | All flight data records |

Confirmed NOT available:
- `CustomLabelItem.BackColour` / `ForeColour` (use ForeColourIdentity instead)
- `Track.GetPosition()` / `GetCallsign()` (use FDR methods)
- `MMI.Tracks` / `MMI.InhibitedAirportMovements`
- Public radar drawing API (use overlay window instead)

## Known Issues & Limitations

### 1. Frequency Matching Accuracy

`Network.Me.Frequencies` returns kHz integers. VATSIM data has MHz strings. Current conversion:
- vatSys: `freqInt * 1000` = Hz
- VATSIM: `freqMhz * 1000000` = Hz
- Tolerance: ±5 kHz matching

**If matching fails:** Log frequencies via DebugView and verify conversion math.

### 2. Overlay Performance

Transparent overlay redraws at 100ms (10 FPS). On low-end systems with many aircraft, may cause slight lag. To optimize:
- Increase `updateTimer.Interval` in `RDFPlugin_ZoomResponsive.cs:44`
- Reduce max ring radius clamp in `CalculateZoomResponsiveRadius()`

### 3. Multi-Monitor Setup

Overlay window bounds taken from `MMI.FPASD.Bounds`. If vatSys moves between monitors, overlay may not follow immediately. Workaround: restart vatSys.

### 4. Profile-Specific Labels

Each profile needs `RDF_TX` added to its own `Labels.xml`. No global plugin label registration in vatSys.

## Debugging

Use [DebugView](https://learn.microsoft.com/en-us/sysinternals/downloads/debugview) to monitor plugin:

```
RDF: Plugin initializing (Zoom Responsive)...
RDF: Plugin initialized successfully
RDF: Updated VATSIM data - 1234 pilots, 234 unique frequencies
RDF: Showing indicator '●' for UAL123
```

### Common Issues

| Symptom | Check |
|---|---|
| No symbols on labels | `Labels.xml` config, verify `<LabelItem>` and item added to template |
| No colored rings | Overlay window failed to create, check DebugView for errors |
| Rings wrong size | `GetPaintPoint()` failing, verify `MMI.FPASD` is available |
| No aircraft highlighted | Frequency mismatch, verify `Network.Me.Frequencies` units |
| Overlay click-through not working | Windows transparency issue, check `CreateParams` |

## Next Steps

- [ ] Add configurable ring radius (NM) to `RDFSettings.json`
- [ ] Support multiple ring sizes for different zoom levels
- [ ] Add option to disable overlay (label-only mode)
- [ ] Cache `GetPaintPoint()` calls per frame to reduce API overhead
- [ ] Add visual TX indicator on frequency change (flash effect)
- [ ] Support for ground vehicles (currently aircraft only)
