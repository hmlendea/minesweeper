# Presentation Layer

## Overview

The presentation layer is built on MonoGame's `Game` class and the NuciXNA screen-based UI framework. It handles all rendering, input routing, and screen management.

## GameWindow

**Location:** `Minesweeper/GameWindow.cs`

**Inheritance:** `Microsoft.Xna.Framework.Game`

**Responsibilities:**
- Graphics device management via `GraphicsDeviceManager`
- Content pipeline root directory (`Content`)
- Main game loop (`Update`/`Draw`)
- Initialisation of all infrastructure singletons
- Screen manager setup with starting screen

**Key Fields:**
- `graphics` — `GraphicsDeviceManager` configured with 24-bit depth/stencil
- `spriteBatch` — Shared `SpriteBatch` for all rendering
- `fpsIndicator` — `FpsIndicator` for debug overlay

**Lifecycle Methods:**

| Method | Purpose |
|--------|---------|
| Constructor | Initialises graphics, content root, mouse visibility, `FpsIndicator` |
| `LoadContent` | Creates `SpriteBatch`; initialises all NuciXNA singletons; loads content; sets starting screen to `SplashScreen` |
| `UnloadContent` | Unloads screen manager and FPS indicator |
| `Update` | Updates settings, screen manager, input manager (if active), FPS indicator |
| `Draw` | Clears to black; draws screen manager and FPS indicator |

**Singleton Initialisation Order (LoadContent):**
1. `GraphicsManager.Instance.SpriteBatch = spriteBatch`
2. `GraphicsManager.Instance.Graphics = graphics`
3. `NuciContentManager.Instance.LoadContent(Content, GraphicsDevice)`
4. `SettingsManager.Instance.LoadContent()`
5. `ScreenManager.Instance.SpriteBatch = spriteBatch`
6. `ScreenManager.Instance.StartingScreenType = typeof(SplashScreen)`
7. `ScreenManager.Instance.LoadContent()`
8. `fpsIndicator.LoadContent()`

## ScreenManager (NuciXNA)

**Type:** `NuciXNA.Gui.Screens.ScreenManager`

**Responsibilities:**
- Screen stack management
- Screen transitions via `ChangeScreens<T>()`
- Content loading/unloading for screens
- Input event forwarding to active screen

**Key Properties:**
- `SpriteBatch` — Shared sprite batch
- `StartingScreenType` — Type of initial screen
- `Size` — Current screen size

## Screens

All screens inherit from `NuciXNA.Gui.Screens.Screen` (or `MenuScreen` for menu-based screens).

### Screen Base Class Contract

| Method | Purpose |
|--------|---------|
| `DoLoadContent()` | Override: load screen-specific content, register controls, register events |
| `DoUnloadContent()` | Override: unload content, unregister events |
| `DoUpdate(GameTime)` | Override: update logic |
| `DoDraw(SpriteBatch)` | Override: custom drawing (most screens delegate to controls) |
| `RegisterEvents()` | Subscribe to `InputManager` events |
| `UnregisterEvents()` | Unsubscribe from `InputManager` events |
| `SetChildrenProperties()` | Layout child controls |

### SplashScreen

**Location:** `Minesweeper/Gui/Screens/SplashScreen.cs`

**Purpose:** Animated logo display; auto-advances to title after delay.

**Properties:**
- `Delay` — Seconds before auto-transition (default: 2)
- `LogoImage` — `GuiImage` with content file `SplashScreen/Logo`

**Behaviour:**
- Counts down `Delay` in `DoUpdate`
- Transitions to `TitleScreen` on delay expiry or any input (keyboard/mouse)
- Centres logo on screen in `SetChildrenProperties`

### TitleScreen

**Location:** `Minesweeper/Gui/Screens/TitleScreen.cs`

**Inheritance:** `MenuScreen` (NuciXNA)

**Purpose:** Main menu with navigation to gameplay and settings.

**Menu Items:**
- `newGameLink` — `GuiMenuLink` → `GameplayScreen`
- `settingsLink` — `GuiMenuLink` → `SettingsScreen`

### GameplayScreen

**Location:** `Minesweeper/Gui/Screens/GameplayScreen.cs`

**Purpose:** Core gameplay; owns `GameManager`; handles input and rendering.

**Key Fields:**
- `game` — `IGameManager` (concrete `GameManager`)
- `infoBar` — `GuiInfoBar` showing timer, mine count, face
- `gameBoard` — `GuiGameBoard` rendering the tile grid
- `transitionDelay` / `transitionScheduled` — End-game transition timing

