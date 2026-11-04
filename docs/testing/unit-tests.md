# Unit Tests

## Overview

Unit tests use NUnit 4.x and are organised by module mirroring the source structure. Tests run in CI via `dotnet test`.

## Test Project Structure

```
Minesweeper.UnitTests/
├── Minesweeper.UnitTests.csproj
├── GameLogic/
│   └── GameManagers/
│       └── GameManagerTests.cs
├── Models/
│   └── TileTests.cs
└── Repositories/
    └── TileRepositoryTests.cs
```

## Test Conventions

- **Naming:** `MethodName_Scenario_ExpectedResult`
- **Framework:** NUnit 4.2.2
- **Assertions:** `Assert.That` with constraints
- **Setup:** `[SetUp]` per test class
- **No Mocking:** Tests use real implementations (Moq referenced but unused)

## GameManagerTests

**Location:** `Minesweeper.UnitTests/GameLogic/GameManagers/GameManagerTests.cs`

**Test Count:** 13 tests

### Test Cases

| Test | Purpose |
|------|---------|
| `NewGame_InitialisesCorrectly` | Verifies initial state after construction |
| `ClearTile_FirstClick_PlacesMinesAndSetsRunning` | First click safety, mine placement |
| `ClearTile_OnMine_SetsAliveFalse` | Mine hit → game over |
| `ClearTile_OnClearedTile_DoesNothing` | Idempotent clear |
| `ClearTile_OnFlaggedTile_DoesNothing` | Flagged tiles cannot be cleared |
| `ClearTile_ZeroDanger_FloodFills` | Recursive clear of adjacent zeros |
| `FlagTile_TogglesFlag` | Flag/unflag cycle |
| `FlagTile_OnClearedTile_DoesNothing` | Cleared tiles cannot be flagged |
| `FlagTile_RespectsLimit` | Max 24 flags |
| `NewGame_ResetsState` | Full state reset |
| `GetTile_ValidCoordinates_ReturnsTile` | Coordinate lookup |
| `GetTile_InvalidCoordinates_ReturnsNull` | Bounds checking |
| `GetAllTiles_ReturnsAllTiles` | Enumeration |

### Key Test Patterns

**First-Click Safety:**
```csharp
[Test]
public void ClearTile_FirstClick_PlacesMinesAndSetsRunning()
{
    game.ClearTile(8, 8);

    Assert.That(game.IsRunning, Is.True);
    Assert.That(game.Alive, Is.True);

    int mineCount = 0;
    foreach (Tile tile in game.GetAllTiles())
    {
        if (tile.Mined) mineCount++;
    }
    Assert.That(mineCount, Is.EqualTo(24));

    // First-click tile and neighbours not mined
    for (int dy = -1; dy <= 1; dy++)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            Tile tile = game.GetTile(8 + dx, 8 + dy);
            Assert.That(tile.Mined, Is.False);
        }
    }
}
```

**Flood-Fill Verification:**
```csharp
[Test]
public void ClearTile_ZeroDanger_FloodFills()
{
    // Arrange: create game with known zero at (0,0)
    // Act: clear (0,0)
    // Assert: all connected zeros and their borders cleared
}
```

## TileTests

**Location:** `Minesweeper.UnitTests/Models/TileTests.cs`

**Test Count:** 6 tests

### Test Cases

| Test | Purpose |
|------|---------|
| `Id_ReturnsCorrectFormat` | `"X,Y"` format |
| `Equals_SameCoordinates_ReturnsTrue` | Position-based equality |
| `Equals_DifferentCoordinates_ReturnsFalse` | Different positions not equal |
| `GetHashCode_ConsistentWithEquals` | Hash code contract |
| `DefaultValues_AreCorrect` | All defaults false/0 |
| `PropertyAssignment_Works` | Setters functional |

### Example

```csharp
[Test]
public void Id_ReturnsCorrectFormat()
{
    Tile tile = new Tile { X = 7, Y = 12 };
    Assert.That(tile.Id, Is.EqualTo("7,12"));
}

[Test]
public void Equals_SameCoordinates_ReturnsTrue()
{
    Tile a = new Tile { X = 5, Y = 10 };
    Tile b = new Tile { X = 5, Y = 10 };
    Assert.That(a.Equals(b), Is.True);
}
```

## TileRepositoryTests

**Location:** `Minesweeper.UnitTests/Repositories/TileRepositoryTests.cs`

**Test Count:** 11 tests

### Test Cases

| Test | Purpose |
|------|---------|
| `Get_ValidCoordinates_ReturnsTile` | Coordinate lookup |
| `Get_InvalidCoordinates_ReturnsNull` | Out of bounds |
| `GetAll_ReturnsAllTiles` | Full enumeration |
| `Clear_RemovesAllTiles` | Repository reset |
| `Add_AddsTile` | Insertion |
| `Remove_ExistingTile_ReturnsTrue` | Deletion success |
| `Remove_NonExistingTile_ReturnsFalse` | Deletion miss |
| `Get_AfterAdd_ReturnsAddedTile` | Round-trip |
| `GetAll_AfterClear_ReturnsEmpty` | Post-clear state |
| `Repository_InheritsFromRepositoryBase` | Type hierarchy |
| `TileRepository_IsRepositoryOfTile` | Generic constraint |

### Example

```csharp
[Test]
public void Get_ValidCoordinates_ReturnsTile()
{
    TileRepository repo = new TileRepository();
    repo.Add(new Tile { X = 3, Y = 4 });

    Tile result = repo.Get(3, 4);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.X, Is.EqualTo(3));
    Assert.That(result.Y, Is.EqualTo(4));
}
```

## Running Tests

### Local

```bash
# All tests
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj

# With coverage
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj --collect:"XPlat Code Coverage"

# Filter by class
dotnet test --filter "FullyQualifiedName~GameManagerTests"
```

### CI

**Workflow:** `.github/workflows/dotnet.yml`

```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

## Coverage

**Current:** Not measured in CI (coverlet referenced but not configured)

**Target Areas:**
- `GameManager` — High priority (core logic)
- `TileRepository` — Medium priority (data access)
- `Tile` — Low priority (simple entity)

## Known Gaps

1. **No Integration Tests:** Screen transitions, input handling, rendering not tested
2. **No UI Tests:** `GuiGameBoard`, `GuiInfoBar`, `GameplayScreen` untested
3. **Timer Bug Untested:** `GameManager.Update()` never called — no test catches this
4. **Settings Persistence:** `SettingsManager` load/save not tested
5. **Edge Cases:** Large board sizes, rapid clicking, concurrent input not tested

## Test Design Principles

1. **Fast:** All tests run in < 1 second
2. **Isolated:** Each test creates fresh `GameManager`/`TileRepository`
3. **Deterministic:** No random seeds controlled (mine placement uses `new Random()`)
4. **Readable:** Names describe scenario and expectation