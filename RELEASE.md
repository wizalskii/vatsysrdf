# Creating a Release

This guide explains how to create and publish a release of the VATSYS RDF Plugin.

## Pre-Release Checklist

- [ ] Code is tested and working in VATSYS
- [ ] All tests pass (if applicable)
- [ ] Documentation is up to date
- [ ] Version number is updated in `AssemblyInfo.cs`
- [ ] CHANGELOG is updated
- [ ] Built in **Release** configuration

---

## Version Numbering

We use [Semantic Versioning](https://semver.org/): `MAJOR.MINOR.PATCH`

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes

Example: `1.0.0` → `1.1.0` (new feature) → `1.1.1` (bug fix)

---

## Build the Release

1. **Update version** in `Properties/AssemblyInfo.cs`:
   ```csharp
   [assembly: AssemblyVersion("1.0.0.0")]
   [assembly: AssemblyFileVersion("1.0.0.0")]
   ```

2. **Build in Release mode:**
   ```cmd
   msbuild VatsysRDF.sln /p:Configuration=Release
   ```

3. **Verify output:**
   - Check `bin\Release\VatsysRDF.dll` exists
   - Right-click DLL → Properties → Details
   - Verify version number is correct

---

## Create Release Package

1. **Create release folder:**
   ```
   VatsysRDF-v1.0.0/
   ├── VatsysRDF.dll          # The plugin binary
   ├── Labels.xml             # Custom label definition
   ├── README.md              # Installation instructions
   └── RDFSettings.json       # Example configuration
   ```

2. **Create example settings file:**
   ```json
   {
     "Enabled": true,
     "SingleTxColor": "#FFFFFF",
     "ConcurrentTxColor": "#FF0000",
     "RequireTxFrequency": false,
     "LowAltitudeFilter": 0
   }
   ```
   Save as `RDFSettings.json` in release folder.

3. **Copy files:**
   ```cmd
   mkdir VatsysRDF-v1.0.0
   copy bin\Release\VatsysRDF.dll VatsysRDF-v1.0.0\
   copy Labels.xml VatsysRDF-v1.0.0\
   copy README.md VatsysRDF-v1.0.0\
   echo {"Enabled":true,"SingleTxColor":"#FFFFFF","ConcurrentTxColor":"#FF0000","RequireTxFrequency":false,"LowAltitudeFilter":0} > VatsysRDF-v1.0.0\RDFSettings.json
   ```

4. **Create ZIP archive:**
   - Right-click folder → Send to → Compressed (zipped) folder
   - Or use command line:
     ```cmd
     powershell Compress-Archive -Path VatsysRDF-v1.0.0 -DestinationPath VatsysRDF-v1.0.0.zip
     ```

---

## Publish to GitHub

1. **Go to Releases page:**
   - Navigate to: https://github.com/wizalskii/vatsysrdf/releases
   - Click "Draft a new release"

2. **Tag the release:**
   - Tag version: `v1.0.0`
   - Target: `main`

3. **Release title:**
   ```
   VATSYS RDF Plugin v1.0.0
   ```

4. **Description template:**
   ```markdown
   ## VATSYS RDF Plugin v1.0.0

   Radio Direction Finder plugin for VATSYS that identifies specific transmitting aircraft using VATSIM data correlation.

   ### 🎯 Key Features
   - ✅ Identifies SPECIFIC transmitting aircraft (not all aircraft!)
   - ✅ Works with both VHF and HF frequencies
   - ✅ No TrackAudio required - uses VATSYS internal AFV
   - ✅ High accuracy using official VATSIM data feed
   - ✅ Visual indicators via track colors and custom labels

   ### 📥 Installation

   1. Download `VatsysRDF-v1.0.0.zip` below
   2. Extract to your VATSYS profile's `Plugins` folder:
      - Path: `Documents\vatSys Files\[YourProfile]\Plugins\`
   3. (Optional) Copy `Labels.xml` to your profile folder
   4. Restart VATSYS

   ### 📖 Documentation

   - [README](https://github.com/wizalskii/vatsysrdf#readme) - Full documentation
   - [FEATURES](https://github.com/wizalskii/vatsysrdf/blob/main/FEATURES.md) - How it works
   - [INSTALLATION](https://github.com/wizalskii/vatsysrdf/blob/main/INSTALLATION.md) - Detailed install guide

   ### 🐛 Known Issues

   - None currently

   ### 📝 Changelog

   #### New Features
   - Initial release
   - VATSIM data correlation for specific aircraft identification
   - Track color highlighting (white/red)
   - Custom label indicators
   - Support for VHF and HF frequencies

   ### 🙏 Credits

   Inspired by the EuroScope RDF plugin by [KingfuChan](https://github.com/KingfuChan/RDF).
   ```

5. **Attach files:**
   - Drag and drop `VatsysRDF-v1.0.0.zip` to the attachments area

6. **Pre-release checkbox:**
   - Leave unchecked for stable releases
   - Check for beta/alpha releases

7. **Publish:**
   - Click "Publish release"

---

## Post-Release

1. **Announce:**
   - VATSIM forums
   - VATSYS Discord
   - Social media (if applicable)

2. **Monitor:**
   - Check GitHub Issues for bug reports
   - Respond to user feedback
   - Plan next release based on feedback

---

## Release Checklist Summary

```
Before Release:
[ ] Code tested in VATSYS
[ ] Version updated in AssemblyInfo.cs
[ ] Built in Release configuration
[ ] Release package created and zipped

During Release:
[ ] GitHub release created
[ ] Tag version set (v1.0.0)
[ ] Release notes written
[ ] ZIP file attached
[ ] Published

After Release:
[ ] Announcement posted
[ ] Issues monitored
[ ] Next release planned
```

---

## Automated Releases (Future)

Consider automating with GitHub Actions:

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Build
        run: msbuild /p:Configuration=Release
      - name: Create Release
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: ${{ github.ref }}
          release_name: Release ${{ github.ref }}
          draft: false
          prerelease: false
      - name: Upload Release Asset
        uses: actions/upload-release-asset@v1
        with:
          upload_url: ${{ steps.create_release.outputs.upload_url }}
          asset_path: ./bin/Release/VatsysRDF.dll
          asset_name: VatsysRDF.dll
          asset_content_type: application/octet-stream
```

---

## Questions?

Open an issue on GitHub: https://github.com/wizalskii/vatsysrdf/issues
