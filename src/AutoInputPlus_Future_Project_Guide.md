# AutoInputPlus Future Project Guide

## Purpose

This document is the working implementation guide for the repository in its current state. It is written for any developer taking over the next phases of work and is meant to be used as a practical, incremental checklist rather than a redesign document.

The guide is based on the current solution structure and current code flow. It favors extending the existing implementation in small, safe steps instead of replacing major pieces all at once.

The priority order for backend work remains:

1. `AutoInputPlus.Core`
2. `AutoInputPlus.Input.Windows`
3. `AutoInputPlus.Engine`
4. `AutoInputPlus.Infrastructure`
5. `AutoInputPlus.Wpf` at the end

The WPF project is intentionally left near the end because the intended tray-first application flow depends on the lower layers already behaving correctly.

---

## Current intended application flow

The future application flow should build on the code that already exists:

1. The app starts as a tray-oriented Windows application.
2. Startup loads app configuration and available profiles.
3. The last active profile becomes the active profile in `IProfileManager`.
4. The selected global hotkey is initialized and registered against the WPF host window.
5. The engine remains either `Disabled` or `Ready` based on the saved app state.
6. A tray action or settings UI can enable or disable the engine.
7. The global hotkey does not enable the engine.
8. When the engine is already enabled, the global hotkey only toggles execution.
9. Execution uses either:
   - single-input mode using `TargetInputBinding`, or
   - sequence mode using the selected sequence in the active profile
10. Settings UI edits profiles, sequences, hotkeys, import/export, and app-level preferences.

That flow matches the current interfaces and existing `EngineState` design better than a design where the hotkey controls everything.

---

## Backend-first implementation order

## Phase 1 - Finish Core decisions that affect all other layers

### Goal

Freeze the remaining backend-facing behavior rules before deeper Engine and Infrastructure work begins.

### Current status

Most model shapes are already good. The missing work is not broad redesign; it is mainly finalizing behavior rules so later implementations do not contradict each other.

### Step 1. Confirm execution modes in `InputProfile`

`InputProfile` already supports two execution styles:

- single-input mode through `TargetInputBinding`
- sequence mode through `SequenceModeActive`, `SelectedSequenceIndex`, and `Sequences`

#### Work

Document and enforce these rules in code comments and engine validation:

- when `SequenceModeActive` is `false`, `TargetInputBinding` is the source of execution
- when `SequenceModeActive` is `true`, the engine uses `SelectedSequenceIndex`
- if `SelectedSequenceIndex` is out of range, execution should not start
- an empty sequence list in sequence mode should not start execution

#### Why this path

The model already contains both modes. Reusing that shape avoids creating a second execution model later.

### Step 2. Finalize `SequenceStep` execution rules

`SequenceStep` already has enough fields for a useful first version.

#### Work

Write the behavior rules directly into Engine tests and sequence execution code:

- disabled steps are skipped
- negative delays are invalid
- `KeyPress` and `MouseClick` execute one full action
- `KeyDown` and `MouseDown` can honor `HoldDuringDelay`
- `KeyUp` and `MouseUp` ignore `HoldDuringDelay`
- `MouseWheelDelta == 0` should be treated consistently

#### Recommended rule for wheel delta

Treat `MouseWheelDelta == 0` as invalid during sequence validation rather than silently running a no-op step.

#### Why this path

`InputSender.MouseWheel(0)` already no-ops at the platform layer, but sequence execution is a higher-level feature. A no-op step in a user-authored sequence is more likely to be a configuration mistake than intentional behavior.

### Step 3. Keep `InputKey` as the source of truth

The repository has already moved away from loose string key tokens. `InputKey` is the right source of truth now.

#### Work

Continue using `InputKey` everywhere backend-facing.
Do not reintroduce arbitrary string-based key entry into Core or Engine.

#### Why this path

This keeps mapping, validation, and UI pickers consistent, and it matches the already-implemented `KeyCodeMapper` approach.

### Step 4. Clarify scheduling model in profile behavior

`InputProfile` already contains:

