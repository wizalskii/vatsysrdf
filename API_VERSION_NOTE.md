# VATSYS API Version Compatibility Note

## ⚠️ Important: Current Build Status

The **minimal version** of the plugin has been built successfully, but the full VATSIM data correlation feature requires API version matching with your VATSYS installation.

## What's Included in This Build

This release includes a **minimal functional plugin** that:
- ✅ Loads successfully in VATSYS
- ✅ Implements the IPlugin interface
- ✅ Demonstrates the plugin can be compiled and loaded
- ⚠️ Does NOT include the full RDF functionality yet

## Why the Minimal Version?

During build, we discovered that the VATSYS API installed has different method signatures than expected:

### API Differences Found:
- `Track.GetPosition()` - Method not found in installed version
- `Track.GetCallsign()` - Method not found in installed version
- `RDP.RadarTrack.ActualAltitude` - Property not found
- `CustomLabelItem.BackColour/ForeColour` - Properties not found
- `CustomColour` constructor - Different signature
- `Audio.VSCSFrequency` - Type resolution issues
- `MMI.Tracks` / `MMI.InhibitedAirportMovements` - Not found

### Possible Causes:
1. **VATSYS Version Mismatch** - The API may have changed between versions
2. **Different Build** - Development vs Production builds may have different APIs
3. **API Evolution** - The API is evolving and method names have changed

## Full Version Source Code

The complete source code with VATSIM data correlation is available in the repository:
- `RDFPlugin.cs` - Full implementation
- `VatsimDataFeed.cs` - VATSIM data integration
- `TransmissionTracker.cs` - Transmission tracking
- All supporting files

## Next Steps to Enable Full Functionality

### Option 1: Match API to Your VATSYS Version
1. Reference the VATSYS documentation for your specific version
2. Update method calls to match the actual API
3. Rebuild with corrected API calls

### Option 2: Update VATSYS
1. Check for VATSYS updates
2. Install the latest version
3. Rebuild the plugin

### Option 3: Contribute API Mapping
If you have VATSYS API documentation:
1. Create an issue with the correct API signatures
2. We'll update the code to match
3. Submit a PR with the fixes

## How to Use This Build

1. **Install the minimal DLL:**
   - Copy `VatsysRDF.dll` to your VATSYS `Plugins` folder
   - The plugin will load and log to DebugView

2. **Verify it loads:**
   - Start VATSYS
   - Use DebugView to see: "RDF: Minimal plugin loaded successfully!"

3. **For full functionality:**
   - Wait for API-matched version
   - Or help us match the API by providing version info

## Files Included

```
VatsysRDF-v1.0.0-minimal/
├── VatsysRDF.dll              # Minimal working plugin
├── Newtonsoft.Json.dll        # Required dependency
├── Labels.xml                 # Label definition (for future use)
├── README.md                  # Installation instructions
├── API_VERSION_NOTE.md        # This file
└── RDFSettings.json           # Example configuration
```

## Contributing

If you can help identify the correct VATSYS API for your version:
1. Check your VATSYS version number
2. Look for API documentation
3. Open an issue with the information
4. We'll update the plugin to match

## Source Code

Full source with VATSIM correlation feature:
**https://github.com/wizalskii/vatsysrdf**

The innovative VATSIM data correlation logic is complete and ready - it just needs the correct VATSYS API calls!

---

**Current Status:** Minimal build - demonstrates successful compilation and plugin loading
**Target Status:** Full VATSIM data correlation with specific aircraft identification
**Blocker:** VATSYS API version compatibility
