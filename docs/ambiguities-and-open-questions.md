# Ambiguities and Open Questions

Known unknowns, undocumented behaviours, and areas requiring clarification.

## Game Logic

### 1. Timer Never Updates
**Question:** Is `GameManager.Update(gameTime)` intentionally not called?
**Evidence:** `GameplayScreen.Update()` does not call `_gameManager.Update(gameTime)`
**Impact:** Timer stays at 00:00 forever
**Resolution needed:** Confirm intended behaviour; fix or document as known limitation

### 2. First-Click Safety Scope
**Question:** Does "first click safe" include only the clicked tile, or the 3×3 area?
**Evidence:** Code excludes 3×3 area (clicked tile + 8 neighbours)
**Impact:** 9 tiles guaranteed safe on first click
**Resolution needed:** Confirm this matches intended design (standard Minesweeper uses 3×3)

### 3. Flood-Fill Recursion Depth
**Question:** Is recursive flood-fill acceptable for 16×16 board?
**Evidence:** `ClearTile` uses recursive calls for zero-danger tiles
**Impact:** Stack overflow risk on larger boards (not an issue at 16×16)
**Resolution needed:** Document as acceptable or convert to iterative

### 4. Flag Limit Enforcement
**Question:** Should flags be limited to mine count (24)?
**Evidence:** `FlagTile` checks `FlagsPlaced < FlagsLimit`
**Impact:** Player cannot flag more than 24 tiles
**Resolution needed:** Confirm intended (standard Minesweeper allows unlimited flags)

### 5. Win Condition: Flags Required?
**Question:** Is victory possible without flagging all mines?
**Evidence:** Win check is `clearedCount == TableSize² - MinesCount` (flags not checked)
**Impact:** Player can win without placing any flags
**Resolution needed:** Confirm intended (standard Minesweeper allows this)

## Rendering

### 6. Grid Line Colour
**Question:** What colour are grid lines between tiles?
**Evidence:** `GuiGameBoard.Draw()` draws 1px lines; colour not documented
**Impact:** Visual appearance unclear
**Resolution needed:** Verify in code or screenshot

### 7. Danger Number Colours
**Question:** Are number colours standard (1=blue, 2=green, 3=red, etc.)?
**Evidence:** `GuiGameBoard.Draw()` uses colour per danger level
**Impact:** Visual consistency with classic Minesweeper
**Resolution needed:** Verify colour mapping matches standard

### 8. Mine Reveal on Loss
**Question:** Are all mines revealed on loss, or only the clicked one?
**Evidence:** `GuiGameBoard.Draw()` shows mine texture when `!Alive`
**Impact:** All mines visible after loss
**Resolution needed:** Confirm intended behaviour

### 9. Flag Display on Loss
**Question:** Are incorrect flags marked on loss?
**Evidence:** Not documented; code shows flags on uncleared tiles
**Impact:** Player cannot see which flags were wrong
**Resolution needed:** Check if standard Minesweeper behaviour (X over wrong flags) is intended

## Settings

### 10. Resolution Setting Not Synced
**Question:** Should `GraphicsSettings.Resolution` update after fullscreen toggle?
**Evidence:** `ApplyGraphicsSettings()` changes GraphicsDevice but not the setting
**Impact:** Saved resolution doesn't match actual display
**Resolution needed:** Fix or document as known limitation

### 11. Supported Resolutions
**Question:** What resolutions are supported in fullscreen?
**Evidence:** `GraphicsSettings.Resolution` is `Size2D`; no enumeration of modes
**Impact:** UI cannot offer resolution dropdown
**Resolution needed:** Query `GraphicsAdapter.SupportedDisplayModes` and populate dropdown

### 12. Settings Save Timing
**Question:** Should settings save on change or on screen exit?
**Evidence:** `Save()` called in `DoUnloadContent()` (on exit)
**Impact:** Settings lost if application crashes mid-screen
**Resolution needed:** Consider save-on-change or explicit Save button

### 13. Settings Exception Handling
**Question:** What should happen if Settings.xml is corrupt?
**Evidence:** `Load()` throws; no try-catch in `GameWindow.LoadContent()`
**Impact:** Application crashes on corrupt settings
**Resolution needed:** Add try-catch with fallback to defaults

## Architecture

### 14. Singleton Lifetime
**Question:** Are NuciXNA singletons (GraphicsManager, ScreenManager, etc.) disposed on exit?
**Evidence:** No `Dispose()` calls in `GameWindow.UnloadContent()`
**Impact:** Potential resource leaks on shutdown
**Resolution needed:** Verify NuciXNA handles disposal or add explicit cleanup

### 15. Screen Registration Order
**Question:** Does registration order matter?
**Evidence:** `GameWindow.LoadContent()` registers SplashScreen, TitleScreen, GameplayScreen, GameOverScreen, VictoryScreen, SettingsScreen
**Impact:** Unknown if order affects behaviour
**Resolution needed:** Check NuciXNA ScreenManager implementation

