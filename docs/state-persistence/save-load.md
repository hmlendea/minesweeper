# Save/Load Game State

## Current State

**No save/load game functionality exists.** The application only persists user preferences (Settings.xml), not game progress.

## Game State That Would Need Persistence

### GameManager State

| Field | Type | Description |
|-------|------|-------------|
| `IsRunning` | `bool` | Game started (first click) |
| `Alive` | `bool` | Player alive |
| `Completed` | `bool` | Victory achieved |
| `FlagsPlaced` | `int` | Current flag count |
| `timer` | `float` | Elapsed seconds |
| `tileRepository` | `TileRepository` | All 256 tiles with state |

### Tile State (per tile)

| Property | Type | Description |
|----------|------|-------------|
| `X`, `Y` | `int` | Position (0-15) |
| `DangerLevel` | `int` | Adjacent mines (0-8) |
| `Flagged` | `bool` | Flag placed |
| `Cleared` | `bool` | Revealed |
| `Mined` | `bool` | Contains mine |

## Required Implementation

### 1. Extend GameManager with Serialisation

```csharp
// Add to GameManager
public GameState GetState()
{
    return new GameState
    {
        IsRunning = this.IsRunning,
        Alive = this.Alive,
        Completed = this.Completed,
        FlagsPlaced = this.FlagsPlaced,
        Timer = this.timer,
        Tiles = tileRepository.GetAll().Select(t => new TileState
        {
            X = t.X,
            Y = t.Y,
            DangerLevel = t.DangerLevel,
            Flagged = t.Flagged,
            Cleared = t.Cleared,
            Mined = t.Mined
        }).ToArray()
    };
}

public void RestoreState(GameState state)
{
    tileRepository.Clear();
    foreach (var ts in state.Tiles)
    {
        Tile tile = new Tile
        {
            X = ts.X,
            Y = ts.Y,
            DangerLevel = ts.DangerLevel,
            Flagged = ts.Flagged,
            Cleared = ts.Cleared,
            Mined = ts.Mined
        };
        tileRepository.Add(tile);
    }
    IsRunning = state.IsRunning;
    Alive = state.Alive;
    Completed = state.Completed;
    FlagsPlaced = state.FlagsPlaced;
    timer = state.Timer;
}
```

### 2. Data Transfer Objects

```csharp
public class GameState
{
    public bool IsRunning { get; set; }
    public bool Alive { get; set; }
    public bool Completed { get; set; }
    public int FlagsPlaced { get; set; }
    public float Timer { get; set; }
    public TileState[] Tiles { get; set; }
}

public class TileState
{
    public int X { get; set; }
    public int Y { get; set; }
    public int DangerLevel { get; set; }
    public bool Flagged { get; set; }
    public bool Cleared { get; set; }
    public bool Mined { get; set; }
}
```

### 3. Save/Load Service

```csharp
public class GameSaveService
{
    private readonly string savePath;

    public GameSaveService()
    {
        savePath = Path.Combine(ApplicationPaths.UserDataDirectory, "savegame.xml");
    }

    public void Save(GameState state)
    {
        XmlFileObject<GameState> saver = new XmlFileObject<GameState>(savePath);
        saver.Save(state);
    }

    public GameState Load()
    {
        if (!File.Exists(savePath)) return null;
        XmlFileObject<GameState> loader = new XmlFileObject<GameState>(savePath);
        return loader.Load();
    }

    public bool HasSave() => File.Exists(savePath);

    public void Delete() => File.Delete(savePath);
}
```

### 4. UI Integration

**TitleScreen:** Add "Continue Game" menu item (enabled only if save exists)

```csharp
continueLink = new GuiMenuLink("Continue Game");
continueLink.Clicked += (s, e) =>
{
    var state = saveService.Load();
    if (state != null)
    {
        ScreenManager.Instance.ChangeScreens<GameplayScreen>();
        // Need to pass state to GameplayScreen
    }
};
```

**GameplayScreen:** Auto-save on exit/pause

```csharp
protected override void DoUnloadContent()
{
    if (game.IsRunning && game.Alive && !game.Completed)
    {
        saveService.Save(game.GetState());
    }
    else
    {
        saveService.Delete();  // Clear save on game over/victory
    }
    // ...
}
```

## File Format (XML)

```xml
<?xml version="1.0" encoding="utf-8"?>
<GameState>
  <IsRunning>true</IsRunning>
  <Alive>true</Alive>
  <Completed>false</Completed>
  <FlagsPlaced>5</FlagsPlaced>
  <Timer>123.45</Timer>
  <Tiles>
    <TileState>
      <X>0</X>
      <Y>0</Y>
      <DangerLevel>1</DangerLevel>
      <Flagged>false</Flagged>
      <Cleared>true</Cleared>
      <Mined>false</Mined>
    </TileState>
    <!-- 255 more tiles -->
  </Tiles>
</GameState>
```

## Considerations

1. **Versioning:** Add `Version` field to `GameState` for future compatibility
2. **Validation:** Verify loaded state consistency (mine count, flag count, etc.)
3. **Corruption Handling:** Try-catch with fallback to new game
4. **Multiple Saves:** Could support named save slots
5. **Auto-Save:** Periodic save during gameplay (every 30s)

## Current Blocker

`GameManager` directly instantiates `TileRepository` and has no interface for state injection. Would need refactoring to support restoration.