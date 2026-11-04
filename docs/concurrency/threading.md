# Threading Model

## Overview

The application is **strictly single-threaded**. All game logic, rendering, and input handling run on the main thread (MonoGame's game loop thread).

## Thread Affinity

| Component | Thread | Notes |
|-----------|--------|-------|
| `Program.Main` | Main | Entry point |
| `GameWindow` (Update/Draw) | Main | MonoGame loop |
| `ScreenManager` | Main | Called from GameWindow |
| `GameplayScreen` | Main | Called from ScreenManager |
| `GameManager` | Main | Called from GameplayScreen |
| `TileRepository` | Main | Called from GameManager |
| `GuiManager` | Main | Called from ScreenManager |
| `InputManager` | Main | Called from GameWindow |
| `SettingsManager` | Main | Called from GameWindow/Screens |
| `FpsIndicator` | Main | Called from GameWindow |

## No Background Threads

- ❌ No `Task.Run`
- ❌ No `ThreadPool`
- ❌ No `BackgroundWorker`
- ❌ No `async`/`await` in game loop
- ❌ No parallel processing

## Singleton Thread Safety

**Only singleton initialisation uses locking:**

```csharp
// SettingsManager.Instance (and all NuciXNA singletons)
static volatile SettingsManager instance;
static readonly Lock syncRoot = new();

public static SettingsManager Instance
{
    get
    {
        if (instance is null)
        {
            lock (syncRoot)
            {
                instance ??= new SettingsManager();
            }
        }
        return instance;
    }
}
```

**Double-checked locking** with `Lock` (C# 12) ensures:
- Single initialisation
- Memory visibility across threads (theoretical — only main thread accesses)
- No performance penalty after initialisation

## Why Single-Threaded?

1. **MonoGame Design:** `Game.Update`/`Draw` run on one thread
2. **Graphics Device:** Not thread-safe; must be accessed from creator thread
3. **Simplicity:** No race conditions, no locks in hot path
4. **Determinism:** Frame-perfect reproducibility

## Potential Threading (Future)

| Feature | Threading Need | Approach |
|---------|----------------|----------|
| Asset streaming | Background load | `Task` + `GraphicsDevice` context |
| Network multiplayer | Network I/O | Dedicated thread + message queue |
| Auto-save | File I/O | `Task.Run` (non-blocking) |
| AI opponent | Computation | `Task.Run` or job system |

## Current Concurrency Risks

**None in practice** — single-threaded execution eliminates:
- Race conditions
- Deadlocks
- Memory visibility issues
- Lock contention

## Verification

**Static Analysis:** No `Thread`, `Task`, `Parallel`, `async` in game assembly (except `Program.Main` which is synchronous).

**Runtime:** `Thread.CurrentThread.ManagedThreadId` constant throughout execution.