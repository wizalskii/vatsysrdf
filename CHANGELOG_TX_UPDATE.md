# TX Symbol Update - February 20, 2026

## Changes Made

### 1. TX Symbol Text Changed ✅
**Before:** `●` (filled circle) or `○` (empty circle)
**After:** `TX` text appears on all transmitting aircraft

**Reason:** More clear and professional indicator

### 2. Single TX Color Changed to Teal ✅
**Before:** White (`#FFFFFF`)
**After:** Teal (`#008080`)

**Reason:** Better visibility and distinction from other radar elements

### 3. Concurrent TX Still Red
**Unchanged:** Red (`#FF0000`) for multiple simultaneous transmissions

## What You'll See

### On Radar Tags:
- Aircraft tuned to your frequency show: **TX** in the label
- Single aircraft transmitting: **Teal track ring + TX label**
- Multiple aircraft transmitting: **Red track ring + TX label**

### Default Colors:
```json
{
  "SingleTxColor": "#008080",      // Teal
  "ConcurrentTxColor": "#FF0000"   // Red
}
```

## Files Updated

| File | Change |
|---|---|
| `RDFSettings.cs:13` | `Color.White` → `Color.Teal` |
| `RDFPlugin_Working.cs:198` | `"●"/"○"` → `"TX"` |
| `bin/Release/VatsysRDF.dll` | Rebuilt and deployed |

## Deployment

**Location:** `C:\Users\danie\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\VatsysRDF.dll`
**Size:** 17 KB
**Timestamp:** Feb 20, 2026 11:08 AM

## Next Steps

1. **Restart vatSys** to load the new DLL
2. **Verify TX appears** on aircraft labels when on your frequency
3. **Check teal color** appears on track rings for single TX

## Customization

To change colors after first run, edit `RDFSettings.json` in the Plugins folder:

```json
{
  "Enabled": true,
  "SingleTxColor": "#008080",      // Change to any hex color
  "ConcurrentTxColor": "#FF0000",  // Change to any hex color
  "ObserverMode": true
}
```

Delete the file and restart to reset to new defaults.

## Label Positioning

The TX symbol appears wherever you placed the `RDF_TX` item in your `Labels.xml`.

**Recommended placement** (top of label):
```xml
<Label Type="Normal" MaximumWidth="11" LabelRectangle="Default">
  <DataLine>
    <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick=""/>
    <Item Type="LABEL_ITEM_ACID" Colour="" .../>
    ...
  </DataLine>
```

This puts TX **above** the callsign on the first line of the label.

## Troubleshooting

### TX doesn't appear
- Check `Labels.xml` has `<LabelItem Name="RDF_TX">` definition
- Check `RDF_TX` is added to a `<Label Type="">` template
- Restart vatSys after XML changes

### Wrong color
- Delete `RDFSettings.json` in Plugins folder
- Restart vatSys to generate new defaults
- Or edit `RDFSettings.json` manually

### TX appears but no teal color
- Track color requires `SelectASDTrackColour()` support
- Check DebugView for errors
- Color may show on track symbol instead of label text (API limitation)
