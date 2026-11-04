# Logging

## Current State

**No structured logging framework is used.** The application relies on:
- `System.Diagnostics.Debug.WriteLine` for debug output
- `Console.WriteLine` (not used but available)
- No log files, no log levels, no structured logging

## Debug Output

### FpsIndicator

**Location:** `Minesweeper/Gui/FpsIndicator.cs`

```csharp
// No explicit logging, but could add:
Debug.WriteLine($"FPS: {FramerateCounter.Instance.CurrentFramesPerSecond:F1}");
```

### SettingsManager

**Potential Logging Points:**
```csharp
public void LoadContent()
{
    // Could log:
    Debug.WriteLine($"Loading settings from {ApplicationPaths.SettingsFile}");
}

public void SaveContent()
{
    // Could log:
    Debug.WriteLine($"Saving settings to {ApplicationPaths.SettingsFile}");
}
```

### GameManager

**Potential Logging Points:**
```csharp
public void ClearTile(int x, int y)
{
    // Could log:
    Debug.WriteLine($"ClearTile({x}, {y}) - Running: {IsRunning}, Alive: {Alive}");
}
```

## Recommended Logging Setup

### NuGet Package

```xml
<PackageReference Include="Serilog" Version="4.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Debug" Version="3.0.0" />
```

### Logger Initialisation (Program.cs)

```csharp
using Serilog;

static void Main()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Debug()
        .WriteTo.File(
            Path.Combine(ApplicationPaths.LogsDirectory, "minesweeper-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7)
        .CreateLogger();

    try
    {
        Log.Information("Starting Minesweeper");
        PrepareFiles();
        RunGame();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

### Usage in Components

```csharp
// In any class
private static readonly ILogger Log = Log.ForContext<GameManager>();

public void ClearTile(int x, int y)
{
    Log.Debug("ClearTile called at ({X}, {Y})", x, y);
    // ...
}
```

## Log Categories

| Category | Level | Examples |
|----------|-------|----------|
| Startup | Information | Version, paths, content loading |
| Gameplay | Debug | Tile clicks, flag placements, state changes |
| Transitions | Information | Screen changes, game start/end |
| Settings | Information | Load/save, changes applied |
| Errors | Error/Warning | Content load failures, settings corruption |
| Performance | Debug | FPS, frame times |

## Log File Location

**Directory:** `ApplicationPaths.LogsDirectory`
- Windows: `%LocalAppData%\Minesweeper\Logs\`
- Linux: `~/.local/share/Minesweeper/Logs/`

**Files:** `minesweeper-YYYYMMDD.log` (daily rolling)

## Structured Logging Benefits

1. **Queryable:** Search by property (e.g., `TileX: 5`)
2. **Correlatable:** Trace game session via `SessionId`
3. **Performant:** Async sinks, minimal overhead
4. **Configurable:** Level per namespace, output per environment

## Current Debug.WriteLine Usage

**None found in codebase.** Only potential locations identified above.

## Migration Path

1. Add Serilog packages
2. Initialise in `Program.Main`
3. Replace `Debug.WriteLine` with `Log.Debug/Information/Warning/Error`
4. Add context enrichment (GameId, SessionId)
5. Configure file sink with rotation
6. Add log viewing to debug menu (optional)