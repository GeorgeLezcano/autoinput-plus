# AutoInputPlus.Input.Windows

Platform-specific Windows implementation for the AutoInputPlus input
system.

This project contains **all Windows-specific input logic** used by the
application, including keyboard simulation, mouse simulation, and global
hotkey handling.

The goal of this project is to **isolate OS-level functionality** so
that:

-   `Core` remains platform-agnostic
-   `Engine` focuses only on automation logic
-   `Wpf` remains UI-only
-   Windows interop code does not leak into the rest of the solution

------------------------------------------------------------------------

# Architectural Role

AutoInputPlus uses a layered architecture:

Core │ ├── Interfaces ├── Models └── Contracts

Engine │ ├── Runtime automation logic ├── Sequence execution └── Profile
management

Infrastructure │ ├── Persistence └── File system

Input.Windows \<-- THIS PROJECT │ ├── Windows keyboard simulation ├──
Windows mouse simulation ├── Global hotkey handling └── Win32 interop
wrappers

Wpf │ └── UI and tray application shell

This project acts as the **Windows adapter layer** for input.

------------------------------------------------------------------------

# Dependency Direction

Dependencies must follow this rule:

Core ↑ Input.Windows ↑ Wpf

Allowed references:

Input.Windows → Core\
Wpf → Input.Windows

Not allowed:

Engine → Win32 APIs\
Core → Windows-specific code

------------------------------------------------------------------------

# Dependency Injection

This project exposes a DI entry point:

DependencyInjection.cs

Example registration:

services.AddWindowsInputServices();

The purpose of this method is to register platform implementations for
interfaces defined in **Core**.

Example (future):

services.AddSingleton\<IInputSender, WindowsInputSender\>();
services.AddSingleton\<IGlobalHotkeyService, WindowsHotkeyService\>();

------------------------------------------------------------------------

# Future Responsibilities

The following components will eventually live here.

------------------------------------------------------------------------

# 1. Keyboard Input Simulation

Windows keyboard events using **SendInput**.

Possible implementation:

Input/ ├── WindowsInputSender.cs ├── KeyboardInputSender.cs

Responsibilities:

-   Key press
-   Key down
-   Key up
-   Key hold
-   Key repeat

Based on Win32:

SendInput\
KEYBDINPUT

------------------------------------------------------------------------

# 2. Mouse Input Simulation

Mouse simulation through Win32.

Examples:

-   Left click
-   Right click
-   Middle click
-   Mouse button hold
-   Mouse button release
-   Mouse wheel scroll

Possible structure:

Input/ ├── WindowsMouseSender.cs

Based on Win32:

SendInput\
MOUSEINPUT

------------------------------------------------------------------------

# 3. Global Hotkeys

Used for:

Start / Stop automation\
Pause automation

Possible structure:

Hotkeys/ ├── WindowsHotkeyService.cs

Responsibilities:

-   Register global hotkeys
-   Unregister hotkeys
-   Dispatch events to engine

Windows APIs:

RegisterHotKey\
UnregisterHotKey

------------------------------------------------------------------------

# 4. Key Mapping

Convert application-level key definitions into Windows Virtual Key
codes.

Example structure:

Mapping/ ├── KeyCodeMapper.cs ├── MouseButtonMapper.cs

Responsibilities:

-   Map Core key models → Win32 VK codes
-   Validate supported keys
-   Provide fallback behavior

------------------------------------------------------------------------

# 5. Win32 Interop Layer

All raw Windows API calls must stay inside this project.

Example structure:

Interop/ ├── NativeMethods.cs ├── NativeConstants.cs ├──
NativeStructs.cs

Responsibilities:

-   Encapsulate P/Invoke calls
-   Define native structs
-   Prevent Win32 code from leaking elsewhere

------------------------------------------------------------------------

# Design Principles

### Keep platform logic isolated

Only this project should know about:

user32.dll\
SendInput\
RegisterHotKey\
Win32 structs

------------------------------------------------------------------------

### Engine should only use interfaces

Example:

IInputSender\
IGlobalHotkeyService

These interfaces belong in **Core**.

This project provides the Windows implementation.

------------------------------------------------------------------------

### Avoid UI dependencies

This project must **not reference WPF**.

Do not depend on:

System.Windows\
Wpf\
WinForms

------------------------------------------------------------------------

# Example Flow (Future)

User presses Start Hotkey\
↓\
WindowsHotkeyService\
↓\
Engine.Start()\
↓\
SequenceRunner\
↓\
IInputSender\
↓\
WindowsInputSender\
↓\
SendInput (Win32)

------------------------------------------------------------------------

# Why this separation exists

Without this project:

-   Win32 code spreads into UI
-   Engine becomes platform-specific
-   Future portability becomes impossible
-   Testing becomes difficult

By isolating Windows functionality here:

-   Architecture stays clean
-   Engine remains testable
-   Platform code is easy to maintain

------------------------------------------------------------------------

# Status

Current status:

Scaffold only\
No runtime implementation yet

Planned future phases:

1.  Keyboard input
2.  Mouse input
3.  Key mapping
4.  Global hotkeys
5.  Advanced targeting (window/process)

------------------------------------------------------------------------

# Notes

This project intentionally starts minimal. Implementation will grow
incrementally as input features are added to the engine.