- `ScheduleStartEnabled`
- `ScheduleStartTime`
- `ScheduleStopEnabled`
- `ScheduleStopTime`

#### Work

Use the first implementation as same-day local time scheduling owned by Engine.
Avoid cron-like or recurring schedule logic in the first pass.

#### Why this path

The existing profile fields are simple and user-facing. A local one-time start/stop interpretation is enough to make the feature useful without introducing a scheduler subsystem too early.

### Phase 1 exit checklist

- [ ] single-input and sequence-mode rules are explicitly documented in code/tests
- [ ] sequence step behavior rules are defined
- [ ] sequence validation rules are decided
- [ ] scheduling is treated as local profile-based start/stop timing only

---

## Phase 2 - Expand `Input.Windows` only where backend execution needs it

### Goal

Finish only the native features that block Engine behavior.

### Current status

`Input.Windows` is already in good shape for keyboard and mouse button execution. Only targeted additions should be made here.

### Step 1. Leave hotkey handling as single-registration

Current `IGlobalHotkey` and `WindowsHotkeyService` assume one active hotkey.

#### Work

Keep that design for now. Do not introduce multiple simultaneous global hotkeys until there is a concrete app need.

#### Why this path

The current app concept only requires one start/stop hotkey. Expanding to a hotkey manager would add complexity without current payoff.

### Step 2. Add targeted tests around hotkey lifecycle

#### Work

Add tests for:

- initialize with zero handle throws
- initialize with different handle after first init throws
- handle message returns false when not registered
- handle message returns false for wrong message or wrong hotkey id
- unregister with nothing registered returns false

#### Why this path

`WindowsHotkeyService` already contains meaningful stateful logic that is more important to test than adding more mapping tests.

### Step 3. Add tests around `InputSender` behavior boundaries

#### Work

Add tests for:

- invalid `InputKey` values throw through `ResolveVirtualKey`
- `MouseWheel(0)` returns without native send attempt
- unsupported mouse button enum values throw from `MouseButtonMapper`

If direct native isolation becomes awkward, keep the initial tests focused on the pure mapping classes first and add more `InputSender` tests after introducing a minimal internal send seam.

#### Why this path

The platform layer is already functionally central. Adding confidence here supports later engine work.

### Step 4. Add cursor movement only when a real sequence feature needs it

#### Work

Do not add cursor position or movement support yet unless the sequence editor requirements clearly demand it.

#### Why this path

The current profile and sequence models do not include pointer coordinates. Adding native movement before the Core model needs it creates dead code and forces model expansion too early.

### Phase 2 exit checklist

- [ ] hotkey lifecycle tests exist
- [ ] mapper edge-case tests exist
- [ ] input sender edge-case tests exist where practical
- [ ] no extra Win32 features were added without model support

---

## Phase 3 - Implement Engine execution properly

### Goal

Make `Engine` the place that actually interprets the active profile and executes the correct input path.

### Current status

The state machine direction is already correct. The missing piece is real execution and cancellation behavior.

### Step 1. Keep enable/disable separate from start/stop

`AutoInputEngine` already reflects the intended model.

#### Work

Preserve these rules:

- `EnableAsync()` controls whether execution is allowed at all
- `DisableAsync()` always prevents future hotkey-driven execution
- `StartAsync()` only starts execution when state is `Ready`
- `ToggleExecutionAsync()` does nothing when state is `Disabled`

#### Why this path

This matches the tray app goal where the app can be running but the automation engine is intentionally not armed.

### Step 2. Inject runtime dependencies into `AutoInputEngine`

The engine currently has no execution dependencies.

#### Work

Inject at least:

- `IProfileManager`
- `ISequenceRunner`
- `IInputSender`

A minimal first version can keep single-input execution inside `AutoInputEngine` and delegate sequence execution to `ISequenceRunner`.

#### Suggested flow

- read `ActiveProfile`
- if `SequenceModeActive` is true, validate the selected sequence and call `ISequenceRunner.ExecuteAsync(...)`
- otherwise execute the target binding repeatedly according to the profile settings

#### Why this path

