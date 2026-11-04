# Contributing to Minesweeper

This document covers the guidelines and processes for contributing to this project, including how to report issues, suggest enhancements, submit code changes, and follow project standards.

## 📑 Table of Contents

- [How to Contribute](#how-to-contribute)
- [License](#license)
- [Getting Assistance](#getting-assistance)

## 🤝 How to Contribute

### Reporting Issues

- Search existing issues first.
- Use the issue templates if available.
- Provide clear reproduction steps.
- Include environment details.

### Suggesting Enhancements

- Check the roadmap and existing discussions.
- Explain the use case and expected behaviour.
- Consider implementation complexity.

### Code Contributions

#### Prerequisites

- .NET 10.0 SDK

#### Development Setup

```bash
# Clone the repository
git clone https://github.com/hmlendea/minesweeper.git
cd minesweeper
```

#### Making Changes

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes.
4. Run tests: `dotnet test --no-build --verbosity normal`
5. Run linters: Not configured
6. Commit with clear and descriptive messages.
7. Push to your fork.
8. Open a Pull Request.

#### Pull Request Guidelines

- Target the `master` branch.
- Keep PRs focused and atomic.
- Revise documentation if applicable.
- Include tests for any new functionality.
- Ensure the CI checks pass.

### Code Style

Follow the project's coding standards:
- C# coding conventions as defined in the repository instructions
- Run `dotnet format` before committing (if available)

### Testing

```bash
# Run all tests
dotnet test --no-build --verbosity normal
```

### Documentation

- Revise relevant docs for changes.
- Follow the documentation style guide.
- Preview changes locally if possible.

## 📄 License

By contributing, you agree that your contributions will be licensed under the GNU General Public License v3.0.

## ❓ Getting Assistance

- Open a GitHub issue for questions or discussions.
- Check existing discussions and documentation first.