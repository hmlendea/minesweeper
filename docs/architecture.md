# Architecture

This document elaborates on the root [ARCHITECTURE.md](../ARCHITECTURE.md) with implementation-level detail.

## Architectural Style

The application follows a **Layered Architecture** with **Screen-Based UI** pattern:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  GameWindow → ScreenManager → Screens → GuiControls         │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Game Logic Layer                        │
│  IGameManager → GameManager → TileRepository → Tile         │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                      │
│  SettingsManager, ApplicationPaths, NuciXNA Singletons      │
└─────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Presentation Layer

**Location:** `Minesweeper/GameWindow.cs`, `Minesweeper/Gui/`

**Components:**
- `GameWindow` — MonoGame `Game` subclass; owns graphics device, content root, main loop
- `ScreenManager` (NuciXNA) — Screen stack, transitions, content loading
- Screens — `SplashScreen`, `TitleScreen`, `GameplayScreen`, `SettingsScreen`, `GameOverScreen`, `VictoryScreen`
- GUI Controls — `GuiGameBoard`, `GuiInfoBar`, `GuiButton`
- `FpsIndicator` — Debug overlay

**Responsibilities:**
- MonoGame host initialisation and game loop (`Update`/`Draw`)
- Screen lifecycle and transitions via `ScreenManager.ChangeScreens<T>()`
- Input routing (keyboard, mouse) to active screen
- Rendering via `SpriteBatch`
- GUI control layout and drawing

**Boundary Rules:**
- Screens must not reference each other directly; use `ScreenManager.ChangeScreens<T>()`
- GUI controls registered with `GuiManager.Instance` and owned by their screen
- No game logic in screens or controls; delegate to `IGameManager`

### Game Logic Layer

**Location:** `Minesweeper/GameLogic/GameManagers/`, `Minesweeper/Models/`, `Minesweeper/Repositories/`

**Components:**
- `IGameManager` — Contract exposed to presentation layer
- `IGameLogicManager` — Base interface for content lifecycle
- `GameManager` — Implementation of game rules and state
- `TileRepository` — In-memory tile grid storage
- `Tile` — Immutable-identity model (position, mine, danger, flag, cleared)
- `ModelBase` — Base class with `Id` and equality

**Responsibilities:**
- Game rules: mine placement, danger calculation, flood-fill clearing
- Game state: timer, alive/running/completed flags, flag count
- Tile grid management via repository pattern
- Win/loss detection

**Boundary Rules:**
- `IGameManager` is the only contract exposed to presentation layer
- `TileRepository` encapsulates all tile storage; `GameManager` never uses arrays directly
- `Tile` is a pure data model; no behaviour
- Deterministic logic (except `Random` for mine placement); no external dependencies

### Infrastructure Layer

**Location:** `Minesweeper/Settings/`, `Minesweeper/Content/Content.mgcb`

**Components:**
- `SettingsManager` — Singleton settings persistence (XML via NuciDAL)
- `ApplicationPaths` — Platform-specific path resolution
- `GameDefines` — Compile-time constants
- NuciXNA Singletons — `GraphicsManager`, `InputManager`, `NuciContentManager`, `GuiManager`

**Responsibilities:**
- Settings persistence (XML serialisation via NuciDAL)
- Platform-specific path resolution
- Content pipeline (MonoGame Content Builder)
- Cross-cutting services via NuciXNA singletons

**Boundary Rules:**
- Singletons accessed via `Instance` property; no direct instantiation
- Settings loaded once at startup; saved on `GameplayScreen` unload
- Content loaded via `NuciContentManager` at `GameWindow.LoadContent`

## Dependency Direction

```
Program
    └── GameWindow
            ├── ScreenManager
            │       └── Screens
            │               └── GuiControls
            ├── SettingsManager
            │       └── ApplicationPaths
            │       └── XmlFileObject (NuciDAL)
            ├── GraphicsManager (NuciXNA)
            ├── InputManager (NuciXNA)
            ├── NuciContentManager (NuciXNA)
            └── GameplayScreen
                    └── GameManager (IGameManager)
                            └── TileRepository
                                    └── Tile
```

**Rules:**
- **Presentation → Logic:** `GameplayScreen` depends on `IGameManager` abstraction
- **Logic → Infrastructure:** `GameManager` has no dependency on settings, graphics, or input
- **Infrastructure → Nothing:** `SettingsManager`, `ApplicationPaths`, NuciXNA singletons have no upward dependencies
- **No Cycles:** Screens do not reference each other; `ScreenManager` mediates all transitions
- **Content Isolation:** Content pipeline output consumed only at `GameWindow.LoadContent`

## Runtime Flow

See [flows/startup-and-rendering.md](./flows/startup-and-rendering.md) and [flows/gameplay-loop.md](./flows/gameplay-loop.md) for detailed sequences.