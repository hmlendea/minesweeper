# Design Decisions

Key architectural decisions and rationale for the Minesweeper codebase.

## ADR-001: Layered Architecture with Screen-Based UI

**Status:** Accepted

**Context:** Need to organise a MonoGame desktop application with clear separation of concerns.

**Decision:** Use three-layer architecture:
- **Presentation:** `GameWindow`, `ScreenManager`, screens, GUI controls
- **Game Logic:** `GameManager`, `IGameManager`, `IGameLogicManager`
- **Infrastructure:** `TileRepository`, `SettingsManager`, NuciXNA services

UI uses NuciXNA's screen-based pattern (`ScreenManager.ChangeScreens<T>`).

**Consequences:**
- ✅ Clear boundaries, testable logic layer
- ✅ NuciXNA integration natural
- ⚠️ `GameplayScreen` directly instantiates `GameManager` (violates interface boundary)
- ⚠️ No dependency injection container

---

## ADR-002: MonoGame.Framework.DesktopGL for Cross-Platform

**Status:** Accepted

**Context:** Target Linux primarily, with Windows/macOS support.

**Decision:** Use `MonoGame.Framework.DesktopGL` (OpenGL + SDL2) instead of WindowsDX or other backends.

**Consequences:**
- ✅ Single codebase for all platforms
- ✅ Mature, stable framework
- ⚠️ Requires SDL2/OpenAL/FreeType on Linux
- ⚠️ No DirectX 12 features

---

## ADR-003: NuciXNA Ecosystem for UI and Data Access

**Status:** Accepted

**Context:** Need screen management, GUI controls, content management, and repository pattern.

**Decision:** Adopt NuciXNA packages (Gui, Graphics, Input, Primitives, DataAccess) and NuciDAL.

**Consequences:**
- ✅ Cohesive, well-integrated libraries
- ✅ Screen management, GUI, repositories included
- ⚠️ Single author (hmlendea) — bus factor
- ⚠️ Version lockstep required
- ⚠️ Limited community/documentation

---

## ADR-004: XML Serialisation via NuciDAL XmlFileObject

**Status:** Accepted

**Context:** Need simple persistence for settings and future game state.

**Decision:** Use `NuciDAL.IO.XmlFileObject<T>` for XML serialisation of `SettingsManager` and `Repository<T>`.

**Consequences:**
- ✅ Zero-config, attribute-free serialisation
- ✅ Works with `ModelBase` entities
- ⚠️ No schema validation
- ⚠️ Versioning not built-in
- ⚠️ Performance OK for small files only

---

## ADR-005: First-Click Mine Placement for Fairness

**Status:** Accepted

**Context:** Classic Minesweeper guarantees first click is never a mine.

**Decision:** Defer mine placement until first `ClearTile` call; exclude clicked tile and its 8 neighbours.

**Consequences:**
- ✅ Fair gameplay, matches player expectations
- ✅ Simple implementation
- ⚠️ Requires `IsRunning` flag to track first click
- ⚠️ Mine placement uses `new Random()` (not seeded for testing)

---

## ADR-006: Recursive Flood-Fill for Zero Tiles

**Status:** Accepted

**Context:** Clearing a zero-danger tile should reveal connected region.

**Decision:** Use recursive `FloodFillClear` in `GameManager`.

**Consequences:**
- ✅ Simple, correct for 16×16 grid (max depth 256)
- ⚠️ Stack overflow risk on larger grids
- ⚠️ Not iterative (harder to interrupt/animate)

---

## ADR-007: Singleton Pattern for Infrastructure Services

**Status:** Accepted

**Context:** Need global access to graphics, content, settings, screens, input.

**Decision:** All infrastructure services use singleton pattern with double-checked locking (`Lock` in C# 12).

**Consequences:**
- ✅ Easy access from anywhere
- ✅ Lazy initialisation
- ⚠️ Hidden dependencies, hard to test
- ⚠️ Global state coupling
- ⚠️ `GameplayScreen` creates `GameManager` directly (not singleton)

---

## ADR-008: Compile-Time Constants in GameDefines

**Status:** Accepted

**Context:** Board size, mine count, tile size, window dimensions.

**Decision:** Use `const` fields in `GameDefines` class.

**Consequences:**
- ✅ Compile-time optimisation
- ✅ Single source of truth
- ⚠️ Requires recompile to change
- ⚠️ Not user-configurable
- ⚠️ Tight coupling (many classes reference `GameDefines`)

---

## ADR-009: No Dependency Injection Container

**Status:** Accepted

**Context:** Small application, manual composition in `GameWindow.LoadContent`.

**Decision:** Manual singleton initialisation and direct instantiation; no DI container.

**Consequences:**
- ✅ Zero dependencies, simple
- ✅ Explicit composition root
- ⚠️ Hard to substitute implementations for testing
- ⚠️ `GameManager` not injected into `GameplayScreen`

---

## ADR-010: NUnit for Unit Testing

**Status:** Accepted

**Context:** Need unit test framework for .NET 10.0.

**Decision:** Use NUnit 4.x with NUnit3TestAdapter.

**Consequences:**
- ✅ Mature, feature-rich
- ✅ VS Test Explorer integration
- ✅ Good constraint syntax (`Assert.That`)
- ⚠️ Moq referenced but unused
- ⚠️ No integration test project yet