# AutoInput Plus User Guide

AutoInput Plus automates repeated keyboard and mouse input. You can create multiple profiles for different tasks, run a single repeated input, or build a sequence of inputs that runs in order.

## Getting Started

1. Select a profile from the **Profile** list, or click **New** to create one.
2. Open the **Settings** tab and choose **Single Input** or **Sequence Mode**.
3. Set a **Start / Stop Hotkey**.
4. Configure the input or sequence you want to run.
5. Make sure the engine is **Enabled**.
6. Press your Start / Stop Hotkey to begin. Press it again to stop when using **Run until manually stopped**.

Changes to profiles are saved automatically as you make them.

## Profiles

Profiles keep their own automation settings and sequences. Use the profile controls at the top of the window to switch between them.

- **New** creates a new profile.
- **Rename** changes the name of the current profile.
- **Delete** removes the current profile.

Switching profiles changes which settings and sequences are active. AutoInput Plus remembers the last active profile for the next time the application starts.

### Importing and Exporting Profiles

Use **Tools > Export Active Profile** to create an encoded profile string that you can copy and save or share.

Use **Tools > Import Profile** to paste an exported profile string and add it as a new profile.

## Settings

The **Settings** tab controls how the current profile runs.

### Start / Stop Hotkey

Click **Set Hotkey**, then press the keyboard shortcut you want to use to start and stop the active profile.

The hotkey works globally, so you can use it while another application is active.

### Single Input Mode

Use **Single Input** when you want AutoInput Plus to repeat or hold one keyboard key or mouse button.

Click **Set Target**, then press the keyboard key or mouse button you want AutoInput Plus to use.

#### Repeating an Input

Leave **Hold target instead of repeating input** unchecked. **Interval (ms)** controls the delay between each input. Lower values repeat the input more quickly.

You can choose to:

- **Run until manually stopped** — continues until you press the Start / Stop Hotkey again.
- **Run for set count** — stops automatically after the configured number of inputs.

#### Holding an Input

Enable **Hold target instead of repeating input** to keep the selected key or mouse button held down until you stop the profile.

When Hold is enabled, the profile runs until manually stopped.

## Sequence Mode

Use **Sequence Mode** when you want several keyboard or mouse actions to run in order.

Select the **Sequence** tab to create and edit sequences for the current profile.

### Managing Sequences

The **Saved Sequences** list contains the sequences stored in the current profile.

- **New** creates a sequence.
- **Rename** changes the selected sequence's name.
- **Delete** removes the selected sequence.

Select a sequence from the list to make it the active sequence for the profile.

### Adding Steps

Click **Add** to add a step to the selected sequence. Click the step's **Target** button, then press the keyboard key or mouse button you want that step to use.

Each step can be configured with:

- **Hold** — holds the selected input instead of pressing or clicking it once.
- **Hold Duration (ms)** — how long a held input stays down before being released.
- **Delay After (ms)** — how long AutoInput Plus waits after the step before moving to the next one.

Click **Remove** to delete the selected step.

When the sequence runs, enabled steps are performed from top to bottom. After the final step, the sequence either runs again or stops according to the **Run Behavior** selected on the Settings tab.

## Engine Controls

The engine controls whether AutoInput Plus is allowed to run automation.

- **Enabled** — allows the active profile to be started with its Start / Stop Hotkey.
- **Status** — shows the current engine state.
- **Run on Windows startup** — starts AutoInput Plus automatically when you sign in to Windows.

Disabling the engine prevents the Start / Stop Hotkey from starting automation.

## System Tray

AutoInput Plus stays available from the Windows system tray.

Left-click the tray icon to open the application to the **Settings** tab. Right-click it to view the current status, open **Settings** or **About**, or exit AutoInput Plus.

Closing the main window hides it instead of exiting the application. To fully close AutoInput Plus, right-click the tray icon and choose **Exit**.

## Themes

Use the **Themes** menu to change the appearance of AutoInput Plus. The selected theme is remembered the next time the application starts.

## Tips

- Create separate profiles when you need different hotkeys, targets, timings, or sequences.
- Use **Single Input** for simple repeating or held inputs and **Sequence Mode** for multi-step actions.
- Make sure the engine is **Enabled** before trying to start a profile with its hotkey.
- When using very short intervals or delays, test the profile carefully before using it in another application.
