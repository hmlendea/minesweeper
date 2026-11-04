# Test Strategy

## Testing Pyramid

```
        /\
       /  \  E2E / Integration (0% - Not implemented)
      /----\
     /      \  Unit Tests (100% - 30 tests)
    /--------\
```

## Current Coverage

| Layer | Tests | Status |
|-------|-------|--------|
| Unit | 30 | ✅ Implemented |
| Integration | 0 | ❌ Not implemented |
| E2E | 0 | ❌ Not implemented |

## Unit Test Strategy

**Scope:** Pure logic classes with no external dependencies

**Target Classes:**
- `GameManager` — Core game logic (13 tests)
- `Tile` — Domain entity (6 tests)
- `TileRepository` — Data access (11 tests)

**Principles:**
- Fast execution (< 1s total)
- Deterministic (no random seeds controlled)
- Isolated (fresh instances per test)
- Readable names (`Method_Scenario_Expected`)

**Tools:** NUnit 4.x, `Assert.That` constraints

## Integration Test Strategy (Planned)

**Scope:** Cross-component interactions

**Target Areas:**
1. Screen transitions (ScreenManager + Screens)
2. Input → GameManager flow (InputManager → GameplayScreen → GameManager)
3. Settings persistence (SettingsScreen → SettingsManager → XML)
4. Rendering pipeline (GuiManager → Controls → SpriteBatch)

**Infrastructure Needed:**
- Headless GraphicsDevice (MonoGame.Extended.Testing or Xvfb)
- Input simulation helpers
- Screen transition verification

**Tools:** NUnit, same as unit tests

## E2E Test Strategy (Future)

**Scope:** Full application flows

**Target Scenarios:**
- Complete game: Start → Play → Win/Loss → Restart
- Settings change: Toggle fullscreen → Persist → Reload
- Accessibility: Keyboard-only navigation

**Tools:** Could use Playwright for desktop or custom MonoGame test host

## Test Data Management

**Current:** Inline test data (no fixtures)

**Planned:**
- `TestData/` folder with predefined board states
- Deterministic mine layouts for reproducible tests
- JSON/XML test cases for settings

## Continuous Integration

**Current Pipeline (`.github/workflows/dotnet.yml`):**
```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

**Runs on:** Every push/PR to `master`

**Environment:** Ubuntu latest, .NET 10.0

## Coverage Goals

| Metric | Current | Target |
|--------|---------|--------|
| Line Coverage | Unknown | ≥ 80% |
| Branch Coverage | Unknown | ≥ 70% |
| Critical Path Coverage | Partial | 100% |

**Critical Paths:**
1. Mine placement (first-click safety)
2. Flood-fill clearing
3. Win/loss detection
4. Flag limit enforcement
5. Screen transitions

## Test Execution

### Local Development

```bash
# Watch mode (requires dotnet-watch)
dotnet watch test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj

# Single test class
dotnet test --filter "FullyQualifiedName~GameManagerTests"

# Verbose output
dotnet test --verbosity detailed
```

### Debugging Tests

1. Set breakpoint in test
2. `dotnet test --no-build` with debugger attached
3. Or use VS Code Test Explorer (NUnit3TestAdapter)

## Known Test Gaps

1. **Timer Bug:** `GameManager.Update()` never called — no test detects this
2. **Randomness:** Mine placement uses `new Random()` — not seeded for reproducibility
3. **Settings:** `SettingsManager` load/save untested
4. **UI Controls:** `GuiGameBoard`, `GuiInfoBar` rendering untested
5. **Screen Lifecycle:** `DoLoadContent`/`DoUnloadContent` untested
6. **Input Events:** `InputManager` event wiring untested

## Recommendations

1. **Add Integration Test Project:** `Minesweeper.IntegrationTests`
2. **Seed Random:** Allow `GameManager` to accept `Random` for testing
3. **Extract Timer:** Make `GameManager.Update` callable/testable
4. **Test Settings:** Add `SettingsManagerTests`
5. **CI Coverage:** Add coverlet + Codecov/Codecov action