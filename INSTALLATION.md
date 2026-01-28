# VATSYS RDF Plugin - Installation Guide

## Quick Start (3 Simple Steps)

### Step 1: Install the Plugin DLL Files

1. Download the latest release ZIP from GitHub
2. Extract the ZIP file
3. Copy **ONLY** these 2 files to your VATSYS Plugins folder:
   ```
   VatsysRDF.dll
   Newtonsoft.Json.dll
   ```

**Where is my Plugins folder?**
```
Documents\vatSys Files\[YourProfileName]\Plugins\
```

For example:
- `Documents\vatSys Files\NAT\Plugins\`
- `Documents\vatSys Files\Australia\Plugins\`

**That's it!** The plugin is now installed, but you won't see indicators yet...

---

### Step 2: Add RDF Indicators to Your Labels (REQUIRED!)

**IMPORTANT**: You **MUST** add the RDF indicator to your Labels.xml file, otherwise you won't see anything!

1. Go to your profile folder: `Documents\vatSys Files\[YourProfileName]\`
2. Open `Labels.xml` in a text editor (Notepad++ recommended)
3. Find the line that starts with:
   ```xml
   <Item Type="LABEL_ITEM_ACID"
   ```
4. **Add this line RIGHT ABOVE IT:**
   ```xml
   <Item Type="RDF_TX" Colour="IdentFlash" LeftClick="" MiddleClick="" RightClick=""/>
   ```

**Example - Before:**
```xml
<DataLine>
  <Item Type="LABEL_ITEM_ACID" Colour="" LeftClick="Label_ACID_Menu"/>
</DataLine>
```

**Example - After:**
```xml
<DataLine>
  <Item Type="RDF_TX" Colour="IdentFlash" LeftClick="" MiddleClick="" RightClick=""/>
  <Item Type="LABEL_ITEM_ACID" Colour="" LeftClick="Label_ACID_Menu"/>
</DataLine>
```

**Pro Tip:** Add it to ALL your label types so it shows on every aircraft:
- Find every `<Item Type="LABEL_ITEM_ACID"` line in your Labels.xml
- Add the RDF_TX line above each one

---

### Step 3: Restart VATSYS

1. Close VATSYS completely
2. Start VATSYS again
3. Connect to VATSIM
4. Watch for ○ and ● indicators on aircraft labels when they transmit!

---

## For Developers (Building from Source)

### Prerequisites
- Visual Studio 2017 or later
- .NET Framework 4.7.2 SDK
- VATSYS installed (for vatSys.exe reference)

### Build Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/[yourusername]/vatsysrdf.git
   cd vatsysrdf
   ```

2. **Update vatSys.exe reference:**
   - Open `VatsysRDF.csproj` in a text editor
   - Update this line with your VATSYS installation path:
   ```xml
   <Reference Include="vatSys">
     <HintPath>C:\Program Files (x86)\vatSys\vatSys.exe</HintPath>
   </Reference>
   ```

3. **Open in Visual Studio:**
   ```bash
   start VatsysRDF.sln
   ```

4. **Build:**
   - Select "Release" configuration
   - Build > Build Solution (or press Ctrl+Shift+B)

5. **Output:**
   - DLL will be in: `bin\Release\VatsysRDF.dll`

6. **Install:**
   - Copy `bin\Release\VatsysRDF.dll` to your VATSYS Plugins folder

---

## Configuration (Optional)

The plugin creates a `RDFSettings.json` file in your Plugins folder. You can edit this to customize behavior:

```json
{
  "Enabled": true,
  "ObserverMode": true,
  "RequireTxFrequency": false,
  "LowAltitudeFilter": 0,
  "SingleTxColor": "#FFFFFF",
  "ConcurrentTxColor": "#FF0000"
}
```

### Settings Explained

- **Enabled** (`true`/`false`) - Turn the plugin on or off
- **ObserverMode** (`true`/`false`) - When you have no frequencies assigned (like OBS positions), show RDF for ALL aircraft on all frequencies
- **RequireTxFrequency** (`true`/`false`) - Not used in current version
- **LowAltitudeFilter** (number) - Minimum altitude in feet (0 = show all altitudes)
- **SingleTxColor** - Color for single transmission
- **ConcurrentTxColor** - Color for multiple concurrent transmissions

---

## How It Works

### For Controller Positions
When you connect as a controller position (like EGGX_CTR, CZQX_CTR), the plugin:
1. Monitors your assigned frequencies (both VHF and HF)
2. Fetches VATSIM network data every 15 seconds
3. Finds which aircraft are tuned to YOUR frequencies
4. Shows ○ or ● indicators on those specific aircraft

### For Observer Positions
When you connect as an observer (like DK_OBS, any _OBS position), the plugin:
1. Detects you have no assigned frequencies
2. With `ObserverMode: true` (default), it shows indicators for **ALL** aircraft
3. You can see RDF indicators for every pilot on the network!
4. Great for training or monitoring

