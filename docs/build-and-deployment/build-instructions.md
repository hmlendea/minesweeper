# Build Instructions

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0 | Build and run |
| MGCB Tool | Latest | Content compilation (auto-installed) |
| SDL2 | 2.x | Runtime (Linux) |
| OpenAL | 1.x | Runtime audio (Linux) |
| FreeType | 2.x | Font rendering (Linux) |

### Linux (Ubuntu/Debian)

```bash
# .NET 10.0 SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

# Runtime dependencies
sudo apt install libsdl2-2.0-0 libopenal1 libfreetype6 ttf-mscorefonts-installer
```

### Windows

- .NET 10.0 SDK from https://dotnet.microsoft.com/download
- No additional runtime dependencies (bundled)

### macOS

```bash
brew install sdl2
```

## Local Build

### Restore

```bash
dotnet restore Minesweeper.slnx
```

### Build

```bash
# Debug
dotnet build Minesweeper.slnx

# Release
dotnet build Minesweeper.slnx -c Release
```

**Output:**
- `Minesweeper/bin/Debug/net10.0/Minesweeper.dll`
- `Minesweeper/bin/Release/net10.0/Minesweeper.dll`

### Content Build

Content is built automatically via `MonoGame.Content.Builder.Task` NuGet package.

**Manual Build (if needed):**
```bash
dotnet tool install -g dotnet-mgcb
mgcb /@:Content/Content.mgcb
```

**Output:** `Content/bin/DesktopGL/Content/` with `.xnb` files

## Running the Application

### From Build Output

```bash
dotnet Minesweeper/bin/Debug/net10.0/Minesweeper.dll
```

### From Project Directory

```bash
dotnet run --project Minesweeper/Minesweeper.csproj
```

### Release Mode

```bash
dotnet run --project Minesweeper/Minesweeper.csproj -c Release
```

## Running Tests

```bash
# All tests
dotnet test Minesweeper.slnx

# Specific project
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj

# With coverage
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj --collect:"XPlat Code Coverage"

# Filter
dotnet test --filter "FullyQualifiedName~GameManagerTests"
```

## Publishing

### Framework-Dependent (Requires .NET Runtime)

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release
```

**Output:** `Minesweeper/bin/Release/net10.0/publish/`

### Self-Contained (Includes Runtime)

```bash
# Linux x64
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r linux-x64 --self-contained true

# Windows x64
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r win-x64 --self-contained true

# macOS x64
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r osx-x64 --self-contained true
```

**Output:** `Minesweeper/bin/Release/net10.0/{rid}/publish/`

### Publish Options

| Option | Description |
|--------|-------------|
| `--self-contained true` | Includes .NET runtime (~150 MB) |
| `--self-contained false` | Requires runtime installed (~5 MB) |
| `-r linux-x64` | Target runtime identifier |
| `-p:PublishSingleFile=true` | Single executable |
| `-p:PublishTrimmed=true` | Trim unused code (smaller, may break reflection) |

## Troubleshooting

### Build Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `MGCB not found` | Tool not installed | `dotnet tool install -g dotnet-mgcb` |
| `Content not found` | Wrong working directory | Run from solution root |
| `SDL2 not found` | Missing runtime library | Install `libsdl2-2.0-0` |
| `Font not found` | Missing mscorefonts | Install `ttf-mscorefonts-installer` |

### Runtime Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `DllNotFoundException: SDL2` | SDL2 not installed | Install SDL2 runtime |
| `ContentLoadException` | Missing .xnb files | Rebuild content |
| `GraphicsDeviceException` | No GPU/display | Use Xvfb or software rendering |

### Test Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `Test host not found` | Missing Test SDK | `dotnet restore` |
| `NUnit not discovered` | Adapter missing | Install `NUnit3TestAdapter` |

## IDE Setup

### Visual Studio Code

**Recommended Extensions:**
- C# Dev Kit
- .NET Install Tool
- NuGet Package Manager

**Tasks:**
```json
{
  "label": "Build",
  "command": "dotnet build Minesweeper.slnx",
  "type": "shell"
}
```

### Visual Studio

1. Open `Minesweeper.slnx`
2. Set `Minesweeper` as startup project
3. F5 to run, Ctrl+R, A to run tests

### Rider

1. Open solution
2. Build → Build Solution
3. Run → Run 'Minesweeper'

## Performance Notes

- **Build Time:** ~30 seconds (cold), ~5 seconds (incremental)
- **Test Time:** < 1 second (30 tests)
- **Startup Time:** ~2 seconds (content loading)
- **Memory:** ~100 MB baseline, ~150 MB during gameplay