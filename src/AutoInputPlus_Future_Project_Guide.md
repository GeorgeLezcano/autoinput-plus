# AutoInputPlus Future Project Guide

## Purpose

This guide describes the recommended implementation order after the current `Input.Windows` stopping point. The goal is to keep the solution moving in a controlled way, with each project gaining a clear responsibility before the next layer is wired in.

At this point, `AutoInputPlus.Input.Windows` is treated as the platform adapter package for:

- global hotkey registration
- keyboard input sending
- mouse button input sending
- mouse wheel sending
- key and modifier mapping to Windows-native values

The next work should happen outside that package unless a later feature clearly belongs there, such as cursor movement or cursor position support.

---

## Current stopping point

The solution is in a good place to pause after the following package-level milestone:

- `Core` contains the app-facing contracts and models.
- `Input.Windows` contains the Windows-specific implementation details.
- `Wpf` is still not deeply wired into hotkeys or input playback.
- `Engine` is still the main unfinished orchestration layer.
- `Infrastructure` is still the main unfinished persistence/settings layer.

That means the next priorities are orchestration, validation, persistence, and host wiring.

---

## Recommended implementation order

## Phase 1: Lock down Core models and execution expectations

### Goal
Make sure `Core` has the final shape needed by `Engine`, `Infrastructure`, and `Wpf` before those projects are wired together.

### Work

#### 1. Confirm model responsibilities
Review and keep the boundaries clear for these models:

- `AppConfiguration`
- `InputProfile`
- `Sequence`
- `SequenceStep`
- `Hotkey`

`Core` should only contain app concepts, not Windows-native concepts.

#### 2. Decide key token policy
Right now `IInputSender` and `Hotkey` use `string` key tokens. Before wiring the rest of the app, decide how the app will avoid typos.

Recommended approach for the next phase:

- add a centralized key token catalog in `Core`, such as `InputKeys`
- use constants for supported tokens like `F8`, `Enter`, `Space`, `A`, `LeftCtrl`, etc.
- make the UI bind to known values instead of letting users type arbitrary key strings everywhere

This keeps the string contract flexible while reducing typo risk.

#### 3. Decide validation ownership
Choose where invalid steps are rejected.

Recommended approach:

- model classes remain simple
- execution-time validation lives in `Engine`
- UI can do friendly pre-validation, but `Engine` remains the final guardrail

#### 4. Confirm sequence behavior rules
Before implementing playback, define these behaviors clearly:

- what a disabled step means
- how `DelayBeforeMilliseconds` and `DelayAfterMilliseconds` are applied
- what `HoldDuringDelay` means for `KeyDown` and `MouseDown`
- whether a `MouseWheelDelta` of zero is ignored or rejected in sequence validation

### Exit criteria
Move on only when the app-facing contracts are stable enough that `Engine` can be implemented without reopening `Input.Windows`.

---

## Phase 2: Implement Engine playback orchestration

### Goal
Make `Engine` the place that actually interprets profiles and sequences using `IInputSender`.

### Work

#### 1. Implement `ISequenceRunner`
`ISequenceRunner` should translate `SequenceStep` values into calls to `IInputSender`.

Recommended behavior:

- skip disabled steps
- apply pre-delay
- execute the action
- if `HoldDuringDelay` is true for down/hold actions, apply post-delay while held
- otherwise apply post-delay after the action finishes

For example:

- `KeyPress` -> `KeyPress(step.Key)`
- `KeyDown` -> `KeyDown(step.Key)`
- `KeyUp` -> `KeyUp(step.Key)`
- `MouseClick` -> `MouseClick(step.MouseButton.Value)`
- `MouseDown` -> `MouseDown(step.MouseButton.Value)`
- `MouseUp` -> `MouseUp(step.MouseButton.Value)`
- `MouseWheel` -> `MouseWheel(step.MouseWheelDelta)`

#### 2. Add sequence validation
Before executing each step, validate only what is required for that action type.

Examples:

- keyboard actions require `Key`
- mouse button actions require `MouseButton`
- wheel actions require a usable `MouseWheelDelta`
- delays should not be negative

#### 3. Implement `IEngine`
The engine should own the high-level state transitions, not raw input details.

Suggested responsibilities:

- start sequence playback
- stop playback safely
- toggle between start and stop
- expose current `EngineState`
- ensure repeated start/stop calls behave predictably

#### 4. Decide threading model
Playback should not block the UI thread.

Recommended direction:

- sequence execution runs asynchronously
- cancellation support is used for stop requests
- the engine serializes start/stop transitions to avoid overlap

### Exit criteria
Move on when one profile/sequence can be played through `Engine` using `Input.Windows`, even if there is no polished WPF UI yet.

---

## Phase 3: Implement Infrastructure persistence and settings

### Goal
Give the application a stable way to save profiles and app configuration before building a larger UI on top.

### Work

#### 1. Implement `IInputProfileStore`
The store should support saving and loading profiles with their sequences and steps.

Recommended first version:

- JSON file persistence
- one main application data folder
- explicit load/save methods
- safe handling for missing files and malformed content

#### 2. Implement `IAppConfigurationStore`
Persist app-level settings such as:

- selected hotkey
- selected profile
- startup behavior
- UI preferences that belong outside WPF view state

#### 3. Implement `IProfileExchange`
Use this for import/export of profile files.

Recommended first version:

- export a profile to a portable JSON file
- import from JSON with validation
- keep format versioning simple at first

