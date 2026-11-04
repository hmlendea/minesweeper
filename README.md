[![Build Status](https://github.com/hmlendea/minesweeper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/minesweeper/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/minesweeper)](https://github.com/hmlendea/minesweeper/blob/master/LICENSE)

# Minesweeper

Modern and open-source reimplementation of the classic puzzle game Minesweeper.

This is a very similar game to the original game with the same name. It was created to fill the gap after the original's removal from modern Windows releases.

The objective is to carefully clear all the mines on a square board, without detonating them.

## 📑 Table of Contents

- [Capabilities](#-capabilities)
- [Usage](#-usage)
  - [Keyboard Shortcuts](#keyboard-shortcuts)
  - [Mouse Controls](#mouse-controls)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
  - [Manual Installation](#manual-installation)
  - [Installation from Source](#installation-from-source)
- [Compatibility](#-compatibility)
- [Privacy and Data](#-privacy-and-data)
  - [Data Locations](#data-locations)
- [Development](#-development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Run](#run)
- [Contributing](#-contributing)
- [Code of Conduct](#-code-of-conduct)
- [Security](#-security)
- [Architecture](#-architecture)
- [GitHub Actions](#-github-actions)
- [Project Structure](#-project-structure)
  - [Projects and Packages](#projects-and-packages)
  - [Directories](#directories)

## ✨ Capabilities

- Classic Minesweeper puzzle gameplay with customizable board and grid settings
- Modern cross-platform desktop UI built with MonoGame
- Audio and graphics settings customization including fullscreen support

## 🚀 Usage

Launch the Minesweeper executable to start a new puzzle game. Use the mouse to reveal tiles or place flags to mark suspected mine locations.

### Keyboard Shortcuts

| Shortcut | Action | Context |
|----------|--------|---------|
| `N` / `R` | New game / Restart | Gameplay screen |
| `Escape` | Open menu / Return to title | Gameplay, Game Over, Victory screens |
| `Enter` / `Space` | Confirm / Continue | Game Over, Victory, Splash screens |

### Mouse Controls

| Action | Button | Context |
|--------|--------|---------|
| Reveal tile | Left click | Gameplay screen |
| Place/remove flag | Right click | Gameplay screen |
| Navigate menus | Left click | All screens |

## 🖥️ System Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| Operating System | Linux | Linux |
| Runtime | .NET 10.0 Runtime | .NET 10.0 Runtime |

## 📦 Installation

[![Obtain it from GitHub](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/github.png)](https://github.com/hmlendea/minesweeper/releases)

### Manual Installation

Download the latest release archive from GitHub Releases, extract it, and execute `Minesweeper`.

### Installation from Source

```bash
git clone https://github.com/hmlendea/minesweeper.git
cd minesweeper
dotnet build -c Release
```

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| .NET | 10.0 | Built targeting .NET 10.0 |
| Operating System | Linux | Primary supported platform |

## 🛡️ Privacy and Data

Minesweeper is a self-contained desktop application that does not collect, process, transmit, or share any personal data, analytics, or telemetry.

For the detailed description of how the application handles privacy and personal data, see the [Privacy Statement](./PRIVACY.md).

The application only persists local graphics and audio preferences to a local configuration file.

| Data | Purpose | Storage | Retention | Optional |
|------|---------|---------|-----------|----------|
| User Preferences | Saving graphics and audio settings | Local XML file | Until deleted | Yes |

### Data Locations

| Platform or Scope | Location | Contents |
|-------------------|----------|----------|
| Linux / macOS | `~/.local/share/Minesweeper/Settings.xml` | Graphics and audio configuration |
| Windows | `%LocalAppData%\Minesweeper\Settings.xml` | Graphics and audio configuration |

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
git clone https://github.com/hmlendea/minesweeper.git
cd minesweeper
```

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project Minesweeper
```

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`

For detailed contribution guidelines, see [CONTRIBUTING.md](./CONTRIBUTING.md).

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 🔒 Security

See the [Security Policy](./SECURITY.md) for supported versions, vulnerability reporting, and disclosure practices.

## 🏗️ Architecture

See the [Architecture Document](./ARCHITECTURE.md) for system context, architectural style, runtime flow, components, data architecture, and design constraints.

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [ dotnet.yml ](https://github.com/hmlendea/minesweeper/blob/master/.github/workflows/dotnet.yml) | Continuous integration | Builds the solution, runs tests on every push and pull request to master |

## 🗂️ Project Structure

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `Minesweeper/` | Application | Main Minesweeper desktop game |
| `Minesweeper.UnitTests/` | Unit Tests | Unit tests for game logic |

### Directories

| Directory | Purpose |
|-----------|---------|
| `Minesweeper/GameLogic/` | Game logic, managers, and repositories |
| `Minesweeper/Gui/` | UI screens, controls, and helpers |
| `Minesweeper/Content/` | MonoGame content (fonts, textures, audio) |
| `Minesweeper/Settings/` | Configuration and settings management |
| `Minesweeper/Resources/` | Embedded resources |

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/minesweeper/issues)!

If you find this project useful, consider starring ⭐️ it on GitHub!

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for additional information.
