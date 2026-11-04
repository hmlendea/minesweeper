# Host and Composition Root

## Overview

The composition root is the `Program` class which prepares the file system and starts the MonoGame host (`GameWindow`).

## Program

**Location:** `Minesweeper/Program.cs`

**Responsibilities:**
- File system preparation (user data directory creation)
- Game window instantiation and execution
- Static `Game` property for global access (if needed)

**Methods:**

| Method | Purpose |
|--------|---------|
| `Main()` | Entry point; calls `PrepareFiles()` then `RunGame()` |
| `PrepareFiles()` | Creates `ApplicationPaths.UserDataDirectory` if not exists |
| `RunGame()` | Creates `GameWindow`, calls `Run()`, disposes on exit |

**Execution Flow:**
```csharp
[STAThread]
static void Main()
{
    PrepareFiles();  // Ensures %LocalAppData%\Minesweeper\ exists
    RunGame();       // Creates GameWindow, runs MonoGame loop
}
```

## GameWindow as Composition Root

**Location:** `Minesweeper/GameWindow.cs`

**Constructor Dependencies:**
- `GraphicsDeviceManager` — Created internally with default settings
- `FpsIndicator` — Created internally

**LoadContent Dependency Initialisation (Manual Composition):**
```csharp
// Graphics
GraphicsManager.Instance.SpriteBatch = spriteBatch;
GraphicsManager.Instance.Graphics = graphics;

// Content
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);

// Settings
SettingsManager.Instance.LoadContent();

// Screens
ScreenManager.Instance.SpriteBatch = spriteBatch;
ScreenManager.Instance.StartingScreenType = typeof(SplashScreen);
ScreenManager.Instance.LoadContent();

// Debug
fpsIndicator.LoadContent();
```

**No DI Container:** The application uses manual singleton initialisation and direct instantiation. There is no dependency injection container.

## Singleton Initialisation Pattern

All infrastructure singletons use double-checked locking with `Lock` (C# 12):

```csharp
static volatile SettingsManager instance;
static readonly Lock syncRoot = new();

public static SettingsManager Instance
{
    get
    {
        if (instance is null)
        {
            lock (syncRoot)
            {
                instance ??= new SettingsManager();
            }
        }
        return instance;
    }
}
```

**Singletons Initialised at LoadContent:**
1. `GraphicsManager.Instance` (NuciXNA)
2. `NuciContentManager.Instance` (NuciXNA)
3. `SettingsManager.Instance` (Application)
4. `ScreenManager.Instance` (NuciXNA)
5. `InputManager.Instance` (NuciXNA) — Initialised on first access in `Update`
6. `GuiManager.Instance` (NuciXNA) — Initialised on first control registration
7. `FramerateCounter.Instance` (Application) — Initialised on first access in `FpsIndicator.Draw`

## Screen Composition

Screens are composed by `ScreenManager` via `ChangeScreens<T>()`. The `GameplayScreen` composes its own dependencies:

```csharp
// GameplayScreen.DoLoadContent
game = new GameManager();           // Direct instantiation (not via interface)
game.LoadContent();

infoBar = new GuiInfoBar(game);
gameBoard = new GuiGameBoard(game)
{
    Size = new Size2D(TableSize * MapTileSize, TableSize * MapTileSize)
};

GuiManager.Instance.RegisterControls(infoBar, gameBoard);
```

**Note:** `GameplayScreen` directly instantiates `GameManager` rather than receiving `IGameManager` via constructor. This is a known deviation from the architectural boundary rule.

## Settings Composition

`SettingsManager` is a singleton that holds:
- `AudioSettings` — Sound enabled state
- `GraphicsSettings` — Fullscreen, resolution
- `DebugMode` — FPS indicator toggle

Settings are loaded from XML at startup and saved on `GameplayScreen` unload and `SettingsScreen` unload.

## Content Composition

Content is defined in `Content/Content.mgcb` and built at compile time by `MonoGame.Content.Builder.Task`. The `NuciContentManager` loads compiled assets at runtime.

**Asset Categories:**
- Fonts: `FrameCounterFont`, `DefaultFont`, `InfoBarFont`, `MenuFont`, `ToolTipFont`
- ScreenManager: `FillImage`, `missing-texture`
- SplashScreen: `Logo`
- Tiles: `mine`, `flag`

## Shutdown Composition

**GameWindow.Dispose()** (inherited from `Game`):
1. Disposes `GraphicsDeviceManager`
2. Disposes `ContentManager`
3. Disposes `GraphicsDevice`

**Application Shutdown Sequence:**
1. `GameWindow.Run()` returns
2. `Program.RunGame()` calls `Game.Dispose()`
3. Process exits

**Settings Persistence on Shutdown:**
- `GameplayScreen.DoUnloadContent()` → `SettingsManager.Instance.SaveContent()`
- `SettingsScreen.DoUnloadContent()` → `SettingsManager.Instance.SaveContent()`
- Abrupt termination (crash, kill) loses unsaved preferences