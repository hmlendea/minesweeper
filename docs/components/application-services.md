# Application Services

## Overview

Application services are cross-cutting infrastructure services provided by NuciXNA singletons and the application's `SettingsManager`.

## NuciXNA Services

### GraphicsManager

**Type:** `NuciXNA.Graphics.GraphicsManager` (NuciXNA.Graphics 2.2.3)

**Responsibilities:**
- Centralises `SpriteBatch` and `GraphicsDeviceManager`
- Provides `ApplyChanges()` for runtime graphics changes

**Key Properties:**
- `SpriteBatch` — Shared sprite batch for all rendering
- `Graphics` — `GraphicsDeviceManager` instance

**Usage:**
- Initialised in `GameWindow.LoadContent`
- `SettingsManager.Update()` calls `GraphicsManager.Instance.Graphics.ApplyChanges()` when resolution/fullscreen changes

### InputManager

**Type:** `NuciXNA.Input.InputManager` (NuciXNA.Input 2.0.4)

**Responsibilities:**
- Keyboard and mouse state aggregation
- Event-based input notification

**Key Events:**
- `KeyboardKeyPressed` — Fired on key press
- `MouseButtonPressed` — Fired on mouse button press

**Usage:**
- Updated in `GameWindow.Update` when `IsActive`
- `ResetInputStates()` called when inactive
- Screens subscribe in `DoLoadContent`, unsubscribe in `DoUnloadContent`

### NuciContentManager

**Type:** `NuciXNA.DataAccess.Content.NuciContentManager` (NuciXNA.DataAccess 4.0.3)

**Responsibilities:**
- Wraps MonoGame `ContentManager`
- Loads compiled assets (textures, fonts)

**Key Methods:**
- `LoadContent(ContentManager, GraphicsDevice)` — Initialises internal content manager
- `LoadTexture2D(string assetName)` — Loads texture
- `LoadSpriteFont(string assetName)` — Loads sprite font

**Usage:**
- Initialised in `GameWindow.LoadContent`
- Controls call `LoadTexture2D`/`LoadSpriteFont` in their `DoLoadContent`

### ScreenManager

**Type:** `NuciXNA.Gui.Screens.ScreenManager` (NuciXNA.Gui 3.2.1)

**Responsibilities:**
- Screen stack and transitions
- Screen content lifecycle
- Input forwarding to active screen

**Key Methods:**
- `ChangeScreens<T>()` — Generic screen transition
- `LoadContent()` — Loads starting screen
- `Update(GameTime)` — Updates active screen
- `Draw(SpriteBatch)` — Draws active screen

**Usage:**
- Initialised in `GameWindow.LoadContent` with `StartingScreenType = typeof(SplashScreen)`
- Screens transition via `ScreenManager.Instance.ChangeScreens<TargetScreen>()`

### GuiManager

**Type:** `NuciXNA.Gui.GuiManager` (NuciXNA.Gui 3.2.1)

**Responsibilities:**
- GUI control registration and lifecycle
- Control layout and drawing

**Key Methods:**
- `RegisterControls(params GuiControl[])` — Registers controls for update/draw
- `Update(GameTime)` — Updates all registered controls
- `Draw(SpriteBatch)` — Draws all registered controls

**Usage:**
- Screens register controls in `DoLoadContent`
- Controls automatically updated/drawn by screen manager

## Application Services

### SettingsManager

**Location:** `Minesweeper/Settings/SettingsManager.cs`

**Pattern:** Singleton with double-checked locking

**Responsibilities:**
- Settings persistence (XML via NuciDAL)
- Runtime graphics settings application
- Settings state management

**Properties:**
- `AudioSettings` — `AudioSettings` (sound enabled)
- `GraphicsSettings` — `GraphicsSettings` (fullscreen, resolution)
- `DebugMode` — `bool` (FPS indicator toggle)

**Methods:**

| Method | Purpose |
|--------|---------|
| `LoadContent()` | Loads from `ApplicationPaths.SettingsFile`; creates defaults if missing |
| `SaveContent()` | Writes current settings to XML file |
| `Update()` | Applies graphics changes to `GraphicsManager.Instance.Graphics` |

