# Build Configuration

## Project Structure

```
Minesweeper.slnx
├── Minesweeper/
│   ├── Minesweeper.csproj
│   ├── Program.cs
│   ├── GameWindow.cs
│   ├── Content/
│   │   └── Content.mgcb
│   ├── GameLogic/
│   ├── Gui/
│   ├── Models/
│   ├── Repositories/
│   ├── Resources/
│   └── Settings/
└── Minesweeper.UnitTests/
    ├── Minesweeper.UnitTests.csproj
    ├── GameLogic/
    ├── Models/
    └── Repositories/
```

## Minesweeper.csproj

**Target Framework:** `net10.0`

**Output Type:** `WinExe` (Windows executable, single-process)

**Key Properties:**
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>WinExe</OutputType>
  <RootNamespace>Minesweeper</RootNamespace>
  <AssemblyName>Minesweeper</AssemblyName>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

**Package References:**
| Package | Version | Purpose |
|---------|---------|---------|
| `MonoGame.Framework.DesktopGL` | 3.8.4 | Core game framework (OpenGL/SDL2) |
| `NuciXNA.Gui` | 3.2.1 | Screen management, GUI controls |
| `NuciXNA.Graphics` | 2.2.3 | GraphicsManager, primitives |
| `NuciXNA.Input` | 2.0.4 | InputManager, events |
| `NuciXNA.Primitives` | 2.1.6 | Primitive drawing helpers |
| `NuciXNA.DataAccess` | 4.0.3 | NuciContentManager, Repository<T> |
| `NuciDAL` | 3.1.0 | ModelBase, XmlFileObject |

**Content Build:**
```xml
<ItemGroup>
  <MonoGameContentReference Include="Content\Content.mgcb" />
</ItemGroup>
```

## Minesweeper.UnitTests.csproj

**Target Framework:** `net10.0`

**Output Type:** `Library`

**Package References:**
| Package | Version | Purpose |
|---------|---------|---------|
| `NUnit` | 4.2.2 | Test framework |
| `NUnit3TestAdapter` | 4.6.0 | VS Test Explorer integration |
| `Microsoft.NET.Test.Sdk` | 17.11.1 | Test host |
| `Moq` | 4.20.72 | Mocking (unused in current tests) |
| `coverlet.collector` | 6.0.2 | Code coverage |

**Project Reference:**
```xml
<ItemGroup>
  <ProjectReference Include="..\Minesweeper\Minesweeper.csproj" />
</ItemGroup>
```

## Content.mgcb

**Location:** `Minesweeper/Content/Content.mgcb`

**Purpose:** Defines all content assets for MonoGame Content Builder

**Structure:**
```
# Directories
/Fonts
/ScreenManager
/SplashScreen
/Tiles

# Fonts
/Fonts/DefaultFont.spritefont
/Fonts/FrameCounterFont.spritefont
/Fonts/InfoBarFont.spritefont
/Fonts/MenuFont.spritefont
/Fonts/ToolTipFont.spritefont

# ScreenManager
/ScreenManager/FillImage.png
/ScreenManager/missing-texture.png

# SplashScreen
/SplashScreen/Logo.png

# Tiles
/Tiles/flag.png
/Tiles/mine.png
```

**Build Output:** `Content/bin/DesktopGL/Content/` with `.xnb` files

## Solution File (Minesweeper.slnx)

**Format:** VS Solution XML (`.slnx`)

**Projects:**
1. `Minesweeper` — Main application
2. `Minesweeper.UnitTests` — Unit test project

**Configuration:** Debug/Release for `Any CPU`

## Build Process

### Local Build (dotnet CLI)

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build Minesweeper.slnx

# Run application
dotnet run --project Minesweeper/Minesweeper.csproj

# Run tests
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj
```

### Content Build Integration

`MonoGame.Content.Builder.Task` NuGet package (transitive via `MonoGame.Framework.DesktopGL`) hooks into MSBuild:

1. Pre-build: `MGCB` compiles `Content.mgcb` → `.xnb` files
2. Output: `Content/bin/DesktopGL/Content/`
3. Runtime: `NuciContentManager` loads from compiled output

### CI Build (GitHub Actions)

**Workflow:** `.github/workflows/dotnet.yml`

**Steps:**
1. Checkout
2. Setup .NET 10.0
3. Install `mscorefonts` (for font rendering)
4. Install `MGCB` tool: `dotnet tool install -g dotnet-mgcb`
5. Restore: `dotnet restore`
6. Build: `dotnet build --no-restore`
7. Test: `dotnet test --no-build --verbosity normal`

**Environment:** Ubuntu latest

## Runtime Dependencies

### Linux (Primary Target)

| Dependency | Package (Ubuntu/Debian) | Purpose |
|------------|-------------------------|---------|
| `libsdl2-2.0-0` | `libsdl2-2.0-0` | Window/input (via MonoGame) |
| `libopenal1` | `libopenal1` | Audio (via MonoGame) |
| `libfreetype6` | `libfreetype6` | Font rendering |
| `mscorefonts` | `ttf-mscorefonts-installer` | Default fonts |

### Windows

- No additional dependencies (SDL2/OpenAL bundled by MonoGame)

### macOS

- SDL2 via Homebrew: `brew install sdl2`

## Publishing

### Self-Contained (Linux)

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release -r linux-x64 --self-contained true
```

**Output:** `Minesweeper/bin/Release/net10.0/linux-x64/publish/`

### Framework-Dependent

```bash
dotnet publish Minesweeper/Minesweeper.csproj -c Release
```

**Requires:** .NET 10.0 runtime installed

## Known Build Issues

1. **MGCB Tool Path:** CI installs `dotnet-mgcb` globally; local builds may need `dotnet tool restore` if using local tool manifest.

2. **Font Content:** SpriteFont files reference system fonts; `mscorefonts` required on Linux for consistent rendering.

3. **Content Root:** `GameWindow` sets `Content.RootDirectory = "Content"`; expects compiled content in `Content/bin/DesktopGL/Content/`.

4. **Nullable/ImplicitUsings:** Enabled in project; code uses explicit types (no `var`) per coding standards.