### 16. Content Manager Scope
**Question:** Is `NuciContentManager` a singleton or per-screen?
**Evidence:** Used as `NuciContentManager.Instance` (singleton)
**Impact:** Content loaded once, shared across screens
**Resolution needed:** Confirm no content unloading needed

## Testing

### 17. Integration Test Scope
**Question:** Should integration tests use real MonoGame or mocks?
**Evidence:** `docs/testing/integration-tests.md` proposes scenarios but no implementation
**Impact:** Unknown test infrastructure requirements
**Resolution needed:** Decide on approach (headless MonoGame vs. mocks)

### 18. UI Test Feasibility
**Question:** Can screens be tested without a display?
**Evidence:** MonoGame requires GraphicsDevice; headless testing unclear
**Impact:** UI logic untestable in CI
**Resolution needed:** Investigate MonoGame headless mode or extract logic

### 19. Test Coverage Target
**Question:** What coverage percentage is acceptable?
**Evidence:** `docs/testing/test-strategy.md` proposes goals but no enforced threshold
**Impact:** Coverage may regress
**Resolution needed:** Set explicit threshold in CI

## Deployment

### 20. Target Platforms
**Question:** Is Windows a supported target or just Linux?
**Evidence:** `WinExe` OutputType, DesktopGL framework, CI on Ubuntu only
**Impact:** Unclear release targets
**Resolution needed:** Document supported platforms explicitly

### 21. Distribution Method
**Question:** How is the game distributed?
**Evidence:** No release workflow, no installer, no packaging
**Impact:** Users must build from source
**Resolution needed:** Decide on distribution (GitHub Releases, Steam, itch.io, etc.)

### 22. Version Numbering
**Question:** What version scheme is used?
**Evidence:** `.csproj` has no `<Version>` property; assembly version defaults
**Impact:** No version tracking
**Resolution needed:** Add version to `.csproj` and document scheme

### 23. Content Pipeline in CI
**Question:** Is MGCB content build reliable in CI?
**Evidence:** `.github/workflows/dotnet.yml` installs MGCB tool and builds content
**Impact:** CI may fail if MGCB has issues
**Resolution needed:** Monitor CI; consider pre-built content fallback

## Security

### 24. Settings.xml Path Traversal
**Question:** Can malicious settings file cause harm?
**Evidence:** XML deserialisation of `SettingsManager` type
**Impact:** Potential XML deserialisation attacks (unlikely but possible)
**Resolution needed:** Validate XML schema or use safer format (JSON)

### 25. Log File Location
**Question:** Where should logs be written?
**Evidence:** `ApplicationPaths` creates `Logs/` directory; no logging implemented
**Impact:** No diagnostics available
**Resolution needed:** Implement logging (see `error-handling/logging.md`)

## Future Features

### 26. Save/Load Game
**Question:** Is game state persistence planned?
**Evidence:** `docs/state-persistence/save-load.md` documents required implementation
**Impact:** Feature not implemented
**Resolution needed:** Prioritise and schedule

### 27. Difficulty Levels
**Question:** Are multiple difficulties planned?
**Evidence:** `GameDefines` hardcodes 16×16/24 mines; `change-guide.md` documents procedure
**Impact:** Only one difficulty available
**Resolution needed:** Decide on difficulty config (enum, settings, or per-game choice)

### 28. Sound Effects
**Question:** Are sound effects planned?
**Evidence:** `AudioSettings.SoundEnabled` exists; no audio content or playback code
**Impact:** Setting has no effect
**Resolution needed:** Add audio content and playback, or remove setting

### 29. High Scores
**Question:** Is a high score table planned?
**Evidence:** No code or documentation references
**Impact:** No replay incentive
**Resolution needed:** Decide on feature scope

### 30. Multiplayer
**Question:** Is multiplayer/competitive mode planned?
**Evidence:** No code or documentation references
**Impact:** Single-player only
**Resolution needed:** Document as out of scope or plan feature

## Documentation Gaps

### 31. NuciXNA Internals
**Question:** How do NuciXNA singletons initialise and dispose?
**Evidence:** External package; source not in repository
**Impact:** Unknown lifecycle management
**Resolution needed:** Document assumptions or inspect NuciXNA source

### 32. MonoGame Version Compatibility
**Question:** Is MonoGame 3.8.4 the target version?
**Evidence:** `.csproj` references `MonoGame.Framework.DesktopGL 3.8.4`
**Impact:** Future upgrades may break
**Resolution needed:** Document upgrade procedure

### 33. .NET 10.0 Stability
**Question:** Is .NET 10.0 stable or preview?
**Evidence:** `TargetFramework: net10.0`
**Impact:** CI may use preview SDK
**Resolution needed:** Confirm release status; pin SDK version if needed

## How to Resolve

1. **Investigate:** Read source code, run experiments
2. **Decide:** Document intended behaviour
3. **Fix:** Implement changes if needed
4. **Update:** Revise affected documentation
5. **Remove:** Delete resolved items from this list