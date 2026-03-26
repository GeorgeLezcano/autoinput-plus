# Autoinput Plus (Make this read me better eventually...)

## Projects
- ### Core: Contains models, interfaces, constants and shared utilities.
- ### Engine: Contains the simulation and runtime logic of the application. Source of truth for states.
- ### Infrastructure: Constains the framework for the application, such as persistence, networking or authentication (if any)
- ### Input.Windows: Constains code to handle key inputs in windows.
- ### Wpf: This is the UI project using C# WPF

# TODO
- Make InputProfileStore.GetAllAsync() resilient to a bad/corrupt profile file so one broken JSON file does not kill profile loading..
- Implement startup bootstrap flow and Windows-startup registry service.
- Implement schedule behavior in AutoInputEngine.

### Import/Export stuff
- Complete import (actually use the profile after validation)
- Save imported profile
- Set imported profile as active
- Refresh UI/profile list after import
- Handle duplicate profile names

# Known Bugs/Issues
- IEngine doesnt expose a status change for the UI to subscribe, explore options so it updates sa global hotkeys are pressed. Right now its only dependent on Checkbox or Form startup, nothing else triggers the updates.
- Fix ShouldStopAfterExecution() so RunUntilStopActive wins over count, and clamp count with Math.Max(1, ...), not Math.Max(0, ...).
