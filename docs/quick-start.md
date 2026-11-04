# Quick Start

Get the Minesweeper game running in 5 minutes.

## Prerequisites

| Requirement | Version | Install Command |
|-------------|---------|-----------------|
| .NET SDK | 10.0 | `dotnet --list-sdks` |
| Git | Any | `sudo apt-get install git` |
| SDL2 | 2.0+ | `sudo apt-get install libsdl2-2.0-0` |
| OpenAL | 1.1+ | `sudo apt-get install libopenal1` |
| MGCB Tool | Latest | `dotnet tool install --global dotnet-mgcb` |

## Clone & Build

```bash
# Clone repository
git clone <repository-url>
cd minesweeper

# Restore dependencies
dotnet restore Minesweeper.slnx

# Build solution (includes content pipeline)
dotnet build Minesweeper.slnx
```

## Run

```bash
# Development run
dotnet run --project Minesweeper/Minesweeper.csproj

# Release run (faster)
dotnet run --project Minesweeper/Minesweeper.csproj -c Release
```

## Play

| Action | Input |
|--------|-------|
| Reveal tile | Left click |
| Flag/Unflag | Right click |
| Restart game | `N` or `R` key |
| Main menu | `Escape` |

## Run Tests

```bash
# All tests
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj

# With coverage
dotnet test Minesweeper.UnitTests/Minesweeper.UnitTests.csproj --collect:"XPlat Code Coverage"
```

## Project Structure

```
Minesweeper/           # Main game project
├── GameWindow.cs      # Entry point, MonoGame host
├── GameLogic/         # Game rules, managers
├── Gui/               # Screens, controls, rendering
├── Models/            # Tile entity
├── Repositories/      # Tile storage
├── Settings/          # Configuration, persistence
└── Content/           # Fonts, textures (built to .xnb)

Minesweeper.UnitTests/ # NUnit tests
```

## Common Issues

| Problem | Fix |
|---------|-----|
| "MGCB not found" | `dotnet tool install --global dotnet-mgcb` |
| "SDL2 not found" | `sudo apt-get install libsdl2-2.0-0 libopenal1` |
| Window doesn't appear | `GDK_BACKEND=x11 dotnet run ...` (Wayland) |
| Timer stuck at 00:00 | Known bug — see `troubleshooting.md` |

## Next Steps

- Read `docs/architecture.md` for system design
- Read `docs/change-guide.md` for modification procedures
- Read `docs/testing/test-strategy.md` for testing approach
- Explore `docs/components/` for UI and screen details