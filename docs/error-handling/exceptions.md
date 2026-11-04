# Exceptions

## Exception Hierarchy

```
System.Exception
├── RepositoryException (Minesweeper.Repositories)
└── Standard .NET exceptions
    ├── ArgumentException
    ├── ArgumentNullException
    ├── InvalidOperationException
    ├── FileNotFoundException
    ├── DirectoryNotFoundException
    ├── IOException
    └── XmlException
```

## RepositoryException

**Location:** `Minesweeper/Repositories/RepositoryException.cs`

```csharp
public class RepositoryException : Exception
{
    public RepositoryException(string message) : base(message) { }
    public RepositoryException(string message, Exception inner) : base(message, inner) { }
}
```

**Usage:** Currently NOT thrown anywhere in the codebase. Reserved for future repository error handling.

**Intended Scenarios:**
- XML serialisation/deserialisation failures
- File access errors during Save/Load
- Corrupted repository data

## Standard Exception Handling

### GameManager

**No Custom Exceptions:** All error conditions handled via return values or state flags.

| Scenario | Handling |
|----------|----------|
| Invalid coordinates | `GetTile` returns `null`; operations return early |
| Flag limit reached | `FlagTile` returns early, no exception |
| Double clear | `ClearTile` returns early if already cleared |
| Mine hit | Sets `Alive = false`, no exception |

### TileRepository

**Inherits from NuciDAL.Repository<T>:** Base class may throw:

| Exception | Cause |
|-----------|-------|
| `ArgumentNullException` | Null entity passed to `Add` |
| `ArgumentException` | Duplicate Id on `Add` |
| `IOException` | File access during `Save`/`Load` |
| `XmlException` | Malformed XML during `Load` |

**Current Usage:** `Save`/`Load` never called, so these don't occur in practice.

### SettingsManager

**XML Loading (LoadContent):**
```csharp
public void LoadContent()
{
    string file = ApplicationPaths.SettingsFile;
    if (File.Exists(file))
    {
        XmlFileObject<SettingsManager> loader = new XmlFileObject<SettingsManager>(file);
        SettingsManager loaded = loader.Load();  // Can throw XmlException, IOException
        // ...
    }
}
```

**Exceptions Not Caught:** Propagate to caller (`GameWindow.LoadContent`), would crash startup.

**XML Saving (SaveContent):**
```csharp
public void SaveContent()
{
    XmlFileObject<SettingsManager> saver = new XmlFileObject<SettingsManager>(ApplicationPaths.SettingsFile);
    saver.Save(this);  // Can throw IOException, UnauthorizedAccessException
}
```

**Exceptions Not Caught:** Propagate to caller (`Screen.DoUnloadContent`), lost silently.

### GameWindow

**Content Loading:**
```csharp
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
// Can throw ContentLoadException if .xnb missing/corrupt
```

**No Try-Catch:** Crashes on missing content.

## Exception Safety

### Current State

| Component | Try-Catch | Finally | Using |
|-----------|-----------|---------|-------|
| `Program.Main` | ❌ | ❌ | ❌ |
| `GameWindow.LoadContent` | ❌ | ❌ | ❌ |
| `SettingsManager.LoadContent` | ❌ | ❌ | ❌ |
| `SettingsManager.SaveContent` | ❌ | ❌ | ❌ |
| `GameManager` operations | ❌ | ❌ | ❌ |
| `TileRepository` operations | ❌ | ❌ | ❌ |

### Resource Disposal

| Resource | Disposed | Method |
|----------|----------|--------|
| `GraphicsDeviceManager` | ✅ | `Game.Dispose()` (base) |
| `ContentManager` | ✅ | `Game.Dispose()` (base) |
| `SpriteBatch` | ❌ | Not disposed (leaked) |
| `XmlFileObject` | N/A | No `IDisposable` |
| File handles | ✅ | `XmlFileObject` closes internally |

## Recommended Exception Handling

### SettingsManager.LoadContent (Robust)

```csharp
public void LoadContent()
{
    string file = ApplicationPaths.SettingsFile;
    try
    {
        if (File.Exists(file))
        {
            XmlFileObject<SettingsManager> loader = new XmlFileObject<SettingsManager>(file);
            SettingsManager loaded = loader.Load();
            AudioSettings = loaded.AudioSettings;
            GraphicsSettings = loaded.GraphicsSettings;
            DebugMode = loaded.DebugMode;
        }
    }
    catch (XmlException ex)
    {
        // Log error, use defaults
        Debug.WriteLine($"Settings XML corrupt: {ex.Message}");
    }
    catch (IOException ex)
    {
        // Log error, use defaults
        Debug.WriteLine($"Settings file access error: {ex.Message}");
    }
}
```

### SettingsManager.SaveContent (Robust)

```csharp
public void SaveContent()
{
    try
    {
        XmlFileObject<SettingsManager> saver = new XmlFileObject<SettingsManager>(ApplicationPaths.SettingsFile);
        saver.Save(this);
    }
    catch (IOException ex)
    {
        // Log error, don't crash
        Debug.WriteLine($"Failed to save settings: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Debug.WriteLine($"Settings save permission denied: {ex.Message}");
    }
}
```

### GameWindow.LoadContent (Robust)

```csharp
protected override void LoadContent()
{
    try
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        // ... singleton initialisation
        NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
        // ...
    }
    catch (ContentLoadException ex)
    {
        // Show error screen or exit gracefully
        Exit();
    }
}
```

## Error Reporting

**Current:** No user-facing error reporting. Exceptions crash the application.

**Planned:**
1. Global exception handler in `Program.Main`
2. Error screen for content load failures
3. Settings corruption recovery (delete file, use defaults)
4. Logging to `ApplicationPaths.LogsDirectory`

## Debugging Exceptions

**Visual Studio:** Enable "Break on all exceptions" (Ctrl+Alt+E)

**VS Code:** `launch.json` with `"stopOnEntry": true`

**Linux:** `coredumpctl` for post-mortem analysis