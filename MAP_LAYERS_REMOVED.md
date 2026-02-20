# RDF Maps/Layers - REMOVED (Not Required)

## Your Question: Is the Maps feature required?

**Answer: NO** ❌ - Map layers are **NOT required** and have been **completely removed** from the current version.

## What Happened Previously

Based on your observation that "RDF rings were available in Maps that could be selected" in a previous version:

### Previous Implementation (Your Friend's Fork)
- Attempted to add RDF rings as a **selectable map layer** in vatSys
- Used `DisplayMaps.Maps.Add()` to inject custom map overlays
- **Result:** Crashed vatSys or didn't work properly (especially with Mexico profile)

### Known API Issue
From your friend's documentation:
```
DisplayMaps.Maps.Add() crashes vatSys — do NOT inject dynamic maps from plugins
```

This is a **known limitation** of the vatSys plugin API - custom map layers cannot be reliably added.

## Current Implementation (What's Deployed)

The **current working version** does NOT use map layers at all.

### What Works Now ✅
1. **TX Text on Radar Tags** - Shows "TX" directly on the aircraft label
2. **Colored Track Rings** - Changes aircraft track symbol color (teal/red)
3. **No separate map layer needed**

### What Was Removed ❌
1. **Custom map layers** - No `DisplayMaps.Maps.Add()` calls
2. **Map selection UI** - No entries in the Maps menu
3. **Overlay windows** - `RDFOverlay.cs` is NOT compiled in current build

## Why This Approach Is Better

### Old Approach (Map Layers)
- ❌ Crashed vatSys
- ❌ Profile-specific issues (Mexico profile didn't work)
- ❌ Required user to manually enable in Maps menu
- ❌ Not supported by vatSys plugin API

### New Approach (Label Items + Track Colors)
- ✅ Stable - uses official plugin API
- ✅ Works with all profiles (ATOP, NAT, Mexico, etc.)
- ✅ Always visible (no manual map selection needed)
- ✅ Native integration with radar display

## What You Should See

When properly configured with `Labels.xml`:

```
Aircraft on your frequency:
├── "TX" text appears on radar tag (where you placed RDF_TX in Labels.xml)
├── Track symbol colored teal (single TX) or red (concurrent)
└── No separate map layer to enable/disable
```

## Installation Confirmation

**Required files in Plugins folder:**
```
Plugins/
├── VatsysRDF.dll          (17 KB)
└── Newtonsoft.Json.dll    (696 KB)
```

**NOT required:**
- ❌ No map XML files
- ❌ No separate map layer configuration
- ❌ No "RDF Rings" in Maps menu
- ❌ No overlay windows

## If You See Map-Related Issues

If the Mexico profile (or any profile) is having issues with RDF:

### Common Cause
You may still have the **old version** with map layer code installed.

### Solution
1. **Delete old files:**
   ```
   Delete: VatsysRDF.dll (old version)
   Delete: VatsysRDF.pdb (old version)
   Delete: Any RDF-related .xml files in Maps/
   ```

2. **Install new version:**
   ```
   Copy: VatsysRDF.dll (17 KB, Feb 20 2026)
   Copy: Newtonsoft.Json.dll (696 KB)
   ```

3. **Configure Labels.xml** (see LABEL_INSTALLATION.md)

4. **Restart vatSys**

## Technical Explanation

### Why Map Layers Don't Work

```csharp
// This was attempted in previous version:
DisplayMaps.Maps.Add(new CustomMap(...));  // ❌ CRASHES vatSys

// Current version uses:
public CustomLabelItem GetCustomLabelItem(...)  // ✅ Stable API
public CustomColour SelectASDTrackColour(...)   // ✅ Stable API
```

The vatSys plugin API provides official methods for:
- Custom label items (`IPlugin.GetCustomLabelItem()`)
- Custom track colors (`IPlugin.SelectASDTrackColour()`)

But does NOT officially support:
- Custom map layers (`DisplayMaps.Maps.Add()` - internal API only)

## Summary

**Maps feature status:** REMOVED - Not required ✅

**Why removed:** Caused crashes and profile incompatibility ❌

**Current approach:** TX text on labels + colored tracks ✅

**Works with all profiles:** Yes, including Mexico profile ✅

**Installation:** Just 2 DLL files, no map configuration needed ✅

---

## If You Still Want Visual Rings

If you specifically want visual rings around aircraft (beyond colored track symbols):

### Option 1: Fixed-Size Overlay (Quick Solution)
- Enable `RDFOverlay.cs` in build
- Draws fixed 20px rings
- Won't crash, but won't use Maps system
- Not zoom-responsive

### Option 2: Zoom-Responsive Overlay (Future Feature)
- `RDFPlugin_ZoomResponsive.cs` + `RDFOverlay_ZoomResponsive.cs`
- Blocked on vatSys API research (see ZOOM_RINGS_STATUS.md)
- Would use transparent overlay window (not Maps system)
- Rings scale with zoom level

**Note:** Both overlay options would be **separate from the Maps system** to avoid crashes. They would draw on a transparent overlay window, not as a selectable map layer.
