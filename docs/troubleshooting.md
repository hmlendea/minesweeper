# Troubleshooting

Common issues, diagnostics, and resolutions for the Minesweeper application.

## Build Issues

### "MGCB tool not found" / Content Build Fails
**Symptoms:**
```
error: Could not execute because the specified command or file was not found.
Possible reasons for this include:
  * You misspelled a built-in dotnet command.
  * You intended to execute a .NET program, but dotnet-mgcb is not installed.
```

**Cause:** MonoGame Content Builder tool not installed.

**Resolution:**
```bash
dotnet tool install --global dotnet-mgcb
# Or for local project:
dotnet tool install --local dotnet-mgcb
```

### "Microsoft.Build.Utilities.Core" Missing on Linux
**Symptoms:**
```
error MSB4062: The "MGCB" task could not be loaded from the assembly...
Could not load file or assembly 'Microsoft.Build.Utilities.Core'
```

**Cause:** Missing MSBuild dependencies on Linux.

**Resolution:**
```bash
# Ubuntu/Debian
sudo apt-get install msbuild

# Or install mono-msbuild
sudo apt-get install mono-msbuild
```

### NuGet Package Restore Fails
**Symptoms:**
```
error NU1301: Unable to load the service index for source...
error NU1101: Unable to find package NuciXNA.Gui
```

**Cause:** Network issues, missing NuGet sources, or package version unavailable.

**Resolution:**
```bash
# Clear cache
dotnet nuget locals all --clear

# Restore with explicit source
dotnet restore Minesweeper.slnx --source https://api.nuget.org/v3/index.json

# Check package versions exist
dotnet search NuciXNA.Gui --source https://api.nuget.org/v3/index.json
```

### Target Framework Not Installed (.NET 10.0)
**Symptoms:**
```
error NETSDK1045: The current .NET SDK does not support targeting .NET 10.0.
```

**Cause:** .NET 10.0 SDK not installed.

**Resolution:**
```bash
# Check installed versions
dotnet --list-sdks

# Install .NET 10.0 (when available)
# For preview: download from https://dotnet.microsoft.com/download/dotnet/10.0
```

## Runtime Issues

### Game Window Doesn't Appear / Crashes on Startup
**Symptoms:** Application exits immediately or shows no window.

**Common Causes:**
1. **Missing GPU drivers / OpenGL support**
2. **Display server issues (Wayland vs X11)**
3. **Missing SDL2/OpenAL libraries**

**Diagnostics:**
```bash
# Check OpenGL
glxinfo | grep "OpenGL renderer"

# Check SDL2
ldd $(which dotnet) | grep -i sdl

# Run with verbose output
dotnet run --project Minesweeper/Minesweeper.csproj --verbosity detailed
```

**Resolutions:**
```bash
# Ubuntu: Install graphics drivers
sudo apt-get install mesa-vulkan-drivers libgl1-mesa-dri

# For Wayland: Force X11
export GDK_BACKEND=x11
dotnet run --project Minesweeper/Minesweeper.csproj

# Install SDL2/OpenAL
sudo apt-get install libsdl2-2.0-0 libopenal1
```

### "DllNotFoundException: SDL2" or "DllNotFoundException: OpenAL"
**Symptoms:**
```
System.DllNotFoundException: Unable to load shared library 'SDL2'
System.DllNotFoundException: Unable to load shared library 'openal32'
```

**Cause:** Native libraries not installed.

**Resolution:**
```bash
# Ubuntu/Debian
sudo apt-get install libsdl2-2.0-0 libopenal1

# Fedora
sudo dnf install SDL2 openal-soft

# Arch
sudo pacman -S sdl2 openal
```

### Content Files Not Found (Missing .xnb)
**Symptoms:**
```
Microsoft.Xna.Framework.Content.ContentLoadException: Could not find Content/Fonts/DefaultFont.xnb
```

**Cause:** Content not built or wrong output path.

**Resolution:**
```bash
# Build content explicitly
cd Minesweeper/Content
dotnet mgcb Content.mgcb /outputDir:bin/DesktopGL/Content /platform:DesktopGL /profile:Reach

# Or build full solution (includes content)
dotnet build Minesweeper.slnx
```

### Settings.xml Corrupt / Cannot Load
**Symptoms:**
```
NuciDAL.Exceptions.XmlFileObjectException: Error deserializing Settings.xml
System.InvalidOperationException: There is an error in XML document
```

**Cause:** Corrupted settings file, version mismatch, or permission issues.

**Resolution:**
```bash
# Delete settings file (recreates with defaults)
rm ~/.local/share/Minesweeper/Settings.xml
# Windows: %LocalAppData%\Minesweeper\Settings.xml

# Check permissions
ls -la ~/.local/share/Minesweeper/
```

### Timer Stays at 00:00
**Symptoms:** Game timer never increments during gameplay.

**Cause:** `GameManager.Update(gameTime)` never called (known bug).

**Location:** `Minesweeper/Gui/Screens/GameplayScreen.cs`

**Fix:**
```csharp
// In GameplayScreen.Update()
public override void Update(GameTime gameTime)
{
    base.Update(gameTime);

    // ADD THIS LINE:
    _gameManager.Update(gameTime);

    // ... rest of method
}
```

### Fullscreen Toggle Doesn't Work
**Symptoms:** Checking fullscreen in settings has no effect.

**Cause:** `GraphicsSettings.Resolution` not updated; `ApplyChanges()` not called correctly.

