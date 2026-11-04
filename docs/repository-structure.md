# Repository Structure

## Source Tree Layout

```
minesweeper/
├── .github/
│   └── workflows/
│       └── dotnet.yml          # CI/CD pipeline
├── Minesweeper/                # Main application project
│   ├── Content/                # MonoGame content pipeline
│   │   ├── Content.mgcb        # Content build definition
│   │   ├── Fonts/              # SpriteFont definitions
│   │   ├── ScreenManager/      # UI framework assets
│   │   ├── SplashScreen/       # Splash screen assets
│   │   └── Tiles/              # Game tile textures
│   ├── GameLogic/
│   │   └── GameManagers/       # Game logic interfaces and implementation
│   │       ├── IGameLogicManager.cs
│   │       ├── IGameManager.cs
│   │       └── GameManager.cs
│   ├── Gui/
│   │   ├── Controls/           # GUI controls
│   │   │   ├── GuiButton.cs
│   │   │   ├── GuiGameBoard.cs
│   │   │   └── GuiInfoBar.cs
│   │   ├── Helpers/
│   │   │   └── FramerateCounter.cs
│   │   ├── Screens/            # Game screens
│   │   │   ├── GameOverScreen.cs
│   │   │   ├── GameplayScreen.cs
│   │   │   ├── SettingsScreen.cs
│   │   │   ├── SplashScreen.cs
│   │   │   ├── TitleScreen.cs
│   │   │   └── VictoryScreen.cs
│   │   └── FpsIndicator.cs
│   ├── Models/
│   │   ├── ModelBase.cs        # Base model with identity
│   │   └── Tile.cs             # Game tile model
│   ├── Repositories/
│   │   ├── Repository.cs       # Generic repository base
│   │   ├── RepositoryException.cs
│   │   └── TileRepository.cs   # Tile-specific repository
│   ├── Resources/
│   ├── Settings/
│   │   ├── ApplicationPaths.cs # Platform-specific paths
│   │   ├── AudioSettings.cs    # Audio preferences
│   │   ├── GameDefines.cs      # Compile-time game constants
│   │   ├── GraphicsSettings.cs # Graphics preferences
│   │   └── SettingsManager.cs  # Settings persistence (singleton)
│   ├── GameWindow.cs           # MonoGame Game host
│   ├── Minesweeper.csproj      # Project file
│   └── Program.cs              # Entry point
├── Minesweeper.UnitTests/      # Unit test project
│   ├── GameLogic/
│   │   └── GameManagers/
│   │       └── GameManagerTests.cs
│   ├── Models/
│   │   └── TileTests.cs
│   ├── Repositories/
│   │   └── TileRepositoryTests.cs
│   └── Minesweeper.UnitTests.csproj
├── docs/                       # Generated documentation
├── ARCHITECTURE.md             # Root architecture document
├── CODE_OF_CONDUCT.md
├── CONTRIBUTING.md
├── LICENSE
├── Minesweeper.slnx            # SDK-style solution file
├── PRIVACY.md
├── README.md
└── SECURITY.md
```

## Module Organisation

### Minesweeper (Main Project)

| Namespace | Responsibility | Key Types |
|-----------|----------------|-----------|
| `Minesweeper` | Application host, entry point | `Program`, `GameWindow` |
| `Minesweeper.GameLogic.GameManagers` | Game rules, state, win/loss | `IGameManager`, `IGameLogicManager`, `GameManager` |
| `Minesweeper.Models` | Domain models | `ModelBase`, `Tile` |
| `Minesweeper.Repositories` | In-memory data access | `Repository<T>`, `TileRepository`, `RepositoryException` |
| `Minesweeper.Settings` | Configuration, persistence | `SettingsManager`, `ApplicationPaths`, `GameDefines`, `AudioSettings`, `GraphicsSettings` |
| `Minesweeper.Gui` | Presentation layer | `FpsIndicator` |
| `Minesweeper.Gui.Controls` | GUI controls | `GuiGameBoard`, `GuiInfoBar`, `GuiButton` |
| `Minesweeper.Gui.Helpers` | Presentation utilities | `FramerateCounter` |
| `Minesweeper.Gui.Screens` | Screen implementations | `SplashScreen`, `TitleScreen`, `GameplayScreen`, `SettingsScreen`, `GameOverScreen`, `VictoryScreen` |

### Minesweeper.UnitTests (Test Project)

| Namespace | Tests For |
|-----------|-----------|
| `Minesweeper.UnitTests.GameLogic.GameManagers` | `GameManager` |
| `Minesweeper.UnitTests.Models` | `Tile` |
| `Minesweeper.UnitTests.Repositories` | `TileRepository` |

## Project Configuration

- **Solution:** `Minesweeper.slnx` (SDK-style)
- **Main Project:** `Minesweeper/Minesweeper.csproj` — `WinExe`, net10.0
- **Test Project:** `Minesweeper.UnitTests/Minesweeper.UnitTests.csproj` — net10.0, `IsTestProject=true`, `ExcludeFromCodeCoverage=true`
- **Common Properties:** All projects have `ImplicitUsings=disable`, `LangVersion=latest`, `Nullable=enable`