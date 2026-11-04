# Privacy and Personal Data

This document describes how Minesweeper handles personal data. The application is a self-contained desktop game with no network connectivity, telemetry, analytics, or external integrations. No personal data is collected, transmitted, or shared.

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Self-Hosted Deployments](#self-hosted-deployments)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Data Protection and Security](#data-protection-and-security)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

This document describes how Minesweeper at https://github.com/hmlendea/minesweeper handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 🏠 Self-Hosted Deployments

Minesweeper is a self-contained desktop application that users compile and run locally. The project maintainers do not operate any central service, collect any data from instances, or receive any telemetry, update checks, crash reports, or other data from self-hosted deployments. Instance operators (users who build and run the application) have full control over their local configuration, storage, logs, backups, access controls, retention, and request handling. No data is sent from a self-hosted instance to project maintainers or any external service.

## 📥 Data We Handle

### Data Provided to the Application

No personal data is requested, collected, or provided to the application by users, administrators, or connected systems.

### Data Generated or Collected by the Application

No personal data is generated or collected automatically by the application. The application stores only local user preferences (graphics settings such as fullscreen state and resolution, audio settings such as sound enabled state, and a debug mode flag) in an XML file at the user's local application data directory. These preferences contain no personally identifiable information.

### Data Received from Integrations

No personal data is received from integrations or third parties. The application has no built-in integrations, network connectivity, or external data sources.

## 🧭 Processing and Use

The application processes the local preference data described above solely for these verified functions:
- Applying graphics settings (fullscreen state and resolution) — Graphics settings category
- Applying audio settings (sound enabled state) — Audio settings category
- Enabling or disabling debug mode — Debug mode category

## 🗄️ Storage, Retention, and Deletion

Local preference data is stored in an XML file at `%LocalAppData%\Minesweeper\Settings.xml` (Windows) or the equivalent platform-specific local application data directory (Linux/macOS). The file is created on first run and updated when the user changes settings in the application. The instance operator (the user) controls this file and can delete it at any time to reset preferences. No backups are created by the application. The project maintainers have no access to this file or its contents.

## 🔗 External Processing and Integrations

The application has no built-in external data transfer, integrations, or third-party services that process or receive data.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | Not applicable | Not applicable | Not applicable |

## 🛡️ Data Protection and Security

The application stores only local preference data in a user-writable directory. No encryption, access controls, or network protections are implemented because no sensitive or personal data is handled. Instance operators are responsible for their own system security, including file permissions, disk encryption, and network exposure. The original maintainers are not responsible for changes, modifications, or privacy issues introduced by downstream forks.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/minesweeper/blob/master/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers via GitHub issues at https://github.com/hmlendea/minesweeper/issues. For a self-hosted instance, contact the instance operator, unless the project explicitly handles the request. Include the operating system, application version, and steps to reproduce the data flow in question; do not send passwords, access tokens, or other secrets.