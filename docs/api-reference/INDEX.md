# API Reference

Auto-generated and curated API documentation for Minesweeper.

## Namespaces

| Namespace | Description |
|-----------|-------------|
| `Minesweeper` | Root namespace, Program entry point |
| `Minesweeper.GameLogic.GameManagers` | Game rules, IGameManager, GameManager |
| `Minesweeper.Gui` | UI components, screens, controls |
| `Minesweeper.Gui.Controls` | GuiButton, GuiGameBoard, GuiInfoBar |
| `Minesweeper.Gui.Helpers` | FramerateCounter |
| `Minesweeper.Gui.Screens` | GameScreen derivatives |
| `Minesweeper.Models` | Tile, ModelBase |
| `Minesweeper.Repositories` | TileRepository, Repository, RepositoryException |
| `Minesweeper.Settings` | SettingsManager, AudioSettings, GraphicsSettings, GameDefines, ApplicationPaths |

## Core Interfaces

### IGameManager
**Location:** `Minesweeper.GameLogic.GameManagers.IGameManager`

```csharp
public interface IGameManager
{
    GameTime GameTime { get; }
    int TableSize { get; }
    bool IsRunning { get; }
    bool Alive { get; }
    int MinesCount { get; }
    int FlagsLimit { get; }
    int FlagsPlaced { get; }
    int FlagsRemaining { get; }
    bool Completed { get; }
    Tile GetTile(int x, int y);
    IEnumerable<Tile> GetAllTiles();
    void ClearTile(int x, int y);
    void FlagTile(int x, int y);
    void NewGame();
}
```

### IGameLogicManager
**Location:** `Minesweeper.GameLogic.GameManagers.IGameLogicManager`

```csharp
public interface IGameLogicManager
{
    void Update(GameTime gameTime);
}
```

## Core Classes

### GameManager
**Location:** `Minesweeper.GameLogic.GameManagers.GameManager`
**Implements:** `IGameManager`, `IGameLogicManager`

**Key Methods:**
- `ClearTile(int x, int y)` — Reveal tile; places mines on first call
- `FlagTile(int x, int y)` — Toggle flag; enforces limit
- `NewGame()` — Full reset
- `Update(GameTime gameTime)` — Timer accumulation (not currently called)

**Properties:** All IGameManager properties + `Timer` (string MM:SS)

### Tile
**Location:** `Minesweeper.Models.Tile`
**Inherits:** `NuciDAL.Models.ModelBase`

**Properties:**
- `X`, `Y` (int) — Coordinates 0–15
- `DangerLevel` (int) — 0–8 adjacent mines
- `Flagged`, `Cleared`, `Mined` (bool) — State flags
- `Id` (string) — Computed `"X,Y"`

**Equality:** Based on `Id` (position)

### TileRepository
**Location:** `Minesweeper.Repositories.TileRepository`
**Inherits:** `NuciDAL.Repositories.Repository<Tile>`

**Methods:**
- `Get(int x, int y)` — Lookup by coordinates
- `GetAll()` — All tiles
- `Clear()` — Remove all (called on NewGame)
- `InitialiseTiles()` — Create 256 tiles

### SettingsManager
**Location:** `Minesweeper.Settings.SettingsManager`
**Pattern:** Singleton

**Properties:**
- `AudioSettings` (AudioSettings)
- `GraphicsSettings` (GraphicsSettings)
- `DebugMode` (bool)

**Methods:**
- `Load()` — Deserialise XML
- `Save()` — Serialise XML
- `ApplyGraphicsSettings()` — Apply to GraphicsDevice

### GameWindow
**Location:** `Minesweeper.GameWindow`
**Inherits:** `Microsoft.Xna.Framework.Game`

**Lifecycle:**
- `Initialize()` — GraphicsManager, singletons, ScreenManager
- `LoadContent()` — Content, settings, register screens
- `Update(GameTime)` — ScreenManager.Update, FpsIndicator
- `Draw(GameTime)` — ScreenManager.Draw, FpsIndicator

### GameScreen (Base)
**Location:** `NuciXNA.Gui.Screens.GameScreen`

**Override Methods:**
- `DoLoadContent()` — Called once on activation
- `DoUnloadContent()` — Called once on deactivation
- `Update(GameTime)` — Per-frame logic
- `Draw(GameTime)` — Per-frame rendering
- `RegisterEvents()` / `UnregisterEvents()` — Input subscriptions

