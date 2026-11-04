# Documentation Maintenance

Guidelines for keeping the Minesweeper documentation accurate and up to date.

## Documentation Structure

```
docs/
├── INDEX.md                           # Master navigation
├── repository-overview.md             # Purpose, scope, entry points
├── repository-structure.md            # Source tree, modules
├── architecture.md                    # Layered architecture
├── components/
│   ├── presentation.md                # Screens, controls, rendering
│   ├── host-and-composition.md        # GameWindow, singletons
│   └── application-services.md        # Settings, paths, NuciXNA
├── flows/
│   ├── startup-and-rendering.md       # Program → GameWindow → first frame
│   ├── gameplay-loop.md               # Update/Draw cycle
│   └── screen-transitions.md          # ScreenManager flows
├── data-model/
│   ├── tile-entity.md                 # Tile class, state machine
│   ├── repository-pattern.md          # TileRepository, NuciDAL
│   └── game-state.md                  # Distributed state, transitions
├── configuration/
│   ├── settings.md                    # SettingsManager, XML persistence
│   ├── build.md                       # csproj, content, CI
│   └── dependencies.md                # NuGet, system deps, licenses
├── testing/
│   ├── unit-tests.md                  # 30 tests, conventions, gaps
│   ├── integration-tests.md           # Planned scenarios
│   └── test-strategy.md               # Pyramid, coverage, CI
├── build-and-deployment/
│   ├── build-instructions.md          # Prerequisites, build, run, publish
│   └── deployment.md                  # Targets, self-contained, versioning
├── error-handling/
│   ├── exceptions.md                  # Hierarchy, handling, improvements
│   └── logging.md                     # Current state, Serilog migration
├── security/
│   └── security.md                    # Threat model, attack surface
├── state-persistence/
│   ├── save-load.md                   # Game save feature design
│   └── settings-persistence.md        # Settings XML details
├── concurrency/
│   └── threading.md                   # Single-threaded model
├── design-decisions/
│   └── adr.md                         # 10 Architecture Decision Records
├── invariants.md                      # System-wide contracts
├── data-model.md                      # Entity relationships
├── change-guide.md                    # Common modification procedures
├── troubleshooting.md                 # Issues, diagnostics, fixes
├── faq.md                             # Frequently asked questions
├── quick-start.md                     # 5-minute setup
├── documentation-maintenance.md       # This file
└── ambiguities-and-open-questions.md  # Known unknowns
```

## Update Triggers

Update documentation when:

| Trigger | Files to Review |
|---------|-----------------|
| New screen added | `components/presentation.md`, `flows/screen-transitions.md`, `change-guide.md` |
| Board size changed | `invariants.md`, `data-model/tile-entity.md`, `change-guide.md`, `quick-start.md` |
| New setting added | `configuration/settings.md`, `state-persistence/settings-persistence.md` |
| New dependency added | `configuration/dependencies.md`, `build-and-deployment/build-instructions.md` |
| Test added/changed | `testing/unit-tests.md`, `testing/test-strategy.md` |
| Bug fixed | `troubleshooting.md`, `invariants.md` (if invariant changed) |
| Architecture decision | `design-decisions/adr.md`, `architecture.md` |
| New feature | All relevant component/flow/data-model files |

## Review Checklist

Before merging changes, verify:

- [ ] All cross-references still valid (file paths, section anchors)
- [ ] Code snippets compile (copy-paste test)
- [ ] Constants match `GameDefines.cs` (TableSize, MinesCount, etc.)
- [ ] Class/method names match current source
- [ ] Namespace paths correct
- [ ] No references to deleted/renamed files
- [ ] Version numbers updated if applicable
- [ ] TODO/FIXME markers addressed or moved to `ambiguities-and-open-questions.md`

## Style Guidelines

### Markdown
- ATX headings (`#`, `##`, `###`)
- Tables for structured data
- Fenced code blocks with language (`csharp`, `bash`, `yaml`)
- Relative links to other docs (`../architecture.md#layers`)
- No HTML unless necessary

### Code Snippets
- Use real code from codebase (copy, don't invent)
- Keep minimal — 5–20 lines
- Include `using` statements if non-obvious
- Comment non-obvious lines

### Diagrams
- Mermaid for flow/sequence/class diagrams
- ASCII for simple trees
- Keep under 80 columns wide

### Tone
- Imperative mood ("Update the constant", not "You should update")
- Present tense
- Specific over generic
- Link to source files

## Versioning

- Documentation version tracks code version
- Major changes: Update `README.md` version badge
- Breaking changes: Note in `change-guide.md` and `CHANGELOG.md` (if exists)

## Automation

### CI Checks (Recommended)
```yaml
# .github/workflows/docs.yml
- name: Check Links
  run: |
    # Install markdown-link-check
    npx markdown-link-check docs/**/*.md

- name: Check Code Snippets
  run: |
    # Extract and compile snippets (custom script)
    ./scripts/verify-snippets.sh
```

### Pre-commit Hook (Optional)
```bash
# .git/hooks/pre-commit
#!/bin/bash
# Check modified .md files for broken links
for f in $(git diff --cached --name-only -- '*.md'); do
  markdown-link-check "$f" || exit 1
done
```

## Responsibility

| Role | Responsibility |
|------|----------------|
| Feature author | Update affected docs in same PR |
| Reviewer | Verify docs updated |
| Maintainer | Periodic full review (quarterly) |

## Deprecation Process

1. Mark section with `> **Deprecated:**` notice
2. Link to replacement
3. Keep for 2 releases
4. Remove and update `INDEX.md`

## Tools

| Task | Tool |
|------|------|
| Link checking | `markdown-link-check` (npm) |
| Spell check | `cspell` (VS Code extension) |
| Format | `prettier --parser markdown` |
| Diagram preview | Mermaid Live Editor / VS Code preview |

## Metrics

Track:
- Stale docs (last modified > 6 months)
- Broken links (CI)
- Missing docs for new features (PR checklist)
- FAQ hits (if analytics available)