It reuses existing interfaces and keeps the engine as orchestration instead of pushing profile interpretation into WPF.

### Step 3. Add a private execution loop for single-input mode

The current profile already supports repeated non-sequence execution:

- interval-based repetition
- run-until-stop mode
- count-based stopping
- hold target option

#### Work

Implement a private async execution loop in `AutoInputEngine` for single-input mode.

#### Recommended first-pass behavior

- if `HoldTargetEnabled` is false:
  - send one press or click per iteration
  - delay by `IntervalMilliseconds` between iterations when continuing
- if `HoldTargetEnabled` is true:
  - send down once at start
  - wait until stop or count completion logic ends
  - send up once during cleanup

#### Why this path

This aligns with the existing profile fields and avoids inventing a second abstraction for simple repeating input.

### Step 4. Add cancellation support

#### Work

Add a `CancellationTokenSource` inside `AutoInputEngine` for the currently active run.
Add a small synchronization mechanism such as `SemaphoreSlim` around start/stop transitions.

#### Minimum expected behavior

- repeated `StartAsync()` while running returns safely
- `StopAsync()` cancels active work and returns the engine to `Ready`
- `DisableAsync()` cancels active work before going to `Disabled`
- completion cleanup is centralized so down/held inputs can be released safely

#### Why this path

The app is asynchronous by nature and the future tray/WPF host must remain responsive.

### Step 5. Implement `SequenceRunner`

`SequenceRunner` currently only checks whether sequence mode is active.

#### Work

Inject `IInputSender` into `SequenceRunner`.
Add step validation and ordered execution.

#### Recommended implementation order

1. Validate the sequence object itself.
2. Iterate steps in order.
3. Skip disabled steps.
4. Apply `DelayBeforeMilliseconds`.
5. Execute action.
6. Apply `DelayAfterMilliseconds`.
7. Respect cancellation between steps and after delays.

#### Action mapping

- `KeyPress` -> `IInputSender.KeyPress(step.Key.Value)`
- `KeyDown` -> `IInputSender.KeyDown(step.Key.Value)`
- `KeyUp` -> `IInputSender.KeyUp(step.Key.Value)`
- `MouseClick` -> `IInputSender.MouseClick(step.MouseButton.Value)`
- `MouseDown` -> `IInputSender.MouseDown(step.MouseButton.Value)`
- `MouseUp` -> `IInputSender.MouseUp(step.MouseButton.Value)`
- `MouseWheel` -> `IInputSender.MouseWheel(step.MouseWheelDelta)`

#### Hold behavior recommendation

For the first pass:

- use `HoldDuringDelay` only for `KeyDown` and `MouseDown`
- when `HoldDuringDelay` is true, apply post-delay before the method returns control to the next step
- do not auto-insert a matching `KeyUp` or `MouseUp`

That means authored sequences remain explicit. If a developer wants a press-and-hold followed by release, the sequence should contain both steps.

#### Why this path

This keeps the sequence model transparent. Hidden automatic release behavior would make sequences harder to reason about.

### Step 6. Add engine tests before adding more features

#### Work

Add tests for:

- initial state is `Disabled`
- enable moves to `Ready`
- start throws when disabled
- toggle while disabled returns without starting
- start while ready moves to running path
- stop from running returns to ready
- disable from running cancels and goes to disabled
- sequence mode delegates to `ISequenceRunner`
- single-input mode uses `TargetInputBinding`

#### Why this path

Engine is the new center of behavior. It should gain real tests as soon as it gains real logic.

### Phase 3 exit checklist

- [ ] engine runs both single-input and sequence mode
- [ ] start/stop/disable behavior is stable
- [ ] held-input cleanup is safe
- [ ] engine tests cover real state transitions and execution paths

---

## Phase 4 - Implement Infrastructure persistence in small steps

### Goal

Persist app configuration and profiles without changing the current public contracts.

### Current status

The interfaces are good, but the implementations are still placeholders.

### Step 1. Standardize storage paths

#### Work

Use a single application data root under `%AppData%` or `%LocalAppData%`.
The first pass should centralize file paths in one internal helper inside Infrastructure.