**Fix:**
```csharp
// In SettingsManager.ApplyGraphicsSettings()
public void ApplyGraphicsSettings()
{
    var graphics = GraphicsManager.Instance.Graphics;
    graphics.IsFullScreen = GraphicsSettings.Fullscreen;

    if (GraphicsSettings.Fullscreen)
    {
        // Use current display mode for fullscreen
        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    }
    else
    {
        graphics.PreferredBackBufferWidth = GameDefines.WindowWidth;
        graphics.PreferredBackBufferHeight = GameDefines.WindowHeight;
    }

    graphics.ApplyChanges();

    // ALSO UPDATE RESOLUTION SETTING:
    GraphicsSettings.Resolution = new Size2D(
        graphics.PreferredBackBufferWidth,
        graphics.PreferredBackBufferHeight
    );
}
```

### Mouse Clicks Offset / Wrong Tile Selected
**Symptoms:** Clicking a tile affects the wrong tile (usually one row off).

**Cause:** Not accounting for InfoBar height (60px) in coordinate conversion.

**Fix:**
```csharp
// In GameplayScreen.Update() - mouse handling
var mousePos = InputManager.Instance.MousePosition;
// SUBTRACT INFOBAR HEIGHT:
int adjustedY = (int)mousePos.Y - GameDefines.InfoBarHeight;
int tileX = (int)mousePos.X / GameDefines.MapTileSize;
int tileY = adjustedY / GameDefines.MapTileSize;
```

## Test Issues

### Tests Fail: "Could not load file or assembly 'MonoGame.Framework'"
**Symptoms:** Unit tests fail to load MonoGame types.

**Cause:** Test project missing MonoGame reference or content not built.

**Resolution:**
```bash
# Add MonoGame to test project (if needed for integration tests)
dotnet add Minesweeper.UnitTests/Minesweeper.UnitTests.csproj package MonoGame.Framework.DesktopGL

# Or mock MonoGame dependencies in unit tests
```

### Tests Fail: Hardcoded 16×16 Expectations
**Symptoms:** Tests fail after changing `GameDefines.TableSize`.

**Cause:** Tests use literal values instead of constants.

**Fix:**
```csharp
// In tests, use:
Assert.That(_gameManager.TableSize, Is.EqualTo(GameDefines.TableSize));
// Instead of:
Assert.That(_gameManager.TableSize, Is.EqualTo(16));
```

## Performance Issues

### Low Frame Rate / Stuttering
**Diagnostics:**
- Enable `DebugMode` in settings
- Check `FpsIndicator` in top-right
- Profile with `dotnet-trace` or Visual Studio profiler

**Common Causes:**
1. **Debug build** - Use `dotnet build -c Release`
2. **VSync disabled** - Enable in settings or driver
3. **Excessive allocations** - Check `Update`/`Draw` loops for `new` objects
4. **Content reloading** - Ensure `LoadContent` called once

### High Memory Usage
**Diagnostics:**
```bash
dotnet-counters monitor --process-id <pid> --counters System.Runtime
```

**Common Causes:**
- Texture leaks (not disposing)
- Event handler leaks (not unregistering)
- Accumulating objects in lists

## Platform-Specific Issues

### Linux: Window Position/Size Wrong
**Cause:** Window manager conflicts with MonoGame's positioning.

**Workaround:**
```csharp
// In GameWindow.Initialize()
Window.Position = Point.Zero; // Or desired position
```

### Linux: Input Lag / Keyboard Repeat
**Cause:** SDL2 keyboard repeat settings.

**Workaround:**
```csharp
// In Program.cs before Run()
SDL.SDL_EnableKeyRepeat(SDL.SDL_DEFAULT_REPEAT_DELAY, SDL.SDL_DEFAULT_REPEAT_INTERVAL);
```

### Windows: Antivirus Blocks Execution
**Cause:** Unsigned executable flagged.

**Resolution:**
- Add exception for build output folder
- Sign executable (requires certificate)

### macOS: Not Supported (DesktopGL)
**Note:** This project uses `MonoGame.Framework.DesktopGL` which targets Windows/Linux. macOS requires `MonoGame.Framework.MacOS` and different project setup.

## Diagnostic Commands

### Check .NET Environment
```bash
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes
```

### Check Project Dependencies
```bash
dotnet list Minesweeper/Minesweeper.csproj package
dotnet list Minesweeper/Minesweeper.csproj package --vulnerable
dotnet list Minesweeper/Minesweeper.csproj package --outdated
```

### Check Build Output
```bash
dotnet build Minesweeper.slnx -v detailed 2>&1 | head -100
```

### Run with Logging
```bash
# Enable MonoGame logging
export MONOGAME_LOG_LEVEL=Debug
dotnet run --project Minesweeper/Minesweeper.csproj
```

### Inspect Settings File
```bash
cat ~/.local/share/Minesweeper/Settings.xml
# Windows: type %LocalAppData%\Minesweeper\Settings.xml
```

## Known Issues Checklist

| Issue | Status | Workaround |
|-------|--------|------------|
| Timer not updating | Open | Call `GameManager.Update()` in `GameplayScreen.Update()` |
| Fullscreen resolution not synced | Open | Update `GraphicsSettings.Resolution` in `ApplyGraphicsSettings()` |
| Settings exceptions unhandled | Open | Wrap `Load()`/`Save()` in try-catch |
| No save/load game | By design | Implement `SaveGame`/`LoadGame` feature |
| No sound effects | By design | Add audio content and playback |
| Hardcoded difficulty | By design | Add difficulty selection |
| No localisation | By design | Add resource files |

## Getting Help

1. **Check existing issues:** GitHub Issues
2. **Enable debug mode:** Set `DebugMode = true` in Settings.xml
3. **Collect logs:** Application output, `dotnet build -v detailed`
4. **Minimal reproduction:** Create small test case
5. **Report:** Include OS, .NET version, GPU, steps to reproduce