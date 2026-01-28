# Quick Installation Guide

## For Users (Installing the Plugin)

### Prerequisites
- VATSYS installed
- A VATSYS profile set up

### Installation Steps

1. **Download** the plugin DLL:
   - Get `VatsysRDF.dll` from the releases page
   - Or build from source (see below)

2. **Locate your VATSYS profile folder:**
   ```
   Documents\vatSys Files\[YourProfileName]\
   ```

3. **Create Plugins folder** (if it doesn't exist):
   ```
   Documents\vatSys Files\[YourProfileName]\Plugins\
   ```

4. **Copy** `VatsysRDF.dll` into the Plugins folder

5. **(Optional) Add custom label:**
   - Copy `Labels.xml` to your profile folder, OR
   - Add this to your existing `Labels.xml`:
   ```xml
   <LabelItem Name="RDF_TX" Type="Custom" />
   ```

6. **Restart VATSYS**

7. **Test it:**
   - Connect to VATSIM
   - When aircraft transmit, you should see:
     - Track colors change (white for single TX, red for multiple)
     - If you added the label: circle indicators (○ or ●) next to callsigns

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

## Configuration

After first run, a `RDFSettings.json` file will be created in the Plugins folder:

```json
{
  "Enabled": true,
  "SingleTxColor": "#FFFFFF",
  "ConcurrentTxColor": "#FF0000",
  "RequireTxFrequency": false,
  "LowAltitudeFilter": 0
}
```

Edit this file to customize colors and behavior.

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