#### Recommended layout

```text
AutoInputPlus/
  config.json
  profiles/
    {profileId}.json
```

#### Why this path

It keeps persistence obvious, easy to inspect manually, and easy to back up or delete during development.

### Step 2. Implement `AppConfigurationStore`

#### Work

Persist `AppConfiguration` to `config.json`.
On load:

- if file does not exist, return a default `AppConfiguration`
- if file exists but is invalid, return default configuration and preserve the option to log the issue later

#### Recommended first-pass fields to persist

- `DataFolderPath`
- `LastActiveProfileId`
- `RunOnSystemStartup`

#### Why this path

The interface is already minimal and does not require additional contracts to become useful.

### Step 3. Implement `InputProfileStore`

#### Work

Persist one profile per file keyed by `ProfileId`.
Implement:

- `SaveProfileAsync`
- `LoadProfileAsync`
- `GetAllAsync`
- `ExistsAsync`
- `DeleteProfileAsync`

#### Recommended behavior

- saving a profile creates or overwrites `{profileId}.json`
- `GetAllAsync()` loads and returns all readable profiles
- malformed profile files are skipped during `GetAllAsync()` but should not crash the entire load
- `LoadProfileAsync(Guid)` should throw a clear file-not-found style exception when missing

#### Why this path

One-profile-per-file keeps profile management simple and works well with future import/export.

### Step 4. Add JSON serializer options once and reuse them

#### Work

Define one internal `JsonSerializerOptions` instance for Infrastructure profile/config persistence.
Use it consistently for stores and later for profile exchange.

#### Why this path

This prevents accidental drift between on-disk persistence and import/export serialization.

### Step 5. Delay registry startup integration until WPF host work

`RunOnSystemStartup` exists in `AppConfiguration`, but changing the registry should not happen in `AppConfigurationStore` yet.

#### Work

For now, treat `RunOnSystemStartup` as a persisted preference.
Registry integration can be added later from the WPF host when the UI for this option exists.

#### Why this path

Saving the preference and applying the OS integration are separate concerns. Keeping them separate avoids making the persistence layer own machine-specific behavior.

### Step 6. Add infrastructure tests around real JSON behavior

#### Work

Add tests for:

- save then load config roundtrip
- save then load profile roundtrip
- get all profiles returns only valid files
- delete removes the profile file
- exists reflects disk state
- missing config returns default config

#### Why this path

This layer should be testable with temporary folders and does not need heavy mocking.

### Phase 4 exit checklist

- [ ] configuration persists to disk
- [ ] profiles persist to disk
- [ ] invalid files are handled safely
- [ ] infrastructure tests cover real file behavior

---

## Phase 5 - Implement profile exchange using the same model shape

### Goal

Make import/export useful without inventing a second profile format.

### Current status

`ProfileExchange` exists but is not implemented.

### Step 1. Start with portable JSON export

#### Work

Use a plain JSON representation of `InputProfile` for the first export format.
Optionally wrap it in a small envelope later if versioning becomes necessary.

#### Recommended first pass

- export returns serialized JSON string
- import accepts serialized JSON string
- validation tries to parse the expected structure

#### Why this path

The project already needs JSON for persistence. Reusing the same shape reduces duplicate logic.

### Step 2. Add lightweight format versioning only when needed

#### Work

If a version marker is added, keep it minimal, for example:

```json
{
  "formatVersion": 1,
  "profile": { ... }
}
```

Do not add compression, custom encoding, or binary packing in the first pass.

#### Why this path

Human-readable export is easier to debug, easier to migrate, and good enough for current app scope.

### Step 3. Validate imported profile shape before accepting it

#### Work

At minimum, validate:

- profile object exists
- profile name is present or defaultable
- sequence indices are sane
- sequence steps are structurally valid
- target binding contains exactly one source

#### Why this path

Import is an entry point for malformed data. Validation here prevents corrupted profiles from reaching runtime execution.

### Phase 5 exit checklist

- [ ] export returns a valid portable profile string
- [ ] import validates before returning a profile
- [ ] validation rejects malformed payloads

