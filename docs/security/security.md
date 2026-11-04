# Security

## Threat Model

**Application Type:** Local single-player desktop game
**Data Sensitivity:** Low (user preferences only)
**Network Access:** None
**Elevation:** None required

## Attack Surface

| Vector | Exposure | Mitigation |
|--------|----------|------------|
| File I/O (Settings.xml) | User-writable directory | No sensitive data |
| Content Loading (.xnb) | Read-only assets | Signed/verified at build |
| Input Handling | Keyboard/Mouse only | No injection vectors |
| Process Memory | Local user context | No secrets stored |

## Data Protection

### Settings.xml

**Location:** `%LocalAppData%\Minesweeper\Settings.xml` (Windows) or `~/.local/share/Minesweeper/Settings.xml` (Linux)

**Contents:**
```xml
<SettingsManager>
  <AudioSettings>
    <SoundEnabled>true</SoundEnabled>
  </AudioSettings>
  <GraphicsSettings>
    <Fullscreen>false</Fullscreen>
    <Resolution>
      <Width>640</Width>
      <Height>700</Height>
    </Resolution>
  </GraphicsSettings>
  <DebugMode>false</DebugMode>
</SettingsManager>
```

**Sensitivity:** None (preferences only)
**Encryption:** Not applied (not needed)
**Integrity:** Not verified (corruption → defaults)

### No Secrets

- No API keys
- No connection strings
- No credentials
- No tokens
- No personal data

## Input Validation

### Coordinate Validation

**GameManager.ClearTile/FlagTile:**
```csharp
Tile tile = tileRepository.Get(x, y);
if (tile is null) return;  // Bounds check via repository
```

**GameplayScreen Mouse Input:**
```csharp
if (boardX < 0 || boardX >= GameDefines.BoardWidth ||
    boardY < 0 || boardY >= GameDefines.BoardHeight) return;
```

### XML Deserialisation

**SettingsManager.LoadContent:**
```csharp
XmlFileObject<SettingsManager> loader = new XmlFileObject<SettingsManager>(file);
SettingsManager loaded = loader.Load();  // No validation
```

**Risk:** Malformed XML → `XmlException` (uncaught, crashes)
**Mitigation:** Add try-catch (see exceptions.md)

## Secure Coding Practices

### Followed

- ✅ No `eval` or dynamic code execution
- ✅ No SQL (no database)
- ✅ No network calls
- ✅ No user input in file paths
- ✅ No reflection on untrusted data
- ✅ Explicit types (no `var`) — prevents type confusion

### Not Applicable

- Authentication/Authorisation
- Session management
- CSRF/XSS (no web)
- SQL Injection
- Command Injection

## Dependency Security

### NuGet Packages

All from nuget.org with verified signatures:
- `MonoGame.Framework.DesktopGL` — Microsoft-signed
- `NuciXNA.*` — Single author (hmlendea), MIT licensed
- `NuciDAL` — Same author
- `NUnit`, `Moq`, etc. — Widely used, reputable

### Vulnerability Scanning

**Not configured in CI.** Recommended:
```yaml
# Add to .github/workflows/dotnet.yml
- name: Security Scan
  run: dotnet list package --vulnerable --include-transitive
```

## Platform Security

### Linux

- Runs as standard user (no sudo)
- Uses XDG directories (`~/.local/share/`)
- No setuid/setgid
- No capabilities required

### Windows

- Runs as standard user
- Uses `%LocalAppData%` (non-roaming)
- No registry writes
- No service installation

### macOS

- Runs as standard user
- Uses `~/Library/Application Support/`
- App Sandbox compatible (if signed)

## Content Security

### .xnb Files

- Compiled at build time from trusted sources
- Read-only at runtime
- No external content loading
- No modding support (currently)

### SpriteFont

- References system fonts at build time
- Embedded glyph data in .xnb
- No runtime font loading

## Randomness

**Mine Placement:** `new Random()` (unseeded)
- Not cryptographically secure
- Not a security concern (game logic only)
- Predictable if seed known (not exploitable)

## Logging Security

**No sensitive data logged** (no logging implemented)
- Settings contain no secrets
- Game state contains no secrets
- No PII processed

## Compliance

| Standard | Applicable | Status |
|----------|------------|--------|
| GDPR | No (no personal data) | N/A |
| CCPA | No | N/A |
| SOC 2 | No | N/A |
| PCI DSS | No | N/A |

## Security Checklist for Changes

- [ ] No new network calls added
- [ ] No new file writes outside user data directory
- [ ] No user input in file paths
- [ ] No secrets in code or config
- [ ] No dynamic code execution
- [ ] Dependencies updated for vulnerabilities
- [ ] XML deserialisation wrapped in try-catch
- [ ] Content loading errors handled gracefully