# Playing the Game

Complete guide to Minesweeper gameplay mechanics, controls, and strategies.

## Objective

Clear all non-mine tiles without detonating any mines. The board is 16×16 with 24 hidden mines.

## Controls

| Action | Input |
|--------|-------|
| Reveal tile | Left mouse click |
| Place/remove flag | Right mouse click |
| Restart game | `N` or `R` key |
| Return to menu | `Escape` key |

## Game Flow

```
Start → First Click (safe) → Play → Win/Loss → Delay → Title Screen
```

### First Click Safety
- Mines placed **after** first click
- Clicked tile + 8 neighbours guaranteed mine-free
- Timer starts on first click

### During Play
- Left click unrevealed tile → reveals it
- Right click unrevealed tile → toggles flag
- Flagged tiles cannot be left-clicked (must unflag first)
- Numbers (1–8) show adjacent mine count
- Empty tiles (0) auto-reveal neighbours (flood fill)

### Win Condition
- All 232 non-mine tiles revealed
- Flags optional for victory
- Victory screen shows after 1.5s delay

### Loss Condition
- Click a mine tile
- All mines revealed
- Game over screen shows after 2s delay

## Info Bar (Top)

```
[ Mines: 24 ]    [ :) ]    [ 00:00 ]
  Flags left      Face      Timer
```

- **Mines:** Remaining flags (24 - flags placed)
- **Face:** `:)` while alive, `:(` after mine hit
- **Timer:** MM:SS format (known bug: stays at 00:00)

## Tile States

| State | Appearance | Interactions |
|-------|------------|--------------|
| Hidden | Gray square | Left: reveal, Right: flag |
| Flagged | Flag icon | Right: unflag |
| Revealed (number) | Colored number | None |
| Revealed (empty) | Light gray | None |
| Mine (revealed) | Mine icon | Only on loss |

## Number Colours

| Number | Colour | Mines Adjacent |
|--------|--------|----------------|
| 1 | Blue | 1 |
| 2 | Green | 2 |
| 3 | Red | 3 |
| 4 | Navy | 4 |
| 5 | Maroon | 5 |
| 6 | Purple | 6 |
| 7 | Black | 7 |
| 8 | Gray | 8 |

## Basic Strategy

### 1. Start at Corners/Edges
- Fewer neighbours = more information per click
- Corners have 3 neighbours, edges have 5

### 2. Use Numbers as Constraints
- A "1" touching one hidden tile → that tile is a mine
- A "1" with one flagged neighbour → other hidden tiles are safe

### 3. Flag Known Mines
- Right-click to mark suspected mines
- Helps track remaining mines
- Prevents accidental clicks

### 4. Chord Clicking (Advanced)
- Not implemented: Middle-click on revealed number with correct flag count → auto-reveal remaining neighbours

### 5. Pattern Recognition
- **1-2-1 pattern:** Mines at the 1s
- **1-2-2-1 pattern:** Mines at the 1s
- **Corner patterns:** Specific configurations force mine positions

## Keyboard Shortcuts

| Key | Action | Context |
|-----|--------|---------|
| `N` | New game | Gameplay, GameOver, Victory |
| `R` | Restart | Gameplay, GameOver, Victory |
| `Escape` | Title screen | Gameplay |
| `Enter` | Title screen | GameOver, Victory |
| `Space` | Title screen | GameOver, Victory |

## Screen Transitions

| From | To | Trigger | Delay |
|------|-----|---------|-------|
| Splash | Title | Any input / 2s | 0–2s |
| Title | Gameplay | Click / Enter | Immediate |
| Gameplay | GameOver | Hit mine | 2s |
| Gameplay | Victory | Clear all safe | 1.5s |
| GameOver | Title | Any key / 4s | 0–4s |
| Victory | Title | Any key / 4s | 0–4s |
| Gameplay | Title | Escape | Immediate |

## Tips

1. **First click anywhere** — it's always safe
2. **Flag aggressively** — reduces mental load
3. **Work from knowns** — expand from certain deductions
4. **Count flags** — InfoBar shows remaining mines
5. **Restart often** — `N`/`R` keys are fast

## Known Issues

- **Timer broken:** Stays at 00:00 (GameManager.Update not called)
- **No chord clicking:** Middle-click does nothing
- **No high scores:** Not tracked
- **No difficulty levels:** Fixed 16×16 / 24 mines