# Deployment

## Deployment Targets

| Target | Method | Status |
|--------|--------|--------|
| Linux (x64) | Self-contained publish | ✅ Supported |
| Windows (x64) | Self-contained publish | ✅ Supported |
| macOS (x64) | Self-contained publish | ✅ Supported |
| Linux (ARM64) | Self-contained publish | ⚠️ Untested |
| Steam | Steamworks SDK | ❌ Not implemented |
| Microsoft Store | MSIX packaging | ❌ Not implemented |

## Self-Contained Deployment

### Linux x64

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r linux-x64 --self-contained true
```

**Output:** `Minesweeper/bin/Release/net10.0/linux-x64/publish/`

**Contents:**
- `Minesweeper` — Executable
- `Minesweeper.dll` — Application assembly
- `*.dll` — All dependencies including .NET runtime
- `Content/` — Compiled content (copied via build)

**Distribution:**
```bash
tar -czf minesweeper-linux-x64.tar.gz -C Minesweeper/bin/Release/net10.0/linux-x64/publish .
```

### Windows x64

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r win-x64 --self-contained true
```

**Output:** `Minesweeper/bin/Release/net10.0/win-x64/publish/`

**Contents:**
- `Minesweeper.exe` — Executable
- `Minesweeper.dll` — Application assembly
- `*.dll` — All dependencies including .NET runtime
- `Content/` — Compiled content

**Distribution:** Zip the publish folder

### macOS x64

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r osx-x64 --self-contained true
```

**Output:** `Minesweeper/bin/Release/net10.0/osx-x64/publish/`

**Note:** Requires code signing for distribution outside App Store.

## Framework-Dependent Deployment

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release
```

**Output:** `Minesweeper/bin/Release/net10.0/publish/`

**Requirements:** Target machine must have .NET 10.0 Desktop Runtime installed.

**Size:** ~5 MB vs ~150 MB for self-contained.

## Content Deployment

Content files are automatically copied to publish output via `MonoGameContentReference` in `.csproj`.

**Verify Content in Publish:**
```bash
ls Minesweeper/bin/Release/net10.0/linux-x64/publish/Content/
# Should contain: Fonts/, ScreenManager/, SplashScreen/, Tiles/
```

## Runtime Dependencies by Platform

### Linux

| Dependency | Package | Required For |
|------------|---------|--------------|
| `libsdl2-2.0-0` | `libsdl2-2.0-0` | Window/input |
| `libopenal1` | `libopenal1` | Audio |
| `libfreetype6` | `libfreetype6` | Fonts |
| `ttf-mscorefonts-installer` | `ttf-mscorefonts-installer` | Default fonts |

**Install Script:**
```bash
#!/bin/bash
set -euo pipefail
sudo apt update
sudo apt install -y libsdl2-2.0-0 libopenal1 libfreetype6 ttf-mscorefonts-installer
```

### Windows

- All dependencies bundled in self-contained publish
- No additional installation required

### macOS

| Dependency | Install |
|------------|---------|
| SDL2 | `brew install sdl2` |

## User Data Directory

Application creates/uses:
- Windows: `%LocalAppData%\Minesweeper\`
- Linux: `~/.local/share/Minesweeper/`
- macOS: `~/Library/Application Support/Minesweeper/`

**Contents:**
- `Settings.xml` — User preferences
- `Logs/` — Log files (if logging implemented)

**Permissions:** Application must have read/write access.

## Single-File Deployment (Experimental)

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

**Caveats:**
- Content files must be extracted at runtime (slower startup)
- May not work with MonoGame content loading
- Test thoroughly before use

## Trimmed Deployment (Experimental)

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r linux-x64 --self-contained true -p:PublishTrimmed=true
```

**Caveats:**
- Reflection-based code (NuciDAL XML serialisation) may break
- Requires thorough testing
- Not recommended for this project

## Versioning

**Assembly Version:** Set in `Minesweeper.csproj`
```xml
<Version>1.0.0</Version>
```

**Semantic Versioning:** Major.Minor.Patch
- Major: Breaking changes
- Minor: New features
- Patch: Bug fixes

## Release Checklist

- [ ] Update version in `.csproj`
- [ ] Update `CHANGELOG.md` (if exists)
- [ ] Build Release configuration
- [ ] Run all tests
- [ ] Test self-contained publish on target platforms
- [ ] Verify content loads correctly
- [ ] Verify settings persist
- [ ] Create distribution archives
- [ ] Tag release in Git

## Known Deployment Issues

1. **Content Path:** `GameWindow` expects `Content/` relative to executable. Ensure publish preserves folder structure.

2. **Font Rendering:** Linux requires `mscorefonts` for correct SpriteFont rendering.

3. **High DPI:** Not tested; may need `SetProcessDpiAwareness` on Windows.

4. **Multiple Instances:** No mutex; multiple instances can run simultaneously.