---

## Phase 6 - Add WPF host plumbing after backend behavior is stable

### Goal

Turn WPF into the Windows shell and settings surface, not the business-logic layer.

### Current status

WPF currently only boots the container and shows `MainWindow`.

### Step 1. Add application bootstrap flow

#### Work

During startup:

1. build service provider
2. load configuration from `IAppConfigurationStore`
3. load all profiles from `IInputProfileStore`
4. set active profile in `IProfileManager`
5. initialize engine enabled/disabled state from the chosen startup policy
6. show tray presence and only show settings window when requested

#### Why this path

It matches the tray-first app concept and keeps startup behavior deterministic.

### Step 2. Add tray integration before building a large settings UI

#### Work

Create the tray icon and add a context menu with at least:

- open settings
- enable engine
- disable engine
- start execution
- stop execution
- exit

#### Recommended engine menu behavior

- enable/disable controls engine armed state
- start/stop controls execution only when allowed
- menu text reflects current `EngineState`

#### Why this path

The tray shell is the main operating model of the application. It should exist before investing heavily in editor screens.

### Step 3. Wire `IGlobalHotkey` into the WPF host window

#### Work

After the host window handle exists:

- call `Initialize(handle)` on `IGlobalHotkey`
- register the active profile hotkey
- hook WPF window message forwarding to `HandleWindowMessage(...)`
- on `HotkeyPressed`, call `IEngine.ToggleExecutionAsync()`

#### Important rule

Do not use the hotkey to call `EnableAsync()`.
It should only toggle execution when the engine is already enabled.

#### Why this path

This exactly matches the current engine design and the intended tray app semantics.

### Step 4. Keep the first settings UI narrow

#### Work

The first real WPF settings surface should only need:

- active profile selector
- start/stop hotkey editor
- single-input target editor
- sequence mode toggle
- sequence list and step list
- import/export actions
- interval and stop-count settings
- schedule start/stop controls

#### Why this path

These are the fields already supported by the current models. Building them first makes the UI exercise existing backend capabilities instead of inventing new ones.

### Step 5. Keep view logic separate from backend logic

#### Work

Use WPF to bind and dispatch commands, but keep:

- input execution in Engine
- persistence in Infrastructure
- native hotkey and input in Input.Windows

Code-behind should mainly own host-specific concerns such as tray icon lifecycle, native message forwarding, and opening windows.

#### Why this path

That keeps future UI changes from destabilizing backend behavior.

### Phase 6 exit checklist

- [ ] tray icon exists with minimum context menu actions
- [ ] hotkey registration is wired to the host window
- [ ] settings window can edit and save the active profile
- [ ] WPF remains a host/presentation layer

---

## Suggested detailed task list by project

## `AutoInputPlus.Core`

### Near-term tasks

- [ ] add XML comments where backend behavior is still ambiguous
- [ ] define and document rules for invalid sequence steps
- [ ] keep `InputKey` enum as the supported key catalog
- [ ] avoid adding UI-only concerns to models

### Add only when needed

- [ ] profile-level `IsEnabled` flag, only if profile selection UX needs it
- [ ] sequence-level enabled flag, only if multiple saved sequences need selective activation beyond selected index
- [ ] cursor position data, only if sequence authoring explicitly requires pointer movement

## `AutoInputPlus.Input.Windows`

### Near-term tasks

- [ ] hotkey lifecycle tests
- [ ] mapper edge-case tests
- [ ] input sender behavior tests where practical
- [ ] preserve one-hotkey design

### Add only when needed

- [ ] cursor movement send support
- [ ] cursor coordinate reads
- [ ] extra mouse buttons beyond left/right/middle, only after Core model expansion

## `AutoInputPlus.Engine`

### Near-term tasks

- [ ] inject dependencies into `AutoInputEngine`
- [ ] implement single-input execution loop
- [ ] implement cancellation and cleanup
- [ ] implement real `SequenceRunner`
- [ ] add engine tests

### Add only when needed

- [ ] scheduled start background timer support
- [ ] richer error reporting surface
- [ ] progress or telemetry events for UI display

