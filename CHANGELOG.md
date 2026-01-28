# Changelog

All notable changes to the VATSYS RDF Plugin will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned Features
- Configuration UI within VATSYS
- Transmission history/log window
- Sound alerts on transmission detection
- Multiple concurrent transmission counter display
- Aircraft proximity filtering

---

## [1.0.0] - 2026-01-28

### Added
- Initial release of VATSYS RDF Plugin
- **VATSIM Data Correlation**: Identifies specific transmitting aircraft using VATSIM public data feed
- **Audio Event Detection**: Monitors VATSYS Audio API for transmission events
- **Visual Indicators**:
  - Track color highlighting (white for single, red for concurrent transmissions)
  - Custom label item showing circle indicators (○ or ●)
- **VHF & HF Support**: Works with both VHF (118-137 MHz) and HF (2-30 MHz) frequencies
- **Thread-Safe Implementation**: Concurrent dictionaries for transmission tracking
- **Automatic Cleanup**: 3-second timeout for transmission indicators
- **VATSIM Data Feed**: 15-second update cycle fetching frequency-to-callsign mappings
- **Frequency Tolerance**: ±5 kHz tolerance for frequency rounding variations
- **Configurable Settings**: JSON configuration file for colors and behavior
- **No External Dependencies**: Works entirely with VATSYS internal AFV (no TrackAudio needed)

### Features
- Detects when aircraft transmit on monitored frequencies
- Correlates transmissions with VATSIM network data
- Highlights only aircraft actually on the transmitting frequency
- Supports custom label items via `RDF_TX` label template
- Provides colored track symbols for visual identification

### Documentation
- Comprehensive README with installation and usage instructions
- FEATURES.md explaining how the correlation system works
- INSTALLATION.md with detailed setup guide
- BUILD.md with complete build instructions
- RELEASE.md for creating releases
- In-code documentation and debug logging

### Technical Details
- Built on .NET Framework 4.7.2
- Implements VATSYS IPlugin interface
- Uses Newtonsoft.Json for settings and VATSIM data parsing
- HttpClient for asynchronous VATSIM data fetching
- Timer-based cleanup and update mechanisms

---

## Release Notes

### v1.0.0 - Initial Release

This is the first stable release of the VATSYS RDF Plugin. The plugin solves a fundamental limitation of the VATSYS Audio API by using creative correlation with VATSIM's public data feed.

**The Problem:**
VATSYS's Audio API only tells us WHEN a transmission occurs, not WHO is transmitting.

**Our Solution:**
1. Fetch VATSIM data feed every 15 seconds
2. Build frequency-to-callsign mappings
3. When transmission detected, query which aircraft are on that frequency
4. Match callsigns to visible tracks
5. Highlight only those specific aircraft

**Accuracy:**
- Very high accuracy when aircraft are visible on scope and in VATSIM data
- Handles frequency rounding with ±5 kHz tolerance
- Works for all VATSIM-connected pilots
- 15-second maximum lag for new aircraft/frequency changes

**Known Limitations:**
- If multiple aircraft are on the same frequency, all are highlighted (can't determine which specific one transmitted - this is fundamental to the approach)
- Requires internet connection for VATSIM data feed
- Only highlights aircraft visible on your radar scope

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 1.0.0 | 2026-01-28 | Initial release with VATSIM data correlation |

---

## Contributing

Found a bug or have a feature request? Please open an issue on GitHub:
https://github.com/wizalskii/vatsysrdf/issues

---

[Unreleased]: https://github.com/wizalskii/vatsysrdf/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/wizalskii/vatsysrdf/releases/tag/v1.0.0
