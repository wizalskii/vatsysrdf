# Pre-Built DLL Notice

## Current Status

**A pre-built DLL is not yet available in this repository.**

To use the plugin, you'll need to build it from source. See [BUILD.md](BUILD.md) for complete instructions.

---

## Why No Pre-Built DLL?

The DLL requires:
- Visual Studio or MSBuild to compile
- Reference to `vatSys.exe` from your VATSYS installation
- The exact path to vatSys.exe varies by installation

Since the plugin references VATSYS directly, we cannot provide a universal pre-built binary.

---

## Building the DLL (Quick Guide)

### Prerequisites
1. Install [Visual Studio](https://visualstudio.microsoft.com/downloads/) (Community Edition is free)
2. Install [VATSYS](https://virtualairtrafficsystem.com/)

### Steps
1. Clone this repository
2. Open `VatsysRDF.sln` in Visual Studio
3. Update the vatSys.exe reference path in `VatsysRDF.csproj`
4. Build in **Release** configuration
5. Find the DLL in `bin\Release\VatsysRDF.dll`

**Full instructions:** See [BUILD.md](BUILD.md)

---

## Future Plans

We plan to provide pre-built DLLs in GitHub Releases once:
- [ ] Automated build pipeline is set up (GitHub Actions)
- [ ] Version 1.0.0 is finalized and tested
- [ ] Community feedback is incorporated

---

## Need Help Building?

If you encounter issues building the plugin:
1. Check [BUILD.md](BUILD.md) for troubleshooting
2. Open an [issue](https://github.com/wizalskii/vatsysrdf/issues) with:
   - Error messages
   - Visual Studio version
   - VATSYS installation path

---

## Community Builds

If you successfully build the plugin, consider:
- Sharing your build process
- Contributing improvements to build documentation
- Helping other users with build issues

---

## Timeline

**Estimated availability of pre-built DLL:** After initial testing and feedback from early adopters.

**Want to help?** We welcome contributions to set up automated builds! See [CONTRIBUTING.md](CONTRIBUTING.md).