## `AutoInputPlus.Infrastructure`

### Near-term tasks

- [ ] centralize file paths
- [ ] implement config store
- [ ] implement profile store
- [ ] define serializer options once
- [ ] add roundtrip file tests

### Add only when needed

- [ ] backup file strategy
- [ ] migration helpers for future profile schema changes
- [ ] file locking or atomic write hardening after multi-process needs appear

## `AutoInputPlus.Wpf`

### Near-term tasks after backend stability

- [ ] tray shell
- [ ] hotkey host integration
- [ ] minimal settings UI
- [ ] active profile loading and saving
- [ ] engine state display

### Add only when needed

- [ ] richer editor UX
- [ ] drag/drop step reordering UI polish
- [ ] onboarding, tips, or profile templates

---

## Suggested implementation notes for specific existing classes

## `AutoInputEngine`

Use the existing class as the state owner. Expand it rather than replacing it.

### Small-step plan

1. add injected dependencies
2. add a private cancellation token source
3. add a private execution task reference
4. add a lightweight transition lock
5. implement single-input execution
6. delegate sequence execution to `ISequenceRunner`
7. add cleanup logic for held down key or mouse button release

### Avoid

- moving state management into WPF
- letting `Input.Windows` decide profile logic
- creating a second engine class for simple versus sequence mode

## `SequenceRunner`

Use the current class and add `IInputSender` plus validation.

### Small-step plan

1. inject `IInputSender`
2. add private validation helper methods
3. execute delays and actions in order
4. add cancellation-aware delay handling
5. add tests with a fake `IInputSender`

### Avoid

- making sequence rules depend on the UI editor
- auto-correcting malformed steps silently during execution

## `ProfileManager`

Keep it as the active profile holder for now.

### Small-step plan

1. keep `SetActiveProfile(...)`
2. add small helper methods only when the UI actually needs them, such as selecting a sequence by index
3. keep profile persistence orchestration elsewhere unless it clearly belongs here later

### Avoid

- turning `ProfileManager` into the disk persistence class
- expanding it into a catch-all application coordinator

## `AppConfigurationStore` and `InputProfileStore`

Use these current classes directly. Add internal helpers under Infrastructure if path or serializer reuse starts repeating.

### Small-step plan

1. define app data root helper
2. add serializer options helper
3. implement safe read and write methods
4. add temp-folder based tests

### Avoid

- introducing a database
- pushing file IO into WPF startup code

## `ProfileExchange`

Use the current class as the single import/export path.

### Small-step plan

1. reuse profile JSON serialization
2. add import validation
3. add export/import tests
4. only add format versioning when required

### Avoid

- custom binary formats
- compressed share strings in the first pass
- different schemas for persistence versus export unless a strong reason appears later

---

## Definition of done for the backend before serious WPF work

Backend readiness is reached when all of the following are true:

- [ ] engine can be enabled and disabled independently from execution start/stop
- [ ] engine can execute single-input mode from the active profile
- [ ] engine can execute the selected sequence from the active profile
- [ ] engine can stop cleanly and release held inputs
- [ ] app configuration saves and loads from disk
- [ ] profiles save and load from disk
- [ ] profile import/export works with validation
- [ ] global hotkey wiring is ready for host integration
- [ ] Engine and Infrastructure have real tests instead of placeholder tests

At that point, WPF can move from shell-only to actual tray app plus settings editor without forcing backend redesign.

---

## Final guidance

The repository is already organized in a good direction. The next work should focus on finishing execution and persistence, not reshaping the whole solution.

The strongest existing decisions worth preserving are:

- app-level contracts in `Core`
- Windows-specific input in `Input.Windows`
- engine state ownership in `Engine`
- persistence behind interfaces in `Infrastructure`
- WPF as host and settings surface rather than business logic

The most effective next milestone is:

1. finish Engine execution
2. finish Infrastructure persistence
3. add tests around both
4. then wire the tray-first WPF host

That order keeps the codebase incremental, testable, and aligned with the intended final application behavior.
