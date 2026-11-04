# Data Model

Domain entities, relationships, and persistence for the Minesweeper application.

## Entity Overview

```
┌─────────────────┐       ┌──────────────────┐       ┌─────────────────┐
│     Tile        │       │  TileRepository  │       │   GameManager   │
├─────────────────┤       ├──────────────────┤       ├─────────────────┤
│ X: int          │       │ Get(x,y): Tile   │       │ TableSize: int  │
│ Y: int          │       │ GetAll(): IEnumerable<Tile>│ MinesCount: int │
│ DangerLevel: int│       │ Clear(): void    │       │ IsRunning: bool │
│ Flagged: bool   │       │ Add(Tile): void  │       │ Alive: bool     │
│ Cleared: bool   │       │ Remove(id): bool │       │ Completed: bool │
│ Mined: bool     │       │                  │       │ FlagsPlaced: int│
│ Id: string      │       │                  │       │ Timer: string   │
└────────┬────────┘       └────────┬─────────┘       └────────┬────────┘
         │                         │                        │
         │ 1..256                  │ 1                      │ 1
         ▼                         ▼                        ▼
    ┌─────────────────────────────────────────────────────────────┐
    │                    Game State (per session)                 │
    └─────────────────────────────────────────────────────────────┘
```

## Tile Entity

**Location:** `Minesweeper/Models/Tile.cs`

**Inheritance:** `NuciDAL.Models.ModelBase`

### Properties

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `X` | `int` | 0–15 | Column index |
| `Y` | `int` | 0–15 | Row index |
| `DangerLevel` | `int` | 0–8 | Adjacent mine count |
| `Flagged` | `bool` | — | Player placed flag |
| `Cleared` | `bool` | — | Tile revealed |
| `Mined` | `bool` | — | Contains mine |
| `Id` | `string` | Computed | `"X,Y"` format |

### Identity & Equality

- **Id:** `"X,Y"` (e.g., `"7,12"`)
- **Equals:** Position-based (same X,Y = equal)
- **GetHashCode:** `Id.GetHashCode()`

### State Machine

```
[Created] → Hidden (Cleared=false, Flagged=false, Mined=false)
    │
    ├─Right Click→ Flagged (Flagged=true)
    │    │
    │    └─Right Click→ Hidden (Flagged=false)
    │
    └─Left Click→ Cleared (Cleared=true)
         │
         ├─If Mined→ GameOver (Alive=false)
         │
         └─If DangerLevel=0→ FloodFill (recursive clear neighbours)
```

## TileRepository

**Location:** `Minesweeper/Repositories/TileRepository.cs`

**Inheritance:** `NuciDAL.Repositories.Repository<Tile>`

### Operations

| Method | Description |
|--------|-------------|
| `Get(int x, int y)` | Lookup by coordinates (converts to `"X,Y"` key) |
| `GetAll()` | All 256 tiles |
| `Clear()` | Remove all tiles (called on NewGame) |

### Internal Storage

- `Dictionary<string, Tile>` keyed by `Id`
- XML serialisable via base class `Save()`/`Load()` (unused)

## GameManager

**Location:** `Minesweeper/GameLogic/GameManagers/GameManager.cs`

**Implements:** `IGameManager`, `IGameLogicManager`

### State Properties

| Property | Type | Description |
|----------|------|-------------|
| `TableSize` | `int` | 16 (from GameDefines) |
| `MinesCount` | `int` | 24 (from GameDefines) |
| `IsRunning` | `bool` | First clear performed |
| `Alive` | `bool` | No mine hit |
| `Completed` | `bool` | All non-mines cleared |
| `FlagsLimit` | `int` | 24 (== MinesCount) |
| `FlagsPlaced` | `int` | Current flag count |
| `FlagsRemaining` | `int` | Computed: Limit - Placed |
| `Timer` | `string` | MM:SS format |

### Operations

| Method | Effect |
|--------|--------|
| `GetTile(x,y)` | Returns tile or null |
| `GetAllTiles()` | All tiles |
| `ClearTile(x,y)` | Reveal tile; place mines on first call |
| `FlagTile(x,y)` | Toggle flag; enforce limit |
| `NewGame()` | Reset all state |

### Internal Algorithm

**Mine Placement (First Click):**
1. Exclude clicked tile + 8 neighbours (3×3)
2. Randomly select 24 tiles from remaining 247
3. Set `Mined = true`
4. Calculate `DangerLevel` for all non-mine tiles

**Flood Fill (Zero Danger):**
- Recursive 8-direction clear
- Stops at non-zero danger or flagged/mined tiles

**Win Check:**
- Count cleared tiles
- Victory if `clearedCount == 256 - 24`

## SettingsManager

**Location:** `Minesweeper/Settings/SettingsManager.cs`

**Pattern:** Singleton

### Persisted Properties

| Property | Type | Default |
|----------|------|---------|
| `AudioSettings.SoundEnabled` | `bool` | `true` |
| `GraphicsSettings.Fullscreen` | `bool` | `false` |
| `GraphicsSettings.Resolution` | `Size2D` | `640×700` |
| `DebugMode` | `bool` | `false` |

### Persistence

- **File:** `Settings.xml` in user data directory
- **Format:** XML via `NuciDAL.IO.XmlFileObject<SettingsManager>`
- **Load:** `GameWindow.LoadContent`
- **Save:** `GameplayScreen.DoUnloadContent`, `SettingsScreen.DoUnloadContent`

## GameDefines (Constants)

**Location:** `Minesweeper/Settings/GameDefines.cs`

| Constant | Value | Used By |
|----------|-------|---------|
| `MapTileSize` | 40 | Board rendering, coordinate conversion |
| `TableSize` | 16 | Grid dimensions, win condition |
| `MinesCount` | 24 | Mine placement, flag limit |
| `InfoBarHeight` | 60 | Screen layout, mouse coordinate adjustment |
| `BoardWidth` | 640 | Window size, rendering |
| `BoardHeight` | 640 | Window size, rendering |
| `WindowWidth` | 640 | Window creation |
| `WindowHeight` | 700 | Window creation |

## Relationships

```
GameManager (1) ───── owns ───── (1) TileRepository
TileRepository (1) ──── contains ──── (256) Tile
GameplayScreen (1) ──── uses ──── (1) GameManager
SettingsManager (1) ──── persists ──── (1) Settings.xml
```

## Serialisation Notes

- **Tile:** XML serialisable via `ModelBase` (all public properties)
- **TileRepository:** Inherits `Save()`/`Load()` from NuciDAL base (unused)
- **SettingsManager:** Direct XML serialisation (includes nested objects)
- **GameManager:** Not serialised (no save-game feature)
- **GameDefines:** Compile-time constants (not serialised)