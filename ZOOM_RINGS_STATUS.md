# Zoom-Responsive Rings - Current Status

## What Was Implemented ✅

1. **TX Symbol on Radar Tags** - Working!
   - Shows `●` for concurrent transmissions
   - Shows `○` for single transmission
   - Requires adding `RDF_TX` to `Labels.xml` (see LABEL_INSTALLATION.md)

2. **Colored Track Rings** - Working!
   - `SelectASDTrackColour()` returns colored tracks
   - White for single TX, Red for concurrent TX
   - Uses `new CustomColour(R, G, B)` constructor

3. **VATSIM Data Correlation** - Working!
   - Fetches data every 15 seconds
   - Matches aircraft by frequency
   - Observer mode shows all aircraft

## What's Blocked ❌

### Zoom-Responsive Ring Overlay

**Files created:** `RDFPlugin_ZoomResponsive.cs`, `RDFOverlay_ZoomResponsive.cs`

**Status:** NOT COMPILING - API issues discovered

**Blockers:**
1. `MMI.FPASD` is type `System.Boolean` (not a window object as expected)
   - Need to find the actual ASD window/display class
   - Tried: checking MMI static properties, couldn't find window reference

2. `Coordinate` properties unknown
   - `Coordinate.LatitudeDegrees` doesn't exist
   - `Coordinate.LongitudeDegrees` doesn't exist
   - Need to find correct property names

3. `CustomLabelItem.ForeColour` doesn't exist
   - Can't set custom colors on label text
   - Track coloring works, but not label text coloring

## Zoom-Responsive Rings Concept

The idea was sound:
```csharp
// Get aircraft position
Coordinate centerCoord = fdr.GetLocation();

// Calculate position 5 NM away
double offsetDegrees = 5.0 / 60.0;  // 5 NM in degrees
Coordinate offsetCoord = new Coordinate(
    centerCoord.Lat + offsetDegrees,  // BLOCKED: Don't know property name
    centerCoord.Lon
);

// Convert both to screen pixels
Point centerPx = ASDWindow.GetPaintPoint(centerCoord);  // BLOCKED: Don't know window object
Point offsetPx = ASDWindow.GetPaintPoint(offsetCoord);

// Calculate pixel radius (scales with zoom!)
double radiusPx = Distance(centerPx, offsetPx);
```

## What's Needed to Unblock

### 1. Find ASD Window Object

Need to discover what property/method returns the actual ASD window that has `GetPaintPoint(Coordinate)`.

**Tried:**
- `MMI.FPASD` - returns boolean
- Searching MMI properties - couldn't find window reference

**Next steps:**
- Decompile `vatSys.exe` with ILSpy
- Search for classes with `GetPaintPoint` method
- Find how other plugins access the display/window

### 2. Find Coordinate Properties

Need to know property names for:
- Latitude (tried: `LatitudeDegrees`, failed)
- Longitude (tried: `LongitudeDegrees`, failed)

**Next steps:**
- Use ILSpy to inspect `vatsys.Coordinate` class
- Check constructor parameters
- Find property/field names

### 3. Alternative: Fixed-Size Rings

Could implement overlay with fixed pixel radius (no zoom responsiveness):
- Use `RDFOverlay.cs` (original version)
- Draws fixed 20px radius rings
- Doesn't scale with zoom, but still shows TX visually

**Pros:**
- Would work immediately
- Simple implementation

**Cons:**
- Rings too small when zoomed out
- Rings too large when zoomed in
- Not as professional as zoom-responsive

## Current Build (What's Deployed)

**File:** `bin/Release/VatsysRDF.dll` (17 KB, built Feb 20 11:04)
**Active source:** `RDFPlugin_Working.cs`
**Features working:**
- ✅ TX symbol on radar tags (`●`/`○`)
- ✅ Colored track rings (white/red)
- ✅ VATSIM data correlation
- ✅ Observer mode
- ❌ Zoom-responsive rings (blocked)

## Recommendations

### Option A: Ship Current Version
- Works great for TX indication
- Colored track rings look professional
- Missing zoom-responsive overlay rings, but core feature works

### Option B: Add Fixed-Size Rings
- Enable `RDFOverlay.cs` in build
- Draw fixed 20px rings
- Better than nothing, but not ideal

### Option C: Full API Investigation
- Use ILSpy/dnSpy to decompile vatSys.exe
- Find correct APIs for window access and coordinates
- Implement proper zoom-responsive rings
- Most professional solution, but requires API research

## Files in Repository

| File | Status | Purpose |
|---|---|---|
| `RDFPlugin_Working.cs` | ✅ ACTIVE | Current working build |
| `RDFPlugin_ZoomResponsive.cs` | ❌ Blocked | Future zoom-responsive version |
| `RDFOverlay_ZoomResponsive.cs` | ❌ Blocked | Overlay with zoom calculation |
| `RDFOverlay.cs` | 💤 Available | Fixed-size ring overlay |
| `LABEL_INSTALLATION.md` | ✅ Complete | How to add TX symbol to labels |
| `CLAUDE.md` | ✅ Updated | Full project documentation |
| `ZOOM_RINGS_STATUS.md` | ✅ This file | Zoom rings implementation status |

## Summary

**What works:** TX symbols on radar tags with colored track rings - looks great!

**What's missing:** Zoom-responsive ring overlay around aircraft.

**Why:** vatSys API for window access and coordinate properties needs investigation with ILSpy.

**Workaround:** Could add fixed-size rings, or ship current version (which already looks professional).