## Screen Classes

| Screen | Purpose | Transitions To |
|--------|---------|----------------|
| `SplashScreen` | Logo display | TitleScreen (2s or input) |
| `TitleScreen` | Main menu | GameplayScreen, SettingsScreen |
| `GameplayScreen` | Active game | GameOverScreen, VictoryScreen, TitleScreen |
| `GameOverScreen` | Mine hit | TitleScreen (4s or key) |
| `VictoryScreen` | All clear | TitleScreen (4s or key) |
| `SettingsScreen` | Options | TitleScreen |

## Control Classes

### GuiGameBoard
**Location:** `Minesweeper.Gui.Controls.GuiGameBoard`
**Renders:** 16×16 tile grid, numbers, flags, mines, grid lines

### GuiInfoBar
**Location:** `Minesweeper.Gui.Controls.GuiInfoBar`
**Renders:** Flags remaining, face, timer

### GuiButton
**Location:** `Minesweeper.Gui.Controls.GuiButton`
**Base:** NuciXNA button with texture/label

### FpsIndicator
**Location:** `Minesweeper.Gui.FpsIndicator`
**Shows:** Current FPS (when DebugMode=true)

## Settings Classes

### AudioSettings
```csharp
public class AudioSettings
{
    public bool SoundEnabled { get; set; } = true;
}
```

### GraphicsSettings
```csharp
public class GraphicsSettings
{
    public bool Fullscreen { get; set; } = false;
    public Size2D Resolution { get; set; } = new(640, 700);
}
```

### GameDefines (Constants)
```csharp
public static class GameDefines
{
    public const int MapTileSize = 40;
    public const int TableSize = 16;
    public const int MinesCount = 24;
    public const int InfoBarHeight = 60;
    public const int BoardWidth = 640;
    public const int BoardHeight = 640;
    public const int WindowWidth = 640;
    public const int WindowHeight = 700;
}
```

## Repository Base (NuciDAL)

### Repository<T>
**Namespace:** `NuciDAL.Repositories`

**Methods:**
- `Add(T item)` — Add by item.Id
- `Remove(string id)` — Remove by Id
- `Get(string id)` — Get by Id
- `GetAll()` — All items
- `Clear()` — Remove all
- `Save(string path)` — XML serialise
- `Load(string path)` — XML deserialise

### ModelBase
**Namespace:** `NuciDAL.Models`

**Properties:**
- `Id` (string) — Primary key

**Methods:**
- `Equals(object)` — By Id
- `GetHashCode()` — Id.GetHashCode()

## NuciXNA Services (Singletons)

| Service | Namespace | Purpose |
|---------|-----------|---------|
| `GraphicsManager` | `NuciXNA.Graphics` | GraphicsDevice wrapper |
| `NuciContentManager` | `NuciXNA.Graphics` | Content loading |
| `ScreenManager` | `NuciXNA.Gui.Screens` | Screen stack/transitions |
| `InputManager` | `NuciXNA.Input` | Keyboard/mouse state |
| `SettingsManager` | `Minesweeper.Settings` | App settings |

## Extension Methods

### ScreenManagerExtensions
**Location:** `NuciXNA.Gui.Screens.ScreenManagerExtensions`

```csharp
public static void RegisterScreen<T>(this ScreenManager manager) where T : GameScreen, new();
public static void ChangeScreens<T>(this ScreenManager manager) where T : GameScreen, new();
```

## Content Assets

| Asset | Path | Type |
|-------|------|------|
| DefaultFont | `Content/Fonts/DefaultFont.spritefont` | SpriteFont |
| MenuFont | `Content/Fonts/MenuFont.spritefont` | SpriteFont |
| InfoBarFont | `Content/Fonts/InfoBarFont.spritefont` | SpriteFont |
| ToolTipFont | `Content/Fonts/ToolTipFont.spritefont` | SpriteFont |
| FrameCounterFont | `Content/Fonts/FrameCounterFont.spritefont` | SpriteFont |
| Logo | `Content/SplashScreen/Logo.png` | Texture2D |
| Flag | `Content/Tiles/flag.png` | Texture2D |
| Mine | `Content/Tiles/mine.png` | Texture2D |
| FillImage | `Content/ScreenManager/FillImage.png` | Texture2D |
| MissingTexture | `Content/ScreenManager/missing-texture.png` | Texture2D |

Built to `.xnb` in `Content/bin/DesktopGL/Content/`