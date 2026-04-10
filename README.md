# AutoInput Plus

AutoInput Plus is a Windows desktop application for configuring and running automated keyboard and mouse input profiles.

---

## Overview

The solution is structured to separate concerns between runtime execution, input handling, persistence, and UI.

### AutoInputPlus.Core

Shared contracts and domain models.

- Application models
- Profile models
- Enums and constants
- Interfaces used across the solution

---

### AutoInputPlus.Engine

Handles runtime execution of automation.

- Input execution logic
- Engine lifecycle and state management
- Repeat / hold / count-based behavior
- Profile execution flow

The engine is the **runtime source of truth** for execution state.

---

### AutoInputPlus.Infrastructure

Persistence and system-level services.

- App configuration storage
- Profile storage
- Import / export functionality
- Windows startup registration

---

### AutoInputPlus.Input.Windows

Windows-specific input handling.

- Global hotkey registration
- Keyboard and mouse capture
- Input simulation

---

### AutoInputPlus.Wpf

The desktop UI.

- WPF views and layout
- User interaction flow
- Engine integration
- Profile management UI
- Tray/menu integration

---

## Build / Run

Open the solution in Visual Studio or use the scripts in `/scripts`.

- Main entry project: `src/AutoInputPlus.Wpf`
- Use `scripts/run.sh` to build and run the app.

---
