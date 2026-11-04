# Frequently Asked Questions

## General

### What is this project?
A classic Minesweeper implementation built with .NET 10.0 and MonoGame (DesktopGL), using the NuciXNA framework for UI, input, and content management.

### What platforms are supported?
- **Primary:** Linux (tested on Ubuntu/Debian)
- **Secondary:** Windows (via DesktopGL)
- **Not supported:** macOS (requires MonoGame.MacOS variant)

### What are the system requirements?
- .NET 10.0 SDK/Runtime
- OpenGL 3.0+ compatible GPU
- SDL2 and OpenAL libraries (Linux)
- ~50 MB disk space

## Gameplay

### How do I play?
- **Left click:** Reveal a tile
- **Right click:** Place/remove flag
- **N or R key:** Restart current game
- **Escape:** Return to title screen

### Why does the first click never hit a mine?
The game delays mine placement until the first click. The clicked tile and its 8 neighbours are guaranteed mine-free.

### How many mines are there?
24 mines on a 16×16 board (256 tiles total).

### What do the numbers mean?
Numbers (1–8) indicate how many mines are in the 8 surrounding tiles.

### How do I win?
Clear all 232 non-mine tiles. Flags are optional.

### How do I lose?
Click on a tile containing a mine.

### Why is the timer stuck at 00:00?
Known bug: `GameManager.Update()` is not called from `GameplayScreen.Update()`. The timer logic exists but never executes.

## Settings

### Where are settings stored?
- **Linux:** `~/.local/share/Minesweeper/Settings.xml`
- **Windows:** `%LocalAppData%\Minesweeper\Settings.xml`

### How do I reset settings?
Delete `Settings.xml` — it will be recreated with defaults on next launch.

### What settings exist?
- **Audio:** Sound enabled/disabled
- **Graphics:** Fullscreen on/off, resolution
- **Debug:** Debug mode (shows FPS counter)

### Does fullscreen work?
Partially. Toggling fullscreen changes the window mode but doesn't update the `Resolution` setting correctly.

## Development

### How do I build?
```bash
dotnet build Minesweeper.slnx
```

### How do I run?
```bash
dotnet run --project Minesweeper/Minesweeper.csproj
```

### How do I run tests?
```bash
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj
```

### How do I add a new screen?
1. Create class inheriting `GameScreen` in `Gui/Screens/`
2. Register in `GameWindow.LoadContent()`: `ScreenManager.Instance.RegisterScreen<NewScreen>()`
3. Navigate: `ScreenManager.Instance.ChangeScreens<NewScreen>()`

### How do I change board size?
Modify `GameDefines.TableSize` and dependent constants (`BoardWidth`, `BoardHeight`, `WindowWidth`, `WindowHeight`, `MinesCount`). Update tests.

### What is NuciXNA?
A collection of MonoGame extensions (Gui, Graphics, Input, Primitives, DataAccess) by Nuci. Provides screen management, UI controls, content loading, and XML serialisation.

### Why XML for settings?
NuciDAL (Nuci Data Access Layer) provides `XmlFileObject<T>` for simple serialisation. No external config library needed.

### How do I add sound effects?
1. Add `.wav`/`.ogg` to `Content/Audio/`
2. Reference in `Content/Content.mgcb`
3. Load via `NuciContentManager.Instance.Load<SoundEffect>("Audio/name")`
4. Play in `GameplayScreen` or `GameManager`

## Troubleshooting

### Game won't start on Linux
Install dependencies:
```bash
sudo apt-get install libsdl2-2.0-0 libopenal1 mesa-vulkan-drivers
```
Try forcing X11: `GDK_BACKEND=x11 dotnet run ...`

### Content build fails
```bash
dotnet tool install --global dotnet-mgcb
dotnet build Minesweeper.slnx
```

### Tests fail with MonoGame errors
Unit tests should not need MonoGame. Mock or avoid MonoGame types in unit tests.

### Settings not saving
Check write permissions on `~/.local/share/Minesweeper/`

## Contributing

### Code style?
- Explicit types (no `var`)
- Expression-bodied members
- Block namespaces
- 4-space indentation
- `Lock` for singletons

### Where do I report bugs?
GitHub Issues

### Can I add difficulty levels?
Yes — see `change-guide.md` for procedure. Requires making `TableSize` and `MinesCount` configurable.

### Is there multiplayer?
No. Single-player only.

### Is there a high score table?
No. Not implemented.

## Technical

### Why .NET 10.0?
Latest LTS at project creation. Can be downgraded to .NET 8.0 by changing `TargetFramework` in `.csproj`.

### Why MonoGame DesktopGL?
Cross-platform OpenGL backend. Works on Windows/Linux without platform-specific frameworks.

### Why no dependency injection?
Simple singleton pattern via NuciXNA managers. Low complexity, single-threaded.

### Why recursive flood-fill?
Simple implementation. Could stack overflow on huge boards (not an issue at 16×16).

### Thread safety?
Single-threaded by design. Only `SettingsManager.Instance` uses `Lock` for initialisation.