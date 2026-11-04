[![Build Status](https://github.com/hmlendea/minesweeper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/minesweeper/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/minesweeper)](https://github.com/hmlendea/minesweeper/blob/master/LICENSE)

# Minesweeper

Modern and open-source reimplementation of the classic puzzle game Minesweeper.

This is a very similar game to the original game with the same name. It was created to fill the gap after the original's removal from modern Windows releases.

The objective is to carefully clear all the mines on a square board, without detonating them.

## 📑 Table of Contents

- [Capabilities](#-capabilities)
- [Usage](#-usage)
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

## ✨ Capabilities

- Classic Minesweeper puzzle gameplay with customizable board and grid settings
- Modern cross-platform desktop UI built with MonoGame
- Audio and graphics settings customization including fullscreen support

## 🚀 Usage

Launch the Minesweeper executable to start a new puzzle game. Use the mouse to reveal tiles or place flags to mark suspected mine locations.

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
