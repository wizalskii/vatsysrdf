# Building VATSYS RDF Plugin

## Prerequisites

Before building the plugin, ensure you have:

1. **Visual Studio 2017 or later** (Community Edition is free)
   - Download: https://visualstudio.microsoft.com/downloads/
   - Required workload: ".NET desktop development"

2. **.NET Framework 4.7.2 SDK**
   - Usually installed with Visual Studio
   - Standalone download: https://dotnet.microsoft.com/download/dotnet-framework/net472

3. **VATSYS installed**
   - Download: https://virtualairtrafficsystem.com/
   - Required for the vatSys.exe reference

---

## Build Instructions

### Option 1: Build with Visual Studio (Recommended)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/wizalskii/vatsysrdf.git
   cd vatsysrdf
   ```

2. **Update VATSYS reference path:**
   - Open `VatsysRDF.csproj` in a text editor
   - Find the line with `<Reference Include="vatSys">`
   - Update the `<HintPath>` to match your VATSYS installation:
     ```xml
     <Reference Include="vatSys">
       <HintPath>C:\Program Files (x86)\vatSys\vatSys.exe</HintPath>
     </Reference>
     ```

3. **Open solution in Visual Studio:**
   ```bash
   start VatsysRDF.sln
   ```
   Or double-click `VatsysRDF.sln`

4. **Restore NuGet packages:**
   - Visual Studio will automatically prompt to restore packages
   - Or right-click solution → "Restore NuGet Packages"

5. **Build:**
   - Select **Release** configuration (not Debug)
   - Press `Ctrl+Shift+B` or go to Build → Build Solution

6. **Output:**
   - Built DLL location: `bin\Release\VatsysRDF.dll`

---

### Option 2: Build with MSBuild (Command Line)

1. **Open Developer Command Prompt for VS:**
   - Start Menu → Visual Studio → Developer Command Prompt

2. **Navigate to project:**
   ```cmd
   cd path\to\vatsysrdf
   ```

3. **Restore NuGet packages:**
   ```cmd
   nuget restore VatsysRDF.sln
   ```

4. **Build:**
   ```cmd
   msbuild VatsysRDF.sln /p:Configuration=Release
   ```

5. **Output:**
   - Built DLL: `bin\Release\VatsysRDF.dll`

---

## Post-Build

### Install the Plugin

1. Copy `bin\Release\VatsysRDF.dll` to your VATSYS Plugins folder:
   ```
   Documents\vatSys Files\[YourProfile]\Plugins\
   ```

2. (Optional) Copy `Labels.xml` to your profile folder

3. Restart VATSYS

---

## Troubleshooting

### "vatSys.exe not found" error

**Problem:** The reference to vatSys.exe cannot be found.

**Solution:**
1. Find your VATSYS installation path
2. Update `VatsysRDF.csproj`:
   ```xml
   <HintPath>YOUR_PATH_HERE\vatSys.exe</HintPath>
   ```

Common locations:
- `C:\Program Files (x86)\vatSys\vatSys.exe`
- `C:\Program Files\vatSys\vatSys.exe`

---

### "Newtonsoft.Json not found" error

**Problem:** NuGet package not restored.

**Solution:**
- Visual Studio: Right-click solution → "Restore NuGet Packages"
- Command line: `nuget restore VatsysRDF.sln`

---

### Build succeeds but plugin doesn't load

**Checklist:**
- [ ] Built in **Release** mode (not Debug)
- [ ] DLL placed in correct Plugins folder
- [ ] VATSYS restarted after copying DLL
- [ ] .NET Framework 4.7.2 installed on your system

**Debug:**
- Use [DebugView](https://learn.microsoft.com/en-us/sysinternals/downloads/debugview)
- Look for lines starting with "RDF:"
- Check for initialization errors

---

## Creating a Release

After building:

1. **Create release folder:**
   ```
   VatsysRDF-v1.0/
   ├── VatsysRDF.dll
   ├── Labels.xml
   ├── README.md
   └── RDFSettings.json (example)
   ```

2. **Zip the folder:**
   ```bash
   VatsysRDF-v1.0.zip
   ```

3. **Upload to GitHub Releases:**
   - Go to: https://github.com/wizalskii/vatsysrdf/releases
   - Click "Create a new release"
   - Tag version: `v1.0.0`
   - Attach `VatsysRDF-v1.0.zip`
   - Add release notes

---

## Development Build vs Release Build

| Build Type | Configuration | Output | Use Case |
|------------|---------------|--------|----------|
| **Development** | Debug | `bin\Debug\` | Testing, debugging |
| **Release** | Release | `bin\Release\` | Distribution, production |

**Always use Release builds for distribution!**

---

## Continuous Integration (Future)

For automated builds, consider setting up GitHub Actions:

```yaml
# .github/workflows/build.yml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
      - name: Restore dependencies
        run: nuget restore
      - name: Build
        run: msbuild /p:Configuration=Release
```

---

## Next Steps

After successful build:
1. Test the plugin in VATSYS
2. Create a GitHub release with the DLL
3. Share with the community!
