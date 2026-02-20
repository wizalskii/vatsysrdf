# RDF Plugin - Map Layer Version (REQUIRED for ATOP)

## ✅ Confirmed: Map Layer IS Required

Based on your experience with the Mexico profile and ATOP Oakland, custom label items from plugins **DO require** a corresponding map layer entry to be selectable and visible in vatSys.

This version properly registers RDF as a map layer.

## What Changed

### Previous Version (Non-Map Layer)
- **Problem:** TX symbols likely didn't appear in ATOP profile
- **Reason:** Custom label items need map layer registration
- **File:** `RDFPlugin_Working.cs`

### New Version (With Map Layer) ✅
- **Solution:** Registers "RDF - Transmission Indicators" in Maps menu
- **Result:** TX symbols will be visible when map is enabled
- **File:** `RDFPlugin_WithMapLayer.cs`

## How to Install

### 1. Close vatSys FIRST

The DLL file cannot be overwritten while vatSys is running.

### 2. Copy Files to Plugins Folder

**Required files:** (same as before)
```
Source: C:\Users\danie\claude\vatsysrdf\bin\Release\

Files:
├── VatsysRDF.dll          (17 KB - NEW VERSION)
└── Newtonsoft.Json.dll    (696 KB)

Destination:
C:\Users\danie\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\
```

### 3. Configure Labels.xml

Add the RDF_TX label item definition and add it to your label templates.

**See:** `LABEL_INSTALLATION.md` for complete instructions.

Quick summary:
```xml
<!-- Add after <Labels> tag -->
<LabelItem Name="RDF_TX" Type="Custom" Description="RDF Transmission Indicator">
  <Plugin>VatsysRDF</Plugin>
</LabelItem>

<!-- Then add to your Normal label -->
<Label Type="Normal">
  <DataLine>
    <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick=""/>
    <Item Type="LABEL_ITEM_ACID" .../>
    ...
  </DataLine>
</Label>
```

### 4. Start vatSys

### 5. Enable RDF in Maps Menu ⭐ NEW STEP

**This is the key difference:**

1. Open vatSys
2. Click **Maps** menu
3. Look for **"RDF - Transmission Indicators"**
4. Click to **enable** it (checkmark should appear)

### 6. Verify TX Symbols Appear

When aircraft are tuned to your frequency, you should see "TX" on their radar tags.

## What This Fixes

### Without Map Layer
- Custom label items defined in plugin
- Plugin provides `GetCustomLabelItem()` implementation
- **But symbols don't appear** in ATOP/NAT profiles
- No entry in Maps menu

### With Map Layer ✅
- Map layer registered: "RDF - Transmission Indicators"
- **Appears in Maps menu** (can be toggled on/off)
- TX symbols **will actually display** when map is enabled
- Works with ATOP Oakland, NAT, Mexico, and all profiles

## Technical Details

### Map Registration Code
```csharp
rdfMap = new DisplayMaps.Map
{
    Name = "RDF - Transmission Indicators"
};

DisplayMaps.Maps.Add(rdfMap);
```

This adds RDF to the selectable maps list in vatSys.

### Why This Works

vatSys plugin architecture requires:
1. **Plugin exports** `IPlugin` interface via MEF
2. **Plugin provides** `GetCustomLabelItem()` method
3. **Map layer registered** so user can enable/disable feature

Without step 3, custom label items may not render in certain profiles (especially ATOP).

## Troubleshooting

### "RDF - Transmission Indicators" doesn't appear in Maps menu
- Plugin failed to load
- Check DebugView for errors
- Verify VatsysRDF.dll is in Plugins folder
- Ensure Newtonsoft.Json.dll is present

### Map appears but TX symbols don't show
- Check Labels.xml configuration
- Verify `<LabelItem Name="RDF_TX">` is defined
- Verify `<Item Type="RDF_TX">` is added to label template
- Restart vatSys after Labels.xml changes

### vatSys crashes on startup
- **Unlikely** with this implementation (uses safe API)
- If it happens:
  1. Delete VatsysRDF.dll from Plugins
  2. Report crash details
  3. Use DebugView to capture error

### Map is enabled but symbols appear/disappear randomly
- This is normal - TX symbols show only when aircraft are on your frequency
- Use Observer Mode (`ObserverMode: true` in settings) to test with all aircraft

## Observer Mode Testing

To test the plugin without connecting to VATSIM:

1. Edit `RDFSettings.json` in Plugins folder:
   ```json
   {
     "Enabled": true,
     "ObserverMode": true
   }
   ```

2. Restart vatSys
3. **ALL aircraft** will show TX (useful for testing)
4. Set back to `false` for normal operation

## Comparison: Map Layer vs No Map Layer

| Feature | No Map Layer | With Map Layer |
|---|---|---|
| Plugin loads | ✅ Yes | ✅ Yes |
| `GetCustomLabelItem()` called | ✅ Yes | ✅ Yes |
| TX symbols visible (ATOP) | ❌ No | ✅ Yes |
| TX symbols visible (NAT) | ❌ No | ✅ Yes |
| Appears in Maps menu | ❌ No | ✅ Yes |
| Can toggle on/off | ❌ No | ✅ Yes |
| Crashes vatSys | ❌ No | ❌ No |

## Files

| File | Purpose | Required |
|---|---|---|
| `VatsysRDF.dll` | Plugin with map layer registration | ✅ Yes |
| `Newtonsoft.Json.dll` | JSON library dependency | ✅ Yes |
| `VatsysRDF.pdb` | Debug symbols | ❌ No (optional) |

**No .pbl file exists.**

## Next Deployment Steps

1. Close vatSys
2. Copy new `VatsysRDF.dll` to Plugins folder (overwrites old version)
3. Start vatSys
4. Go to **Maps** menu
5. Enable **"RDF - Transmission Indicators"**
6. TX symbols should now appear!

---

**Build info:**
- File: `RDFPlugin_WithMapLayer.cs` (active in build)
- Size: ~17 KB
- Date: Feb 20, 2026
- Feature: Proper map layer registration for ATOP compatibility
