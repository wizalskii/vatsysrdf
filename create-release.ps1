# Create GitHub Release v1.0.0
$releaseNotes = Get-Content "RELEASE_NOTES_v1.0.0.md" -Raw

$body = @{
    tag_name = "v1.0.0"
    name = "VATSYS RDF Plugin v1.0.0"
    body = $releaseNotes
    draft = $false
    prerelease = $false
} | ConvertTo-Json

# Note: This requires authentication
# Option 1: Install GitHub CLI: https://cli.github.com/
# Then run: gh release create v1.0.0 release/VatsysRDF-v1.0.0.zip --title "VATSYS RDF Plugin v1.0.0" --notes-file RELEASE_NOTES_v1.0.0.md

# Option 2: Create release manually on GitHub:
# 1. Go to https://github.com/wizalskii/vatsysrdf/releases/new
# 2. Tag: v1.0.0
# 3. Title: VATSYS RDF Plugin v1.0.0
# 4. Description: Copy content from RELEASE_NOTES_v1.0.0.md
# 5. Upload: release/VatsysRDF-v1.0.0.zip
# 6. Click "Publish release"

Write-Host "GitHub Release v1.0.0 is ready to be created!"
Write-Host ""
Write-Host "To create the release, you have two options:"
Write-Host ""
Write-Host "OPTION 1: Using GitHub Web Interface (Easiest)"
Write-Host "1. Go to: https://github.com/wizalskii/vatsysrdf/releases/new"
Write-Host "2. Tag: v1.0.0"
Write-Host "3. Title: VATSYS RDF Plugin v1.0.0"
Write-Host "4. Description: Copy content from RELEASE_NOTES_v1.0.0.md"
Write-Host "5. Upload file: release\VatsysRDF-v1.0.0.zip"
Write-Host "6. Click 'Publish release'"
Write-Host ""
Write-Host "OPTION 2: Using GitHub CLI"
Write-Host "1. Install GitHub CLI from: https://cli.github.com/"
Write-Host "2. Run: gh auth login"
Write-Host "3. Run: gh release create v1.0.0 release/VatsysRDF-v1.0.0.zip --title 'VATSYS RDF Plugin v1.0.0' --notes-file RELEASE_NOTES_v1.0.0.md"
