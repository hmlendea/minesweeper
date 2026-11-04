# Repository Pattern

## Overview

The repository layer provides data access abstraction for `Tile` entities using NuciDAL's `Repository<T>` base class with XML persistence.

## Base Repository (NuciDAL)

**Type:** `NuciDAL.Repositories.Repository<T>` (NuciDAL 3.1.0)

**Generic Constraints:** `T : ModelBase`

**Key Members:**

| Member | Type | Description |
|--------|------|-------------|
| `Get(string id)` | `T` | Retrieves by Id |
| `GetAll()` | `IEnumerable<T>` | Returns all entities |
| `Add(T entity)` | `void` | Adds entity |
| `Remove(string id)` | `bool` | Removes by Id |
| `Clear()` | `void` | Removes all entities |
| `Save()` | `void` | Persists to XML |
| `Load()` | `void` | Loads from XML |

**Internal Storage:** `Dictionary<string, T>` keyed by `ModelBase.Id`

**Persistence:** `NuciDAL.IO.XmlFileObject<T>` handles XML serialisation

## TileRepository

**Location:** `Minesweeper/Repositories/TileRepository.cs`

**Inheritance:** `Repository<Tile>`

**Namespace:** `Minesweeper.Repositories`

```csharp
public class TileRepository : Repository<Tile>
{
    public Tile Get(int x, int y)
    {
        string key = $"{x},{y}";
        return base.Get(key);
    }

    public IEnumerable<Tile> GetAll()
    {
        return base.GetAll();
    }

    public void Clear()
    {
        base.Clear();
    }
}
```

**Added Methods:**
- `Get(int x, int y)` — Coordinate-based lookup (converts to `"X,Y"` key)
- `GetAll()` — Exposes base `GetAll()` with explicit return type
- `Clear()` — Exposes base `Clear()`

## RepositoryException

**Location:** `Minesweeper/Repositories/RepositoryException.cs`

**Purpose:** Custom exception for repository operations

```csharp
public class RepositoryException : Exception
{
    public RepositoryException(string message) : base(message) { }
    public RepositoryException(string message, Exception inner) : base(message, inner) { }
}
```

**Usage:** Not currently thrown in `TileRepository`; reserved for future error handling.

## IGameLogicManager Interface

**Location:** `Minesweeper/GameLogic/GameManagers/IGameLogicManager.cs`

**Purpose:** Defines repository contract for game logic

```csharp
public interface IGameLogicManager
{
    Tile GetTile(int x, int y);
    IEnumerable<Tile> GetAllTiles();
    void ClearTile(int x, int y);
    void FlagTile(int x, int y);
    void NewGame();
}
```

**Note:** This interface is NOT implemented by `TileRepository`. It's implemented by `GameManager` which owns a `TileRepository`. The naming is misleading — it's a game logic interface, not a repository interface.

## Usage in GameManager

**Location:** `Minesweeper/GameLogic/GameManagers/GameManager.cs`

### Field

```csharp
private readonly TileRepository tileRepository = new TileRepository();
```

### Initialisation

```csharp
public GameManager()
{
    InitialiseTiles();
}

private void InitialiseTiles()
{
    for (int y = 0; y < TableSize; y++)
    {
        for (int x = 0; x < TableSize; x++)
        {
            Tile tile = new Tile { X = x, Y = y };
            tileRepository.Add(tile);
        }
    }
}
```

### Operations

| GameManager Method | Repository Call |
|--------------------|-----------------|
| `GetTile(x,y)` | `tileRepository.Get(x, y)` |
| `GetAllTiles()` | `tileRepository.GetAll()` |
| `ClearTile(x,y)` | `tileRepository.Get(x, y)` → mutates tile |
| `FlagTile(x,y)` | `tileRepository.Get(x, y)` → mutates tile |
| `NewGame()` | `tileRepository.Clear()` → `InitialiseTiles()` |
| `CalculateDangerLevels()` | Iterates `tileRepository.GetAll()` |
| `CheckWinCondition()` | Iterates `tileRepository.GetAll()` |
| `RevealAllMines()` | Iterates `tileRepository.GetAll()` |

## Persistence Behaviour

**Current State:** Repository persistence is NOT used.

- `TileRepository` inherits `Save()`/`Load()` from base but never calls them
- `GameManager.NewGame()` always creates fresh tiles
- No save/load game feature exists
- Settings persistence is separate (`SettingsManager`)

**XML Schema (if enabled):**
```xml
<Repository>
  <Items>
    <Tile>
      <X>7</X>
      <Y>12</Y>
      <DangerLevel>3</DangerLevel>
      <Flagged>false</Flagged>
      <Cleared>true</Cleared>
      <Mined>false</Mined>
    </Tile>
    <!-- 255 more tiles -->
  </Items>
</Repository>
```

## Unit Test Coverage

**Location:** `Minesweeper.UnitTests/Repositories/TileRepositoryTests.cs`

**Test Cases (11 tests):**
- `Get_ValidCoordinates_ReturnsTile`
- `Get_InvalidCoordinates_ReturnsNull`
- `GetAll_ReturnsAllTiles`
- `Clear_RemovesAllTiles`
- `Add_AddsTile`
- `Remove_ExistingTile_ReturnsTrue`
- `Remove_NonExistingTile_ReturnsFalse`
- `Get_AfterAdd_ReturnsAddedTile`
- `GetAll_AfterClear_ReturnsEmpty`
- `Repository_InheritsFromRepositoryBase`
- `TileRepository_IsRepositoryOfTile`

## Design Notes

1. **Thin Wrapper:** `TileRepository` adds only coordinate-based `Get`; all other functionality from NuciDAL base.

2. **No Interface:** `TileRepository` does not implement an interface; `GameManager` depends on concrete type.

3. **Single Instance:** `GameManager` owns one `TileRepository` for its lifetime; recreated on `NewGame()`.

4. **Thread Safety:** Not thread-safe; single-threaded game loop only.

5. **XML Serialisation:** Available via NuciDAL but unused; would require `Save()`/`Load()` calls in `GameManager`.