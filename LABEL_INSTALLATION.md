# Installing RDF TX Symbol in vatSys Labels

## Overview

The RDF plugin displays a TX symbol (● or ○) on aircraft radar tags when they're tuned to your frequency. This guide shows how to add the RDF custom label item to your vatSys profile labels.

## Step 1: Copy the DLL to Plugins Folder

```
Copy VatsysRDF.dll to:
C:\Users\<YourName>\OneDrive\Documents\vatSys Files\Profiles\<ProfileName>\Plugins\
```

Also copy `Newtonsoft.Json.dll` to the same folder.

## Step 2: Add Custom Label Item Definition

### For ATOP Oakland Profile

Edit `C:\Users\<YourName>\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Labels.xml`

Add this line **after** the opening `<Labels>` tag (around line 2):

```xml
  <LabelItem Name="RDF_TX" Type="Custom" Description="RDF Transmission Indicator">
    <Plugin>VatsysRDF</Plugin>
  </LabelItem>
```

### For NAT Profile

Same process - edit your NAT profile's `Labels.xml` and add the same `<LabelItem>` definition.

## Step 3: Add RDF_TX to Your Label Templates

Now add the `RDF_TX` item to the label types you want it to appear on.

### Example: Add to "Normal" Label (Recommended)

Find the `<Label Type="Normal">` section and add RDF_TX to the first DataLine:

#### Before:
```xml
<Label Type="Normal" MaximumWidth="11" LabelRectangle="Default">
  <DataLine>
    <Item Type="LABEL_ITEM_ACID" Colour="" P1_Colour="" LeftClick="Label_Move" .../>
    <Item Type="LABEL_ITEM_EMERG" Colour="Emergency" .../>
  </DataLine>
```

#### After:
```xml
<Label Type="Normal" MaximumWidth="11" LabelRectangle="Default">
  <DataLine>
    <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick="" LeftPadding="0"/>
    <Item Type="LABEL_ITEM_ACID" Colour="" P1_Colour="" LeftClick="Label_Move" .../>
    <Item Type="LABEL_ITEM_EMERG" Colour="Emergency" .../>
  </DataLine>
```

### Example: Add to "Extended" Label

Find the `<Label Type="Extended">` section and add to the second DataLine (with ACID):

```xml
<DataLine>
  <Item Type="SELECT_VERT" />
  <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick=""/>
  <Item Type="AURORA_COMM_ICON" Colour="" .../>
  <Item Type="LABEL_ITEM_ACID" Colour="" .../>
  ...
</DataLine>
```

### Example: Add to "Quicktag" Label

```xml
<Label Type="Quicktag" MaximumWidth="11" LabelRectangle="Default">
  <DataLine>
    <Item Type="RDF_TX" Colour="" LeftClick="" MiddleClick="" RightClick=""/>
    <Item Type="LABEL_ITEM_CPROMPT" Colour="CFLHighlight" .../>
    ...
  </DataLine>
```

## Step 4: Restart vatSys

1. Close vatSys completely
2. Start vatSys
3. Load your profile
4. Connect to VATSIM or use observer mode

## Verification

When the plugin is working:

1. Check DebugView for: `RDF: Plugin initialized successfully`
2. Aircraft on your frequency should show:
   - `○` symbol for single transmission
   - `●` symbol for concurrent transmissions
   - Colored rings around the aircraft (zoom-responsive)
   - Colored track symbols

## Color Settings

Colors can be customized in `RDFSettings.json` (created in Plugins folder after first run):

```json
{
  "Enabled": true,
  "SingleTxColor": "#FFFFFF",      // White for single TX
  "ConcurrentTxColor": "#FF0000",  // Red for concurrent TX
  "CircleRadius": 20,
  "PenWidth": 2,
  "ObserverMode": true
}
```

## Troubleshooting

### Symbol doesn't appear
- Check Labels.xml syntax (must be valid XML)
- Verify `<LabelItem>` definition is present
- Ensure RDF_TX is added to the correct `<Label Type="">` section
- Check DebugView for errors

### Colors don't show
- Plugin uses custom colors - ensure ForeColourIdentity is not overridden
- Check RDFSettings.json color values are valid hex codes

### Works in one profile but not another
- Each profile has its own Labels.xml
- Must add RDF_TX to each profile separately

## Observer Mode

When `ObserverMode: true` and you have no frequencies assigned, the plugin will highlight ALL aircraft on VATSIM. Great for testing!

To disable: Set `"ObserverMode": false` in RDFSettings.json.
