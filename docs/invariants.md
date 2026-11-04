# Invariants

System-wide invariants and contracts that must hold throughout the Minesweeper application.

## Game Logic Invariants

### Board Structure
- **Board Size:** Always 16×16 tiles (`GameDefines.TableSize = 16`)
- **Mine Count:** Always 24 mines (`GameDefines.MinesCount = 24`)
- **Tile Size:** Always 40 pixels (`GameDefines.MapTileSize = 40`)
- **Total Tiles:** 256 tiles per game

### Mine Placement
- **First Click Safety:** The first cleared tile and its 8 neighbours (3×3 area) never contain mines
- **Mine Count:** Exactly 24 mines placed after first click
- **Mine Distribution:** Uniform random across eligible tiles

### Danger Levels
- **Range:** 0–8 (inclusive) for non-mine tiles
- **Calculation:** Count of mined tiles in 8-connected neighbourhood
- **Mined Tiles:** `DangerLevel` is irrelevant (not displayed)

### Tile State Transitions
```
Hidden → Cleared (left click)
Hidden ↔ Flagged (right click, toggle)
Flagged → Hidden (right click, toggle)
Cleared → (terminal, no further transitions)
```

### Flag Constraints
- **Maximum Flags:** 24 (`FlagsLimit = MinesCount`)
- **Flag Placement:** Only on uncleared tiles
- **Flag Removal:** Always allowed on flagged tiles

### Win Condition
- **Victory:** All 232 non-mine tiles cleared (`Cleared = true`)
- **Equivalent:** `clearedCount == TableSize² - MinesCount`

### Loss Condition
- **Defeat:** Any mine tile cleared (`Cleared = true && Mined = true`)
- **Effect:** `Alive = false`, all mines revealed

### Timer
- **Starts:** On first `ClearTile` (when `IsRunning` becomes true)
- **Stops:** When `Alive = false` or `Completed = true`
- **Format:** MM:SS (minutes:seconds)
- **Precision:** Float seconds, displayed as whole seconds

## Rendering Invariants

### Screen Dimensions
- **Window:** 640×700 pixels (`GameDefines.WindowWidth × WindowHeight`)
- **Board:** 640×640 pixels (`BoardWidth × BoardHeight`)
- **Info Bar:** 640×60 pixels at top (`InfoBarHeight`)

### Tile Rendering
- **Grid:** 16×16 tiles at 40px each
- **Grid Lines:** 1px between tiles (drawn on right/bottom edges)
- **Cleared Tile:** `revealedColor` background
- **Uncleared Tile:** `unrevealedColor` background
- **Danger Numbers:** Centered, coloured by level (1=Blue, 2=Green, 3=Red, 4=Navy, 5=Maroon, 6=Purple, 7=Black, 8=Gray)
- **Flag:** `flagTexture` on uncleared flagged tiles
- **Mine:** `mineTexture` on uncleared mined tiles only when `!Alive`

### Info Bar Layout
- **Left Third:** "Mines: {FlagsRemaining}"
- **Center Third:** Face ":)" if `Alive`, ":(" if `!Alive`
- **Right Third:** Timer "MM:SS"

## Settings Invariants

### Persistence
- **File:** `Settings.xml` in user data directory
- **Format:** XML via `NuciDAL.XmlFileObject<SettingsManager>`
- **Load:** Once at startup in `GameWindow.LoadContent`
- **Save:** On `GameplayScreen.DoUnloadContent` and `SettingsScreen.DoUnloadContent`

### Default Values
- `AudioSettings.SoundEnabled = true`
- `GraphicsSettings.Fullscreen = false`
- `GraphicsSettings.Resolution = 640×700`
- `DebugMode = false`

### Graphics Application
- **Fullscreen:** Applied via `GraphicsManager.Instance.Graphics.ApplyChanges()`
- **Resolution:** Windowed fixed at 640×700; fullscreen uses display mode

## Repository Invariants

### TileRepository
- **Key Format:** `"X,Y"` (e.g., `"7,12"`)
- **Count:** 256 tiles after `InitialiseTiles()`
- **Lookup:** `Get(x,y)` returns null for out-of-bounds
- **Clear:** Removes all tiles (called on `NewGame()`)

### ModelBase Identity
- **Id:** `"X,Y"` string
- **Equality:** Based on Id only
- **HashCode:** `Id.GetHashCode()`

## Concurrency Invariants

- **Single Thread:** All game code runs on main thread
- **No Locks:** No `lock` statements in game logic (only singleton init)
- **No Async:** No `async`/`await` in game loop
- **Graphics Device:** Only accessed from main thread

## Input Invariants

### Mouse
- **Left Click:** `ClearTile` at tile coordinates
- **Right Click:** `FlagTile` at tile coordinates
- **Bounds:** Ignored if outside board area (adjusted for InfoBar)

### Keyboard
- **N/R:** Restart game (`NewGame()`)
- **Escape:** Return to TitleScreen
- **Enter/Space/Escape (GameOver/Victory):** Return to TitleScreen

## Screen Transition Invariants

### Delays
- **Splash → Title:** 2s auto, any input skips
- **Gameplay → GameOver:** 2s delay
- **Gameplay → Victory:** 1.5s delay
- **GameOver → Title:** 4s auto, Enter/Space/Escape skips
- **Victory → Title:** 4s auto, Enter/Space/Escape skips
- **Gameplay → Title (Esc):** Immediate

### Lifecycle
- **DoLoadContent:** Called once when screen becomes active
- **DoUnloadContent:** Called once when screen is replaced
- **RegisterEvents/UnregisterEvents:** Balanced (every += has -=)

## File System Invariants

### User Data Directory
- **Windows:** `%LocalAppData%\Minesweeper\`
- **Linux:** `~/.local/share/Minesweeper/`
- **Created:** `Program.PrepareFiles()` before game starts

### Files
- `Settings.xml` — User preferences (read/write)
- `Logs/` — Directory for logs (not currently used)

## Known Invariant Violations (Bugs)

1. **Timer Not Updating:** `GameManager.Update()` never called → timer stays at 00:00
2. **Resolution Not Synced:** `GraphicsSettings.Resolution` not updated after fullscreen toggle
3. **Settings Exceptions Uncaught:** XML load/save can crash application
4. **No Save Game:** Game state not persisted (only settings)