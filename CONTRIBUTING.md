# Contributing to VATSYS RDF Plugin

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## How to Contribute

### Reporting Bugs

1. **Check existing issues** - Search [existing issues](https://github.com/wizalskii/vatsysrdf/issues) first
2. **Use the bug report template** - Click "New Issue" and select "Bug Report"
3. **Provide details** - Include:
   - Steps to reproduce
   - Expected vs actual behavior
   - VATSYS version
   - Plugin version
   - DebugView output (if available)
   - Configuration file

### Suggesting Features

1. **Check existing requests** - See if someone already suggested it
2. **Use the feature request template** - Select "Feature Request" when creating an issue
3. **Explain the use case** - Help us understand why this feature would be valuable

### Contributing Code

#### Getting Started

1. **Fork the repository**
   ```bash
   # Click "Fork" on GitHub
   git clone https://github.com/YOUR_USERNAME/vatsysrdf.git
   cd vatsysrdf
   ```

2. **Create a branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/your-bug-fix
   ```

3. **Set up development environment**
   - Install Visual Studio 2017 or later
   - Install VATSYS
   - See [BUILD.md](BUILD.md) for detailed setup

#### Coding Standards

- **C# Style**: Follow Microsoft C# coding conventions
- **Formatting**: Use 4 spaces for indentation (no tabs)
- **Naming**:
  - PascalCase for classes, methods, properties
  - camelCase for local variables
  - UPPER_CASE for constants
- **Comments**: Add XML documentation comments for public APIs
- **Error Handling**: Always log errors to Debug output with "RDF:" prefix

**Example:**
```csharp
/// <summary>
/// Gets all callsigns currently tuned to a specific frequency
/// </summary>
/// <param name="frequencyHz">Frequency in Hertz</param>
/// <returns>Set of callsigns on the frequency</returns>
public HashSet<string> GetCallsignsOnFrequency(uint frequencyHz)
{
    try
    {
        // Implementation
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"RDF: Error getting callsigns: {ex.Message}");
        return new HashSet<string>();
    }
}
```

#### Testing

- **Manual testing**: Test your changes in VATSYS on VATSIM network
- **Test scenarios**:
  - Single transmission
  - Multiple concurrent transmissions
  - VHF and HF frequencies
  - Frequency changes
  - Network connection loss
- **Debug output**: Verify appropriate debug messages are logged

#### Commit Messages

Use conventional commits format:

```
type(scope): subject

body (optional)

footer (optional)
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding tests
- `chore`: Build/tooling changes

**Examples:**
```bash
feat(correlation): add proximity filtering for transmitters

Add configuration option to only show transmitters within X nautical miles
of the controller position. Helps reduce clutter in busy airspace.
```

```bash
fix(audio): handle null frequency in event handler

Prevent NullReferenceException when ReceivingChanged event fires
with a null sender. Fixes issue #12.
```

#### Pull Requests

1. **Update your branch**
   ```bash
   git fetch origin
   git rebase origin/main
   ```

2. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

3. **Create Pull Request**
   - Go to GitHub and click "New Pull Request"
   - Fill in the PR template
   - Link related issues

4. **PR Template**
   ```markdown
   ## Description
   Brief description of the changes

   ## Type of Change
   - [ ] Bug fix
   - [ ] New feature
   - [ ] Breaking change
   - [ ] Documentation update

   ## Testing
   - [ ] Tested in VATSYS
   - [ ] Tested on VATSIM network
   - [ ] Tested VHF frequencies
   - [ ] Tested HF frequencies

   ## Checklist
   - [ ] Code follows project style guidelines
   - [ ] Self-review completed
   - [ ] Comments added for complex code
   - [ ] Documentation updated
   - [ ] No new warnings introduced

   ## Related Issues
   Fixes #(issue number)
   ```

5. **Code Review**
   - Respond to feedback
   - Make requested changes
   - Update PR as needed

### Documentation

- **README.md**: User-facing documentation
- **FEATURES.md**: Technical explanations
- **Code comments**: Explain *why*, not *what*
- **CHANGELOG.md**: Keep updated with changes

## Project Structure

```
vatsysrdf/
├── RDFPlugin.cs          # Main plugin entry point
├── VatsimDataFeed.cs     # VATSIM API integration
├── RDFSettings.cs        # Configuration management
├── TransmissionTracker.cs # Transmission state tracking
├── PositionConverter.cs   # Coordinate conversion
├── RDFOverlay.cs         # UI overlay (future use)
├── Properties/           # Assembly metadata
└── .github/             # GitHub templates
```

## Areas for Contribution

### High Priority
- [ ] Configuration UI within VATSYS
- [ ] Transmission history/log window
- [ ] Sound alerts on transmission
- [ ] Performance optimizations

### Medium Priority
- [ ] Multiple transmission counter display
- [ ] Proximity filtering
- [ ] Custom frequency filters
- [ ] Export transmission logs

### Low Priority
- [ ] Unit tests
- [ ] CI/CD pipeline
- [ ] Installer/updater
- [ ] Localization

## Questions?

- **GitHub Discussions**: Ask questions about contributing
- **Issues**: Report bugs or request features
- **Email**: Contact the maintainer

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to VATSYS RDF Plugin! 🎉