**Lifecycle:**
- `DoLoadContent`: Creates `GameManager`, `GuiInfoBar`, `GuiGameBoard`; registers controls and input events
- `DoUnloadContent`: Unloads game, unregisters events, saves settings
- `DoUpdate`: Updates game; handles win/loss transition delays (1.5s victory, 2.0s game over)
- `DoDraw`: Empty (controls draw themselves)

**Input Handling:**
- `OnMouseButtonPressed`: Converts screen coordinates to tile coordinates; left-click → `ClearTile`, right-click → `FlagTile`; bounds-checked against board area
- `OnKeyboardKeyPressed`: N/R → `RestartGame` (calls `game.NewGame()`); Escape → `TitleScreen`

### SettingsScreen

**Location:** `Minesweeper/Gui/Screens/SettingsScreen.cs`

**Inheritance:** `MenuScreen`

**Purpose:** Audio/graphics preference editing.

**Menu Items:**
- `fullScreenToggle` — `GuiMenuToggle` bound to `SettingsManager.Instance.GraphicsSettings.Fullscreen`
- `backLink` — `GuiMenuLink` → `TitleScreen`

**Events:**
- `fullScreenToggle.StateChanged` → Updates `SettingsManager.Instance.GraphicsSettings.Fullscreen`
- `DoUnloadContent` saves settings

### GameOverScreen / VictoryScreen

**Location:** `Minesweeper/Gui/Screens/GameOverScreen.cs`, `VictoryScreen.cs`

**Purpose:** Post-game display with auto-return to title.

**Properties:**
- `delay` — Seconds before auto-transition (default: 4)
- `messageText` — `GuiText` with "Game Over" (red) or "You Win!" (green)

**Input:** Enter/Space/Escape → immediate transition to `TitleScreen`

## GUI Controls

### GuiGameBoard

**Location:** `Minesweeper/Gui/Controls/GuiGameBoard.cs`

**Inheritance:** `NuciXNA.Gui.Controls.GuiControl`

**Purpose:** Renders the minefield grid; handles tile visuals.

**Content Dependencies:**
- `pixelTexture` — 1×1 white texture (created at runtime)
- `mineTexture` — `Tiles/mine`
- `flagTexture` — `Tiles/flag`
- `dangerFont` — `Fonts/InfoBarFont`

**Rendering (DoDraw):**
- Iterates `game.TableSize` × `game.TableSize` tiles
- Calculates screen position: `originX + x * tileSize`, `originY + y * tileSize` (originY = `InfoBarHeight`)
- Cleared tiles: draws `revealedColor` background; if `DangerLevel > 0`, draws number in `DangerColors[DangerLevel]`
- Uncleared tiles: draws `unrevealedColor` background; if flagged, draws `flagTexture`; if game over and mined, draws `mineTexture`
- Draws grid lines in `gridLineColor`

**Danger Colours:**
| Level | Colour |
|-------|--------|
| 1 | Blue |
| 2 | Green |
| 3 | Red |
| 4 | Navy |
| 5 | Maroon |
| 6 | Purple |
| 7 | Black |
| 8 | Gray |

### GuiInfoBar

**Location:** `Minesweeper/Gui/Controls/GuiInfoBar.cs`

**Inheritance:** `NuciXNA.Gui.Controls.GuiControl` (sealed)

**Purpose:** Top bar showing flags remaining, face indicator, timer.

**Child Controls:**
- `flagsText` — Left-aligned: "Mines: {FlagsRemaining}"
- `faceText` — Centered: ":)" if alive, ":(" if dead
- `timerText` — Right-aligned: "MM:SS" format

**Layout:** Divides width into thirds; updates in `DoUpdate` via `SetChildrenProperties`

### GuiButton

**Location:** `Minesweeper/Gui/Controls/GuiButton.cs`

**Purpose:** Base button control (used by menu system via NuciXNA)

## FpsIndicator

**Location:** `Minesweeper/Gui/FpsIndicator.cs`

**Purpose:** Debug overlay showing current FPS.

**Dependencies:** `FramerateCounter.Instance`, `SettingsManager.Instance.DebugMode`

**Behaviour:**
- Loads `Fonts/FrameCounterFont`
- Updates `FramerateCounter` with delta time in `Draw`
- Only draws when `DebugMode` is true
- Renders at `Vector2.One` in lime colour

## FramerateCounter

**Location:** `Minesweeper/Gui/Helpers/FramerateCounter.cs`

**Pattern:** Singleton with double-checked locking

**Metrics:**
- `TotalFrames` — Cumulative frame count
- `TotalSeconds` — Cumulative time
- `CurrentFramesPerSecond` — 1 / deltaTime
- `AverageFramesPerSecond` — Rolling average over last 100 frames (`MAXIMUM_SAMPLES`)

**Update:** Called each frame with `deltaTime`; maintains sample queue