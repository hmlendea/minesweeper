# Integration Tests

## Current Status

**No integration tests exist** in the repository.

## Planned Integration Test Scenarios

### Screen Transition Tests

| Scenario | Description |
|----------|-------------|
| `SplashScreen_AutoAdvances_ToTitleScreen` | Verify 2-second delay or input skip |
| `TitleScreen_NewGame_NavigatesToGameplay` | Menu link click transitions |
| `TitleScreen_Settings_NavigatesToSettings` | Menu link click transitions |
| `SettingsScreen_Back_ReturnsToTitle` | Back link click |
| `GameplayScreen_Escape_ReturnsToTitle` | Escape key handling |
| `GameplayScreen_MineHit_TransitionsToGameOver` | Game over flow |
| `GameplayScreen_Victory_TransitionsToVictory` | Win flow |
| `GameOverScreen_AutoAdvances_ToTitle` | 4-second delay or input |
| `VictoryScreen_AutoAdvances_ToTitle` | 4-second delay or input |

### Gameplay Integration Tests

| Scenario | Description |
|----------|-------------|
| `GameplayScreen_LeftClick_ClearsTile` | Mouse input → GameManager.ClearTile |
| `GameplayScreen_RightClick_FlagsTile` | Mouse input → GameManager.FlagTile |
| `GameplayScreen_RestartKey_ResetsGame` | N/R key → NewGame |
| `GameplayScreen_FirstClick_Safe` | First click never hits mine |
| `GameplayScreen_FloodFill_ClearsRegion` | Zero tile clears connected area |
| `GameplayScreen_FlagLimit_Enforced` | Cannot exceed 24 flags |
| `GameplayScreen_Timer_Updates` | Timer increments during play |

### Settings Integration Tests

| Scenario | Description |
|----------|-------------|
| `SettingsScreen_FullscreenToggle_Persists` | Toggle → Save → Reload |
| `SettingsManager_LoadMissing_CreatesDefaults` | First run behaviour |
| `SettingsManager_CorruptedXML_HandlesGracefully` | Error handling |

## Required Test Infrastructure

### Test Host Setup

```csharp
// Hypothetical integration test base
public abstract class IntegrationTestBase
{
    protected GameWindow GameWindow;
    protected ScreenManager ScreenManager;
    protected InputManager InputManager;

    [SetUp]
    public void SetUp()
    {
        // Headless MonoGame setup or test host
        GameWindow = new GameWindow();
        // Run one frame to initialise
    }

    [TearDown]
    public void TearDown()
    {
        GameWindow?.Dispose();
    }
}
```

### Headless Rendering

MonoGame requires a graphics device. Options:

1. **MonoGame.Extended.Testing** — Provides headless `GraphicsDevice`
2. **Virtual Framebuffer (Xvfb)** — CI-friendly, real GL context
3. **Software Rasterizer** — `MESA_GL_VERSION_OVERRIDE` + `LIBGL_ALWAYS_SOFTWARE`

### Input Simulation

```csharp
// Simulate input events
InputManager.Instance.SimulateKeyPress(Keys.N);
InputManager.Instance.SimulateMouseClick(MouseButton.Left, x, y);
```

## CI Integration

**Workflow Addition (`.github/workflows/dotnet.yml`):**

```yaml
- name: Integration Tests
  run: |
    # Start virtual display
    Xvfb :99 -screen 0 1024x768x24 &
    export DISPLAY=:99
    dotnet test Minesweeper.IntegrationTests/Minesweeper.IntegrationTests.csproj --no-build
```

## Recommended Project Structure

```
Minesweeper.IntegrationTests/
├── Minesweeper.IntegrationTests.csproj
├── ScreenTransitions/
│   ├── SplashScreenTests.cs
│   ├── TitleScreenTests.cs
│   ├── GameplayScreenTests.cs
│   ├── SettingsScreenTests.cs
│   └── EndGameScreenTests.cs
├── Gameplay/
│   ├── InputHandlingTests.cs
│   ├── GameFlowTests.cs
│   └── WinLossTests.cs
└── Settings/
    └── SettingsPersistenceTests.cs
```

## Dependencies Needed

| Package | Purpose |
|---------|---------|
| `MonoGame.Extended.Testing` | Headless GraphicsDevice |
| `NUnit` | Test framework |
| `NUnit3TestAdapter` | VS integration |
| `Microsoft.NET.Test.Sdk` | Test host |

## Priority

**High:** Screen transitions and core gameplay loop
**Medium:** Settings persistence
**Low:** Visual regression (screenshot comparison)