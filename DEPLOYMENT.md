# VatsysRDF Plugin - Deployment Guide

## Required Files ✅

Only **2 files** are required to install the plugin:

### 1. `VatsysRDF.dll` (17 KB)
- **Location:** `bin/Release/VatsysRDF.dll`
- **Purpose:** The RDF plugin itself
- **Required:** YES

### 2. `Newtonsoft.Json.dll` (696 KB)
- **Location:** `bin/Release/Newtonsoft.Json.dll`
- **Purpose:** JSON library dependency (for reading VATSIM data feed)
- **Required:** YES

## Optional Files 📋

### `VatsysRDF.pdb` (30-42 KB)
- **Location:** `bin/Release/VatsysRDF.pdb`
- **Purpose:** Debug symbols for Visual Studio debugging
- **Required:** NO
- **Use case:** Only needed if you're debugging the plugin with a debugger
- **Note:** Safe to include, but not necessary for normal operation

## Installation Steps

### 1. Copy Required Files

Copy these 2 files to your vatSys Plugins folder:

```
Source: C:\Users\danie\claude\vatsysrdf\bin\Release\
Files:
  - VatsysRDF.dll
  - Newtonsoft.Json.dll

Destination:
  C:\Users\<YourName>\OneDrive\Documents\vatSys Files\Profiles\<ProfileName>\Plugins\
```

**Example for ATOP Oakland:**
```
C:\Users\danie\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\
  ├── VatsysRDF.dll
  └── Newtonsoft.Json.dll
```

### 2. Configure Labels.xml

See `LABEL_INSTALLATION.md` for complete instructions on adding the TX symbol to your radar tags.

Quick summary:
1. Edit `Profiles\<ProfileName>\Labels.xml`
2. Add `<LabelItem Name="RDF_TX" Type="Custom">` definition
3. Add `<Item Type="RDF_TX" .../>` to your label templates

### 3. Restart vatSys

The plugin will load automatically on vatSys startup.

## File Verification

After installation, your Plugins folder should look like this:

```
Plugins/
├── VatsysRDF.dll          ← 17 KB
├── Newtonsoft.Json.dll    ← 696 KB
└── [other plugins...]
```

**Optional (if debugging):**
```
Plugins/
├── VatsysRDF.dll          ← 17 KB
├── VatsysRDF.pdb          ← 30 KB (debug symbols)
├── Newtonsoft.Json.dll    ← 696 KB
└── [other plugins...]
```

## Newtonsoft.Json.dll - Important Note

If you already have `Newtonsoft.Json.dll` in your Plugins folder (from another plugin), you **do not need to copy it again**. vatSys will use the existing one.

**Version check:** This plugin was built with Newtonsoft.Json v13.0.3. If you have an older version, consider updating to 13.0.3 or newer.

## No .pbl File Required

**Note:** There is no `.pbl` file. If you see `VatsysRDF.pdb`, that's a **debug symbols file** (.pdb, not .pbl), which is optional.

## Download from GitHub

Get the latest DLLs from:
https://github.com/wizalskii/vatsysrdf

**Pre-built binaries:** `bin/Release/` folder in the repository

## Troubleshooting

### Plugin doesn't load
- Verify both DLL files are in the Plugins folder
- Check file sizes match (17 KB for VatsysRDF.dll, 696 KB for Newtonsoft.Json.dll)
- Use DebugView to see error messages
- Ensure you're using .NET Framework 4.7.2 or newer

### TX symbol doesn't appear
- This requires Labels.xml configuration
- See `LABEL_INSTALLATION.md` for complete instructions

### Wrong version
- Check file timestamp: should be Feb 20, 2026 or newer
- Latest version shows "TX" text (not ● symbols)
- Latest version uses teal color for single TX

## Quick Install Command (PowerShell)

```powershell
# From the vatsysrdf project directory
$dest = "$env:USERPROFILE\OneDrive\Documents\vatSys Files\Profiles\ATOP Oakland\Plugins\"
Copy-Item "bin\Release\VatsysRDF.dll" $dest -Force
Copy-Item "bin\Release\Newtonsoft.Json.dll" $dest -Force
```

## Summary

**Minimum required files:** 2
- `VatsysRDF.dll`
- `Newtonsoft.Json.dll`

**Optional files:** 1
- `VatsysRDF.pdb` (debug symbols only)

**No .pbl file exists or is needed.**
