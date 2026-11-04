# Change Guide

Procedures for common modifications to the Minesweeper codebase.

## Adding a New Screen

### 1. Create Screen Class
```csharp
// Minesweeper/Gui/Screens/NewScreen.cs
using NuciXNA.Gui.Screens;

namespace Minesweeper.Gui.Screens;

public sealed class NewScreen : GameScreen
{
    public override void DoLoadContent() { /* ... */ }
    public override void DoUnloadContent() { /* ... */ }
    public override void Update(GameTime gameTime) { /* ... */ }
    public override void Draw(GameTime gameTime) { /* ... */ }
}
```

### 2. Register in GameWindow
```csharp
// Minesweeper/GameWindow.cs - LoadContent()
ScreenManager.Instance.RegisterScreen<NewScreen>();
```

### 3. Navigate to Screen
```csharp
// From any screen
ScreenManager.Instance.ChangeScreens<NewScreen>();
```

### 4. Add Transition Delay (Optional)
```csharp
// In source screen's Update
if (condition)
{
    _transitionTimer += gameTime.ElapsedGameTime.TotalSeconds;
    if (_transitionTimer >= 2.0)
    {
        ScreenManager.Instance.ChangeScreens<NewScreen>();
    }
}
```

## Modifying Board Size

### 1. Update GameDefines
```csharp
// Minesweeper/Settings/GameDefines.cs
public const int TableSize = 20; // Was 16
```

### 2. Update Dependent Constants
```csharp
public const int BoardWidth = MapTileSize * TableSize;  // 800
public const int BoardHeight = MapTileSize * TableSize; // 800
public const int WindowWidth = BoardWidth;              // 800
public const int WindowHeight = BoardHeight + InfoBarHeight; // 860
```

### 3. Update Mine Count (Proportional)
```csharp
public const int MinesCount = TableSize * TableSize / 10; // ~40 for 20×20
```

### 4. Verify/Update
- `GuiGameBoard.cs` rendering loops (uses `GameDefines.TableSize`)
- `GameManager.cs` win condition (`TableSize * TableSize - MinesCount`)
- `TileRepository.InitialiseTiles()` (creates `TableSize²` tiles)
- Unit tests expecting 16×16 or 24 mines

## Adding a New Setting

### 1. Add to Settings Classes
```csharp
// Minesweeper/Settings/GraphicsSettings.cs (or AudioSettings.cs)
public class GraphicsSettings
{
    public bool Fullscreen { get; set; }
    public Size2D Resolution { get; set; }
    public bool VSync { get; set; } = true; // New setting
}
```

### 2. Expose in SettingsManager
```csharp
// Minesweeper/Settings/SettingsManager.cs
public GraphicsSettings GraphicsSettings { get; } = new();
```

### 3. Apply Setting (if graphics)
```csharp
// In SettingsManager or GameWindow
public void ApplyGraphicsSettings()
{
    var g = GraphicsManager.Instance.Graphics;
    g.SynchronizeWithVerticalRetrace = GraphicsSettings.VSync;
    g.ApplyChanges();
}
```

### 4. Add UI Control
```csharp
// Minesweeper/Gui/Screens/SettingsScreen.cs
// Add checkbox/button in DoLoadContent
```

### 5. Persistence
- Automatic via `SettingsManager.Save()` (XML serialisation)
- No code changes needed

## Adding a New Tile Type

### 1. Extend Tile Model
```csharp
// Minesweeper/Models/Tile.cs
public enum TileType { Normal, Mine, Special } // Add enum

public class Tile : ModelBase
{
    public TileType Type { get; set; } = TileType.Normal;
    // ... existing properties
}
```

### 2. Update Repository
```csharp
// Minesweeper/Repositories/TileRepository.cs
// No changes needed (stores Tile base class)
```

### 3. Update GameManager Logic
```csharp
// Minesweeper/GameLogic/GameManagers/GameManager.cs
private void PlaceMines(int excludeX, int excludeY)
{
    // Add logic for special tile placement
}
```

### 4. Update Rendering
```csharp
// Minesweeper/Gui/Controls/GuiGameBoard.cs
// Add case for new TileType in Draw()
```

## Adding a New Difficulty Level

### 1. Create Difficulty Enum
```csharp
// Minesweeper/Settings/GameDefines.cs
public enum Difficulty { Beginner, Intermediate, Expert, Custom }
```

### 2. Make Constants Configurable
```csharp
// Move TableSize, MinesCount to Settings or DifficultyConfig
public static class DifficultyConfig
{
    public static (int Size, int Mines) Get(Difficulty d) => d switch
    {
        Difficulty.Beginner => (9, 10),
        Difficulty.Intermediate => (16, 40),
        Difficulty.Expert => (30, 99),
        _ => (16, 24)
    };
}
```

### 3. Update GameManager
```csharp
// Accept difficulty in NewGame()
public void NewGame(Difficulty difficulty = Difficulty.Intermediate)
{
    var (size, mines) = DifficultyConfig.Get(difficulty);
    TableSize = size;
    MinesCount = mines;
    // Recalculate BoardWidth/Height etc.
}
```

### 4. Add UI Selection
```csharp
// TitleScreen or SettingsScreen - difficulty selector
```