#### 4. Implement `IProfileManager`
The manager should coordinate profile operations rather than making UI code talk to stores directly.

Suggested responsibilities:

- create profile
- clone profile
- rename profile
- delete profile
- select active profile
- save current changes

### Exit criteria
Move on when the application can persist configuration and profiles without the UI owning file logic.

---

## Phase 4: Wire WPF to the existing packages

### Goal
Use `Wpf` as the host and composition layer, not as the place where business logic lives.

### Work

#### 1. Set up startup composition
At startup:

- build the service provider
- register `Core`/`Engine`/`Infrastructure`/`Input.Windows` dependencies
- load configuration and profiles

#### 2. Initialize global hotkeys
The WPF app should own the native window handle and message hook.

Recommended flow:

- get the main window handle after initialization
- call `IGlobalHotkey.Initialize(handle)`
- register the selected app hotkey from configuration
- forward native messages into `HandleWindowMessage(...)`
- react to `HotkeyPressed` by toggling the engine

#### 3. Build minimum viable screens
Implement the smallest useful UI first.

Recommended order:

- profile list
- sequence list
- step editor
- selected hotkey display/editor
- start/stop status area

#### 4. Keep WPF thin
Do not move validation, persistence, or playback logic into code-behind beyond host integration.

### Exit criteria
Move on when the user can:

- choose a profile
- edit sequences
- save changes
- register a global hotkey
- start and stop playback through the UI

---

## Phase 5: Add profile editing quality-of-life features

### Goal
Make profile authoring comfortable enough that the app becomes truly usable.

### Work

#### 1. Replace free-text key entry in the UI
Avoid raw typing for key tokens wherever possible.

Recommended direction:

- dropdowns for common keys
- grouped categories such as letters, digits, function keys, navigation keys, modifiers
- use centralized key token constants from `Core`

#### 2. Add step templates
Support easy insertion of common actions like:

- press Enter
- left click
- right click
- wheel up/down one notch
- hold key then release

#### 3. Add profile duplication and undo-friendly editing
You do not need full undo immediately, but you should make it easy to duplicate sequences and profiles before editing.

#### 4. Add validation feedback in the editor
Show invalid or incomplete steps before the user tries to run them.

### Exit criteria
Move on when editing no longer depends on memorizing exact key tokens.

---

## Phase 6: Add recording and cursor features

### Goal
Expand the app from basic playback to richer automation authoring.

### Work

#### 1. Add mouse position support
This is the next natural `Input.Windows` expansion.

Potential additions:

- get current cursor position
- move cursor to absolute position
- move cursor relatively
- click at current or explicit position

This likely requires adding a cursor-related contract in `Core` and Win32 implementations in `Input.Windows`.

#### 2. Add input recording support
Recording is a separate feature and should not be forced into `InputSender`.

You will need to decide whether recording lives in:

- `Engine`
- a new package later
- or a dedicated Windows-input recording service

#### 3. Decide recorded step format
If mouse positions are recorded, define how they appear in `SequenceStep`.

Possible future directions:

- extend `SequenceStep`
- add optional coordinate properties
- or create a more flexible action payload model later

### Exit criteria
Move on when the app can record or at least capture cursor-based actions in a stable format.

---

## Phase 7: Harden the application

### Goal
Turn the working app into a reliable one.

### Work

#### 1. Expand tests
Focus tests on:

- sequence validation
- sequence runner behavior
- engine state transitions
- persistence correctness
- hotkey registration state logic where possible

#### 2. Improve exception handling and logging
Add useful logging around:

- profile loading/saving
- hotkey registration failures
- sequence execution start/stop
- invalid steps

#### 3. Define versioning and migration rules
As profile/config formats evolve, add a simple migration approach.

#### 4. Review DI boundaries
By this point, revisit service lifetimes and confirm they still make sense.

### Exit criteria
This phase is complete when the app can be used, debugged, and extended without fragile behavior.

---

## Guidance on key token usage right now

Until a centralized key token catalog is added, the safest usage pattern is:

- only use keys that are already supported by `KeyCodeMapper`
- avoid arbitrary text input for key names in the UI
- prefer fixed selections in the UI layer
- store exactly the token names expected by the mapper

Examples of safe current tokens include:

- `A` through `Z`
- `0` through `9`
- `F1` through `F12`
- `Enter`
- `Escape`
- `Space`
- `Tab`
- `Left`, `Right`, `Up`, `Down`
- `Ctrl`, `Control`, `Alt`, `Shift`
- `LeftCtrl`, `RightCtrl`, `LeftShift`, `RightShift`, `LeftAlt`, `RightAlt`
- `Win`, `LWin`, `RWin`

This is the biggest usability gap left at the current stopping point, and it should be addressed early in the next phase.

---

## Go/No-Go guidance for the current stopping point

### Go
Proceed from this checkpoint if your goal is to begin implementing:

- engine playback
- WPF hotkey hookup
- profile persistence
- step editor UI

The platform adapter layer is far enough along for those next steps.

### No-Go
Do not keep expanding `Input.Windows` right now unless you specifically need:

- cursor movement
- cursor position capture
- additional key mappings beyond the current supported catalog
- input recording

Those are future additions, not blockers for the next application phases.

---

## Recommended next concrete task

If continuing immediately after this checkpoint, the best next step is:

**Implement `ISequenceRunner` and the first execution validation path in `Engine`.**

That gives the rest of the solution something real to consume and keeps progress moving outward from the now-stable platform package.
