# Autoinput Plus (Make this read me better eventually...)

## Projects
- ### Core: Contains models, interfaces, constants and shared utilities.
- ### Engine: Contains the simulation and runtime logic of the application. Source of truth for states.
- ### Infrastructure: Constains the framework for the application, such as persistence, networking or authentication (if any)
- ### Input.Windows: Constains code to handle key inputs in windows.
- ### Wpf: This is the UI project using C# WPF

# TODO
- Fix ShouldStopAfterExecution() so RunUntilStopActive wins over count, and clamp count with Math.Max(1, ...), not Math.Max(0, ...).
- Make InputProfileStore.GetAllAsync() resilient to a bad/corrupt profile file so one broken JSON file does not kill profile loading.
- Later: move InputBindingJsonConverter out of the store class for cleanliness.
- Later: implement startup bootstrap flow and Windows-startup registry service.
- Later: Implement schedule behavior in AutoInputEngine.