## Modifying Win/Loss Conditions

### Current Logic (GameManager.cs)
```csharp
// Win: all non-mine tiles cleared
if (clearedCount == TableSize * TableSize - MinesCount)
    Completed = true;

// Loss: mine cleared
if (tile.Mined)
    Alive = false;
```

### Example: Add "Flag All Mines" Win
```csharp
// Additional win condition
var flaggedMines = _tileRepository.GetAll()
    .Count(t => t.Flagged && t.Mined);
if (flaggedMines == MinesCount && FlagsPlaced == MinesCount)
    Completed = true;
```

## Adding Sound Effects

### 1. Add Content
- Place `.wav`/`.ogg` in `Content/Audio/`
- Add to `Content/Content.mgcb`

### 2. Load in GameWindow
```csharp
// Minesweeper/GameWindow.cs
private SoundEffect _clickSound;
private SoundEffect _flagSound;
private SoundEffect _explosionSound;
private SoundEffect _victorySound;

protected override void LoadContent()
{
    _clickSound = NuciContentManager.Instance.Load<SoundEffect>("Audio/click");
    _flagSound = NuciContentManager.Instance.Load<SoundEffect>("Audio/flag");
    _explosionSound = NuciContentManager.Instance.Load<SoundEffect>("Audio/explosion");
    _victorySound = NuciContentManager.Instance.Load<SoundEffect>("Audio/victory");
}
```

### 3. Play in GameManager/GameplayScreen
```csharp
// GameplayScreen.Update()
if (InputManager.Instance.IsNewMouseButtonPress(MouseButtons.Left))
{
    _clickSound.Play();
    _gameManager.ClearTile(x, y);
}
```

### 4. Respect AudioSettings
```csharp
if (SettingsManager.Instance.AudioSettings.SoundEnabled)
    _clickSound.Play();
```

## Adding Localisation

### 1. Create Resource Files
```
Resources/
  Strings.resx          (default/en)
  Strings.ro.resx       (Romanian)
  Strings.fr.resx       (French)
```

### 2. Add Resource Wrapper
```csharp
// Minesweeper/Resources/Strings.cs
public static class Strings
{
    public static string MinesLabel => Resources.Strings.MinesLabel;
    public static string GameOver => Resources.Strings.GameOver;
    // ...
}
```

### 3. Replace Hardcoded Strings
```csharp
// GuiInfoBar.cs
_spriteBatch.DrawString(_font, $"{Strings.MinesLabel}: {FlagsRemaining}", ...);

// GameOverScreen.cs
_spriteBatch.DrawString(_font, Strings.GameOver, ...);
```

### 4. Add Language Setting
```csharp
// SettingsManager.cs
public CultureInfo Language { get; set; } = CultureInfo.CurrentUICulture;
```

## Adding Unit Tests

### 1. Create Test File
```csharp
// Minesweeper.UnitTests/GameLogic/GameManagers/NewFeatureTests.cs
using NUnit.Framework;
using Minesweeper.GameLogic.GameManagers;

namespace Minesweeper.UnitTests.GameLogic.GameManagers;

[TestFixture]
public class NewFeatureTests
{
    private GameManager _gameManager;

    [SetUp]
    public void SetUp()
    {
        _gameManager = new GameManager();
        _gameManager.NewGame();
    }

    [Test]
    public void NewFeature_ShouldWork()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### 2. Follow Conventions
- Namespace: `Minesweeper.UnitTests.<MirrorSourcePath>`
- Class: `<ClassName>Tests`
- Methods: `MethodName_Scenario_ExpectedResult`
- Use `[SetUp]` for common initialisation

### 3. Run Tests
```bash
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj
```

## Updating Dependencies

### 1. Check Current Versions
```bash
dotnet list Minesweeper/Minesweeper.csproj package
```

### 2. Update NuGet Packages
```bash
dotnet add Minesweeper/Minesweeper.csproj package NuciXNA.Gui --version 3.3.0
```

### 3. Verify Build
```bash
dotnet build Minesweeper.slnx
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj
```

### 4. Check Breaking Changes
- Review NuciXNA release notes
- Check for API changes in used namespaces
- Update code as needed

## Adding CI Checks

### 1. Edit Workflow
```yaml
# .github/workflows/dotnet.yml
- name: Run Tests
  run: dotnet test --no-build --verbosity normal

- name: Check Formatting
  run: dotnet format --verify-no-changes

- name: Security Audit
  run: dotnet list package --vulnerable --include-transitive
```

### 2. Add New Job (Optional)
```yaml
jobs:
  build:
    # ...
  test:
    needs: build
    # ...
  security:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Audit
        run: dotnet list package --vulnerable
```

## Common Pitfalls

| Change | Pitfall | Solution |
|--------|---------|----------|
| Board size | Hardcoded 16 in tests | Update test expectations |
| New screen | Forget RegisterScreen | Add to GameWindow.LoadContent |
| New setting | Not persisted | SettingsManager auto-saves XML |
| Tile property | Not in ModelBase | All public props serialised |
| Graphics change | Not applied | Call Graphics.ApplyChanges() |
| Timer | Not updating | Call GameManager.Update() |
| Mouse coords | InfoBar offset | Subtract InfoBarHeight |