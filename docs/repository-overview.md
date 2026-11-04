# Repository Overview

## Purpose

Minesweeper is a self-contained desktop puzzle game that reimplements the classic Minesweeper game for modern Linux systems. It was created to fill the gap after the original game's removal from modern Windows releases.

## Scope

The repository contains:
- A MonoGame-based desktop application (`Minesweeper` project)
- Unit tests (`Minesweeper.UnitTests` project)
- Build configuration and CI/CD pipeline
- Documentation and project governance files

## Entry Points

| Entry Point | Location | Description |
|-------------|----------|-------------|
| Application | `Minesweeper/Program.cs` | `Program.Main()` prepares file system and starts `GameWindow.Run()` |
| Game Loop | `Minesweeper/GameWindow.cs` | `GameWindow : Game` — MonoGame host, graphics device, update/draw loop |
| Tests | `Minesweeper.UnitTests/` | NUnit 4.x test project with Moq |

## Principal Capabilities

- Classic Minesweeper gameplay with customizable board and grid settings
- Modern cross-platform desktop UI built with MonoGame (DesktopGL backend)
- Audio and graphics settings customization including fullscreen support
- Local-only persistence of user preferences (no network, no telemetry)

## Target Platform

- **Runtime:** .NET 10.0
- **Operating System:** Linux (primary), cross-platform capable via MonoGame DesktopGL
- **Graphics:** OpenGL via SDL2
- **Audio:** OpenAL