# Minesweeper Documentation

Master index and navigation for the Minesweeper documentation.

## Repository Summary

Minesweeper is a modern, open-source reimplementation of the classic puzzle game built with .NET 10.0 and MonoGame 3.8.4. It is a self-contained desktop application for Linux (cross-platform capable via DesktopGL) that runs locally without network connectivity. The architecture separates game logic from presentation using a screen-based UI framework (NuciXNA ScreenManager) and persists only user preferences to a local XML file.

## Root Document Map

- [ARCHITECTURE.md](../ARCHITECTURE.md) — High-level architecture overview
- [SECURITY.md](../SECURITY.md) — Security policy and vulnerability reporting
- [PRIVACY.md](../PRIVACY.md) — Privacy and personal data handling
- [LICENSE](../LICENSE) — GNU General Public License v3.0
- [ROADMAP.md](../ROADMAP.md) — Project roadmap (if present)
- [CONTRIBUTING.md](../CONTRIBUTING.md) — Contribution guidelines
- [CODE_OF_CONDUCT.md](../CODE_OF_CONDUCT.md) — Community code of conduct

## Documentation Catalogue

### Getting Started
- [quick-start.md](./quick-start.md) — 5-minute setup: prerequisites, build, run, play
- [repository-overview.md](./repository-overview.md) — Purpose, scope, entry points, capabilities
- [faq.md](./faq.md) — Frequently asked questions

### Architecture & Design
- [architecture.md](./architecture.md) — Layered architecture, dependency direction, runtime flow
- [repository-structure.md](./repository-structure.md) — Source tree, module organisation, project config
- [components/presentation.md](./components/presentation.md) — Presentation layer: GameWindow, screens, controls, FPS indicator
- [components/host-and-composition.md](./components/host-and-composition.md) — GameWindow as composition root, singleton initialisation order
- [components/application-services.md](./components/application-services.md) — SettingsManager, ApplicationPaths, NuciXNA singletons
- [design-decisions/adr.md](./design-decisions/adr.md) — 10 Architecture Decision Records
- [design-decisions.md](./design-decisions.md) — Key architectural decisions (root-level convenience copy of ADR content)
- [invariants.md](./invariants.md) — System-wide invariants and contracts

### Data & State
- [data-model.md](./data-model.md) — Domain entities, relationships, persistence overview
- [data-model/tile-entity.md](./data-model/tile-entity.md) — Tile class, properties, state transitions, serialisation
- [data-model/repository-pattern.md](./data-model/repository-pattern.md) — TileRepository, NuciDAL base, IGameLogicManager
- [data-model/game-state.md](./data-model/game-state.md) — Distributed state, transitions, timer bug
- [state-persistence/save-load.md](./state-persistence/save-load.md) — Game save feature (current state, required implementation)
- [state-persistence/settings-persistence.md](./state-persistence/settings-persistence.md) — SettingsManager persistence, file location, format, triggers

### Execution Flows
- [flows/startup-and-rendering.md](./flows/startup-and-rendering.md) — Program.Main → GameWindow.LoadContent → first frame
- [flows/gameplay-loop.md](./flows/gameplay-loop.md) — GameplayScreen.Update → GameManager → TileRepository → rendering
- [flows/screen-transitions.md](./flows/screen-transitions.md) — ScreenManager.ChangeScreens<T> flow, Splash→Title→Gameplay→GameOver/Victory→Title

### User-Facing Behaviours
- [behaviour/playing-the-game.md](./behaviour/playing-the-game.md) — Gameplay mechanics, controls, strategy, screen transitions
- [behaviour/settings-management.md](./behaviour/settings-management.md) — Settings categories, persistence, UI workflow

### Configuration & Dependencies
- [configuration/settings.md](./configuration/settings.md) — SettingsManager, AudioSettings, GraphicsSettings, XML persistence
- [configuration/build.md](./configuration/build.md) — csproj configs, content build, CI pipeline, runtime dependencies
- [configuration/dependencies.md](./configuration/dependencies.md) — NuGet packages, dependency graph, NuciXNA ecosystem, system deps, licenses

### Testing
- [testing/unit-tests.md](./testing/unit-tests.md) — 30 tests, conventions, GameManagerTests, TileTests, TileRepositoryTests, gaps
- [testing/integration-tests.md](./testing/integration-tests.md) — Planned scenarios, infrastructure needs, CI integration
- [testing/test-strategy.md](./testing/test-strategy.md) — Testing pyramid, coverage goals, CI, known gaps, recommendations

### Build & Deployment
- [build-and-deployment/build-instructions.md](./build-and-deployment/build-instructions.md) — Prerequisites, local build, content build, running, publishing, troubleshooting
- [build-and-deployment/deployment.md](./build-and-deployment/deployment.md) — Targets, self-contained/framework-dependent, content deployment, versioning

### Error Handling & Logging
- [error-handling/exceptions.md](./error-handling/exceptions.md) — Exception hierarchy, RepositoryException, standard handling, improvements
- [error-handling/logging.md](./error-handling/logging.md) — Current state, recommended Serilog setup, log categories, migration path

### Security
- [security/security.md](./security/security.md) — Threat model, attack surface, data protection, input validation, dependency security, compliance

### Concurrency
- [concurrency/threading.md](./concurrency/threading.md) — Single-threaded model, thread affinity, singleton locking, future needs

### Operations & Maintenance
- [change-guide.md](./change-guide.md) — Procedures for common modifications (screens, board size, settings, tile types, difficulty, sounds, localisation, tests, dependencies, CI)
- [troubleshooting.md](./troubleshooting.md) — Build, runtime, test, performance, platform-specific issues; diagnostics
- [documentation-maintenance.md](./documentation-maintenance.md) — Structure, update triggers, review checklist, style, automation
- [ambiguities-and-open-questions.md](./ambiguities-and-open-questions.md) — 33 known unknowns and areas requiring clarification

### API Reference
- [api-reference/INDEX.md](./api-reference/INDEX.md) — Namespaces, interfaces, classes, screens, controls, settings, NuciXNA services, content assets

## Navigation Aids

- **Newcomers:** [quick-start.md](./quick-start.md) → [repository-overview.md](./repository-overview.md) → [architecture.md](./architecture.md) → [flows/startup-and-rendering.md](./flows/startup-and-rendering.md)
- **Component owners:** [components/](./components/) → [data-model/](./data-model/) → [design-decisions/adr.md](./design-decisions/adr.md) → [invariants.md](./invariants.md)
- **Debuggers:** [flows/](./flows/) → [error-handling/](./error-handling/) → [troubleshooting.md](./troubleshooting.md) → [ambiguities-and-open-questions.md](./ambiguities-and-open-questions.md)
- **Contributors:** [change-guide.md](./change-guide.md) → [testing/](./testing/) → [configuration/build.md](./configuration/build.md) → [documentation-maintenance.md](./documentation-maintenance.md)
- **Operators:** [build-and-deployment/](./build-and-deployment/) → [security/security.md](./security/security.md) → [concurrency/threading.md](./concurrency/threading.md)

## Maintenance Metadata

- Last reviewed: 2026-11-04
- Owner: Repository maintainers
- Coverage status: Complete — all 41 files generated and verified
- Known duplication: `design-decisions.md` (root) mirrors `design-decisions/adr.md` for INDEX compatibility