**Persistence:**
- File: `ApplicationPaths.SettingsFile` → `%LocalAppData%\Minesweeper\Settings.xml` (Windows) or `~/.local/share/Minesweeper/Settings.xml` (Linux)
- Format: XML serialisation via `NuciDAL.IO.XmlFileObject<T>`
- Loaded once at startup
- Saved on `GameplayScreen.DoUnloadContent` and `SettingsScreen.DoUnloadContent`

**Graphics Application (Update):**
```csharp
bool graphicsChanged = false;

if (GraphicsManager.Instance.Graphics.IsFullScreen != GraphicsSettings.Fullscreen)
{
    GraphicsManager.Instance.Graphics.IsFullScreen = GraphicsSettings.Fullscreen;
    graphicsChanged = true;
}

if (GraphicsManager.Instance.Graphics.PreferredBackBufferWidth != GraphicsSettings.Resolution.Width ||
    GraphicsManager.Instance.Graphics.PreferredBackBufferHeight != GraphicsSettings.Resolution.Height)
{
    GraphicsManager.Instance.Graphics.PreferredBackBufferWidth = GraphicsSettings.Resolution.Width;
    GraphicsManager.Instance.Graphics.PreferredBackBufferHeight = GraphicsSettings.Resolution.Height;
    graphicsChanged = true;
}

if (graphicsChanged)
{
    GraphicsManager.Instance.Graphics.ApplyChanges();
}
```

### ApplicationPaths

**Location:** `Minesweeper/Settings/ApplicationPaths.cs`

**Responsibilities:**
- Platform-specific path resolution

**Properties:**

| Property | Windows | Linux/macOS |
|----------|---------|-------------|
| `UserDataDirectory` | `%LocalAppData%\Minesweeper` | `~/.local/share/Minesweeper` |
| `LogsDirectory` | `UserDataDirectory\Logs` | `UserDataDirectory/Logs` |
| `SettingsFile` | `UserDataDirectory\Settings.xml` | `UserDataDirectory/Settings.xml` |

**Implementation:**
```csharp
static readonly string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

public static string UserDataDirectory => Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Minesweeper");
```

### GameDefines

**Location:** `Minesweeper/Settings/GameDefines.cs`

**Responsibilities:**
- Compile-time game constants

**Constants:**

| Constant | Value | Description |
|----------|-------|-------------|
| `MapTileSize` | 40 | Pixel size of each tile |
| `TableSize` | 16 | Grid dimension (16×16) |
| `MinesCount` | 24 | Number of mines |
| `InfoBarHeight` | 60 | Top bar height in pixels |
| `BoardWidth` | 640 | `TableSize * MapTileSize` |
| `BoardHeight` | 640 | `TableSize * MapTileSize` |
| `WindowWidth` | 640 | `BoardWidth` |
| `WindowHeight` | 700 | `InfoBarHeight + BoardHeight` |

**Note:** These are `const` values; changing requires recompilation. Not user-configurable at runtime.

### AudioSettings

**Location:** `Minesweeper/Settings/AudioSettings.cs`

**Properties:**
- `SoundEnabled` — `bool` (default: true)

### GraphicsSettings

**Location:** `Minesweeper/Settings/GraphicsSettings.cs`

**Properties:**
- `Fullscreen` — `bool`
- `Resolution` — `Size2D` (computed: fullscreen → display mode; windowed → `GameDefines.WindowWidth/Height`)

## Service Initialisation Order

1. `Program.Main()` → `PrepareFiles()` (creates user data directory)
2. `GameWindow` constructor → `GraphicsDeviceManager`, `FpsIndicator`
3. `GameWindow.LoadContent()`:
   - `SpriteBatch` creation
   - `GraphicsManager.Instance` initialisation
   - `NuciContentManager.Instance.LoadContent()`
   - `SettingsManager.Instance.LoadContent()`
   - `ScreenManager.Instance` initialisation with `SplashScreen`
   - `FpsIndicator.LoadContent()`
4. `GameWindow.Update()` loop:
   - `SettingsManager.Instance.Update()`
   - `ScreenManager.Instance.Update()`
   - `InputManager.Instance.Update()` (if active)
   - `FpsIndicator.Update()`
5. Screen transitions trigger their own `DoLoadContent`/`DoUnloadContent`