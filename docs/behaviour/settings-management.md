# Settings Management

User-facing settings, their effects, and management workflows.

## Settings Categories

### Audio Settings
**Location:** `Minesweeper/Settings/AudioSettings.cs`

| Setting | Type | Default | Effect |
|---------|------|---------|--------|
| `SoundEnabled` | `bool` | `true` | Master toggle for all sound effects |

**UI:** SettingsScreen → "Sound" checkbox

**Persistence:** Saved to `Settings.xml` on screen unload

**Runtime Application:** Checked before playing sounds (not yet implemented)

### Graphics Settings
**Location:** `Minesweeper/Settings/GraphicsSettings.cs`

| Setting | Type | Default | Effect |
|---------|------|---------|--------|
| `Fullscreen` | `bool` | `false` | Windowed vs borderless fullscreen |
| `Resolution` | `Size2D` | `640×700` | Window size (windowed) / display mode (fullscreen) |

**UI:** SettingsScreen → "Fullscreen" checkbox, resolution dropdown (not implemented)

**Persistence:** Saved to `Settings.xml` on screen unload

**Runtime Application:** `SettingsManager.ApplyGraphicsSettings()` called on load and change

**Known Issue:** `Resolution` not updated after fullscreen toggle

### Debug Settings
**Location:** `Minesweeper/Settings/SettingsManager.cs`

| Setting | Type | Default | Effect |
|---------|------|---------|--------|
| `DebugMode` | `bool` | `false` | Enables FPS counter (FpsIndicator) |

**UI:** SettingsScreen → "Debug Mode" checkbox

**Persistence:** Saved to `Settings.xml`

**Runtime Application:** `FpsIndicator.Visible = DebugMode` in `GameWindow.LoadContent`

## Settings Manager

**Location:** `Minesweeper/Settings/SettingsManager.cs`

**Pattern:** Thread-safe singleton with double-checked locking

### Public API

```csharp
// Access instance
var settings = SettingsManager.Instance;

// Load from disk (called at startup)
settings.Load();

// Save to disk (called on screen unload)
settings.Save();

// Apply graphics changes
settings.ApplyGraphicsSettings();

// Properties
settings.AudioSettings.SoundEnabled
settings.GraphicsSettings.Fullscreen
settings.GraphicsSettings.Resolution
settings.DebugMode
```

### Initialisation Order
1. `Program.PrepareFiles()` — ensures data directory exists
2. `GameWindow.LoadContent()` — calls `SettingsManager.Instance.Load()`
3. `SettingsManager.Load()` — deserialises XML or creates defaults
4. `SettingsManager.ApplyGraphicsSettings()` — applies to GraphicsDevice
5. `FpsIndicator.Visible = DebugMode` — shows/hides FPS counter

### Persistence Details

**File Location:**
- Linux: `~/.local/share/Minesweeper/Settings.xml`
- Windows: `%LocalAppData%\Minesweeper\Settings.xml`

**Format:** XML via `NuciDAL.IO.XmlFileObject<SettingsManager>`

**Example Output:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<SettingsManager xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <AudioSettings>
    <SoundEnabled>true</SoundEnabled>
  </AudioSettings>
  <GraphicsSettings>
    <Fullscreen>false</Fullscreen>
    <Resolution>
      <X>640</X>
      <Y>700</Y>
    </Resolution>
  </GraphicsSettings>
  <DebugMode>false</DebugMode>
</SettingsManager>
```

**Triggers:**
- Load: Once at startup
- Save: `GameplayScreen.DoUnloadContent()`, `SettingsScreen.DoUnloadContent()`

## Settings Screen

**Location:** `Minesweeper/Gui/Screens/SettingsScreen.cs`

### UI Elements
- Checkbox: "Sound Enabled" → `AudioSettings.SoundEnabled`
- Checkbox: "Fullscreen" → `GraphicsSettings.Fullscreen`
- Checkbox: "Debug Mode" → `DebugMode`
- Button: "Back" → Returns to TitleScreen

### Event Flow
```
User clicks checkbox
    → Property updated in SettingsManager
    → If graphics: ApplyGraphicsSettings() called immediately
    → User clicks Back
    → DoUnloadContent() → SettingsManager.Save()
    → ScreenManager.ChangeScreens<TitleScreen>()
```

## Application Paths

**Location:** `Minesweeper/Settings/ApplicationPaths.cs`

### Responsibilities
- Resolve user data directory per platform
- Ensure directory exists
- Provide base path for settings, logs, future saves

### Implementation
```csharp
public static string UserDataPath
{
    get
    {
        if (OperatingSystem.IsWindows())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Minesweeper");
        else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "Minesweeper");
    }
}

public static void EnsureDirectories()
{
    Directory.CreateDirectory(UserDataPath);
    Directory.CreateDirectory(Path.Combine(UserDataPath, "Logs"));
}
```

### Called From
- `Program.PrepareFiles()` — before `GameWindow.Run()`

## Settings Workflow Diagram

```
┌─────────────────┐
│   Program.cs    │
│ PrepareFiles()  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ GameWindow.Load │
│   Content()     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│SettingsManager. │
│    Load()       │
│  (XML or default)│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ApplyGraphics    │
│Settings()       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     User opens      ┌─────────────────┐
│  TitleScreen    │──── SettingsScreen │  SettingsScreen │
└────────┬────────┘                     └────────┬────────┘
         │                                       │
         │ User changes settings                 │
         │ (immediate ApplyGraphics for graphics)│
         ▼                                       ▼
┌─────────────────┐                     ┌─────────────────┐
│  GameplayScreen │                     │  DoUnloadContent│
│  (or other)     │                     │  SettingsManager│
└────────┬────────┘                     │     .Save()     │
         │                              └────────┬────────┘
         │                                       │
         ▼                                       ▼
┌─────────────────┐                     ┌─────────────────┐
│ DoUnloadContent │                     │  Settings.xml   │
│ SettingsManager │                     │   (persisted)   │
│    .Save()      │                     └─────────────────┘
└─────────────────┘
```

## Adding New Settings

See `change-guide.md#adding-a-new-setting` for complete procedure.

### Quick Checklist
1. Add property to `AudioSettings` or `GraphicsSettings` (or new class)
2. Expose in `SettingsManager`
3. Add UI in `SettingsScreen.DoLoadContent()`
4. Apply at runtime if graphics-related
5. Test: change → back → restart → verify persisted

## Known Issues

| Issue | Impact | Workaround |
|-------|--------|------------|
| Resolution not synced | Fullscreen resolution not saved | Manual fix in `ApplyGraphicsSettings()` |
| No VSync setting | Cannot control frame pacing | Add `VSync` to `GraphicsSettings` |
| No resolution dropdown | Fixed 640×700 windowed | Implement dropdown with supported modes |
| Settings exceptions unhandled | Corrupt XML crashes game | Wrap Load/Save in try-catch |
| No sound effects implemented | SoundEnabled has no effect | Add audio content and playback |