### The Indicators
- **○** (empty circle) - Single aircraft transmitting
- **●** (filled circle) - Multiple aircraft transmitting concurrently

---

## Troubleshooting

### "I don't see any indicators!"

**Check #1: Did you add RDF_TX to your Labels.xml?**
- This is **REQUIRED** - Step 2 is NOT optional!
- The plugin can't display anything without this line
- See Step 2 above for exact instructions

**Check #2: Is the plugin enabled?**
- Check `RDFSettings.json` in your Plugins folder
- Make sure `"Enabled": true`

**Check #3: Are you connected to VATSIM?**
- The plugin needs an active VATSIM network connection
- Wait 15 seconds after connecting for initial data load

**Check #4: Are there aircraft on your frequencies?**
- **Controller:** Aircraft must be on YOUR assigned frequencies
- **Observer:** Check that `ObserverMode` is `true` in RDFSettings.json

### "Plugin not loading"

**Check #1: Are both DLLs in the Plugins folder?**
- `VatsysRDF.dll` (17 KB) - REQUIRED
- `Newtonsoft.Json.dll` (700 KB) - REQUIRED
- Both files must be present!

**Check #2: Correct Plugins folder?**
- Must be: `Documents\vatSys Files\[YourProfile]\Plugins\`
- NOT in: `Program Files\vatSys\`

**Check #3: Check Windows Event Viewer**
- Open Event Viewer > Windows Logs > Application
- Look for errors from VATSYS

### "Only works for some aircraft"

**Issue:** You only added RDF_TX to one label type
**Fix:** Add the RDF_TX line to ALL label types in Labels.xml
- Search for every occurrence of `<Item Type="LABEL_ITEM_ACID"`
- Add the RDF_TX line above each one

### "Works as controller but not as observer"

**Issue:** ObserverMode is disabled
**Fix:** Edit `RDFSettings.json` and set `"ObserverMode": true`

---

## Uninstallation

To remove the plugin:
1. Delete `VatsysRDF.dll` from your Plugins folder
2. Remove the `<Item Type="RDF_TX".../>` lines from your Labels.xml
3. (Optional) Delete `RDFSettings.json` from your Plugins folder
4. Keep `Newtonsoft.Json.dll` if other plugins need it

---

## What's in the Release ZIP?

```
VatsysRDF-v1.0.0/
├── VatsysRDF.dll          ← Main plugin (REQUIRED - copy this!)
├── Newtonsoft.Json.dll    ← JSON library (REQUIRED - copy this!)
├── RDFSettings.json       ← Example config (auto-created, reference only)
├── Labels.xml             ← Example labels file (reference only, DON'T copy!)
├── README.md              ← Main documentation
└── INSTALLATION.txt       ← This guide
```

**IMPORTANT:**
- **DO copy:** The 2 DLL files to your Plugins folder
- **DON'T copy:** Labels.xml (modify your existing one instead!)
- **DON'T copy:** RDFSettings.json (it will be created automatically)

---

## Quick Checklist

Before reporting issues, verify:

- [ ] Copied `VatsysRDF.dll` to Plugins folder
- [ ] Copied `Newtonsoft.Json.dll` to Plugins folder
- [ ] Added `<Item Type="RDF_TX".../>` line to Labels.xml
- [ ] Added it to EVERY label type (Limited, Normal, Extended, etc.)
- [ ] Saved Labels.xml
- [ ] Restarted VATSYS completely
- [ ] Connected to VATSIM network
- [ ] Waited 15 seconds for data to load
- [ ] Checked RDFSettings.json has `"Enabled": true`
- [ ] If observer: Checked `"ObserverMode": true`

**Still having issues?** Open an issue on GitHub with:
- Your Labels.xml file (attach it)
- Your RDFSettings.json file (attach it)
- VATSYS version number
- Whether you're connecting as controller or observer
- What position code you're using (e.g., EGGX_CTR, DK_OBS)

---

## Adding to GitHub

When you're ready to push to your `vatsysrdf` repository:

```bash
# Configure git (if not already done)
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Create initial commit
git add .
git commit -m "Initial commit: VATSYS RDF Plugin"

# Add your GitHub repository
git remote add origin https://github.com/[yourusername]/vatsysrdf.git

# Push to GitHub
git branch -M main
git push -u origin main
```

---

## Troubleshooting Build Issues

**"vatSys.exe not found":**
- Update the HintPath in VatsysRDF.csproj to point to your VATSYS installation

**"Newtonsoft.Json not found":**
- Visual Studio should restore NuGet packages automatically
- If not, right-click solution > "Restore NuGet Packages"

**Build succeeds but plugin doesn't load:**
- Make sure you built in Release mode, not Debug
- Check that .NET Framework 4.7.2 is installed
- Use DebugView to see error messages

---

## Support

For issues, questions, or contributions:
- Open an issue on GitHub
- Check the README.md for detailed documentation
- Use DebugView to see plugin debug output
