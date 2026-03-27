using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Wpf.Services;
using AutoInputPlus.Wpf.Views.Dialogs;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    private bool _allowClose;
    private bool _isUpdatingEngineUi;
    private bool _isUpdatingStartupUi;
    private bool _isUpdatingProfilesUi;
    private HwndSource? _windowSource;
    private readonly IEngine _engine;
    private readonly IProfileExchange _profileExchange;
    private readonly IProfileManager _profileManager;
    private readonly IInputProfileStore _inputProfileStore;
    private readonly IAppConfigurationStore _appConfigurationStore;
    private readonly IStartupRegistrationService _startupRegistrationService;
    private readonly IGlobalHotkey _globalHotkey;

    private AppConfiguration _appConfiguration = new();

    /// <summary>
    /// Initializes the main window.
    /// </summary>
    public MainWindow(
        IEngine engine,
        IProfileExchange profileExchange,
        IProfileManager profileManager,
        IInputProfileStore inputProfileStore,
        IAppConfigurationStore appConfigurationStore,
        IStartupRegistrationService startupRegistrationService,
        IGlobalHotkey globalHotkey)
    {
        ArgumentNullException.ThrowIfNull(engine);

        InitializeComponent();

        _engine = engine;
        _profileExchange = profileExchange;
        _profileManager = profileManager;
        _inputProfileStore = inputProfileStore;
        _appConfigurationStore = appConfigurationStore;
        _startupRegistrationService = startupRegistrationService;
        _globalHotkey = globalHotkey;

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
        SourceInitialized += MainWindow_SourceInitialized;
    }

    #region Tabs

    /// <summary>
    /// Shows the main window and selects the settings tab.
    /// </summary>
    public void ShowSettingsTab()
    {
        ShowWindow();
        RootTabControl.SelectedItem = SettingsTab;
    }

    /// <summary>
    /// Shows the main window and selects the sequence tab.
    /// </summary>
    public void ShowSequenceTab()
    {
        ShowWindow();
        RootTabControl.SelectedItem = SequenceTab;
    }

    /// <summary>
    /// Shows the main window and selects the about tab.
    /// </summary>
    public void ShowAboutTab()
    {
        ShowWindow();
        RootTabControl.SelectedItem = AboutTab;
    }

    #endregion

    #region Lifecycle

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

        _appConfiguration = await _appConfigurationStore.LoadConfigurationAsync();

        SettingsTabViewContent.SetContext(_profileManager, _inputProfileStore);

        UpdateThemeMenuChecks(ThemeManager.CurrentTheme);
        RefreshEngineUi();
        LoadStartupRegistrationUi();
        await LoadProfilesAsync();
        LoadSettingsTabFromActiveProfile();
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        _windowSource = PresentationSource.FromVisual(this) as HwndSource;

        if (_windowSource is null)
        {
            return;
        }

        _windowSource.AddHook(WndProc);
        _globalHotkey.Initialize(_windowSource.Handle);
        _globalHotkey.HotkeyPressed += GlobalHotkey_HotkeyPressed;
        RegisterActiveProfileHotkey();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
        _globalHotkey.HotkeyPressed -= GlobalHotkey_HotkeyPressed;

        if (_windowSource is not null)
        {
            _windowSource.RemoveHook(WndProc);
            _windowSource = null;
        }
    }

    #endregion

    #region Menu Handlers

    private void LightBlueThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(AppTheme.LightBlue);
    }

    private void DarkBlueThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(AppTheme.DarkBlue);
    }

    private void OpenDataFolderMenuItem_Click(object sender, RoutedEventArgs e)
    {
        string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        appDataDirectory = Path.Combine(appDataDirectory, "AutoInputPlus");

        if (!Directory.Exists(appDataDirectory))
        {
            Directory.CreateDirectory(appDataDirectory);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = appDataDirectory,
            UseShellExecute = true,
            Verb = "open"
        });
    }

    private async void ExportProfile_Click(object sender, RoutedEventArgs e)
    {
        InputProfile activeProfile = _profileManager.ActiveProfile;
        string exportedProfile = await _profileExchange.ExportProfileAsync(activeProfile);

        var dialog = new ExportProfileWindow(exportedProfile)
        {
            Owner = this
        };

        dialog.ShowDialog();
    }

    private async void ImportProfile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ImportProfileWindow(_profileExchange)
        {
            Owner = this
        };

        bool? dialogResult = dialog.ShowDialog();

        if (dialogResult != true)
        {
            return;
        }

        InputProfile importedProfile = await _profileExchange.ImportProfileAsync(dialog.EncodedProfile);
        await _inputProfileStore.SaveProfileAsync(importedProfile);

        _profileManager.SetActiveProfile(importedProfile);
        await SaveLastActiveProfileAsync(importedProfile);
        await LoadProfilesAsync();
        LoadSettingsTabFromActiveProfile();
        RegisterActiveProfileHotkey();
    }

    private async void EngineEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingEngineUi)
        {
            return;
        }

        await _engine.EnableAsync();
        _appConfiguration.EngineEnabled = true;
        await _appConfigurationStore.SaveConfigurationAsync(_appConfiguration);
        RefreshEngineUi();
    }

    private async void EngineEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingEngineUi)
        {
            return;
        }

        await _engine.DisableAsync();
        _appConfiguration.EngineEnabled = false;
        await _appConfigurationStore.SaveConfigurationAsync(_appConfiguration);
        RefreshEngineUi();
    }

    private void RunOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingStartupUi)
        {
            return;
        }

        _startupRegistrationService.SetEnabled(true);
    }

    private void RunOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingStartupUi)
        {
            return;
        }

        _startupRegistrationService.SetEnabled(false);
    }

    private async void ProfilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingProfilesUi || ProfilesComboBox.SelectedItem is not InputProfile selectedProfile)
        {
            return;
        }

        _profileManager.SetActiveProfile(selectedProfile);
        await SaveLastActiveProfileAsync(selectedProfile);
        LoadSettingsTabFromActiveProfile();
        RegisterActiveProfileHotkey();
    }

    private void NewProfileButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show($"Feature '{nameof(NewProfileButton_Click)}' not implemented");
    }

    private void RenameProfileButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show($"Feature '{nameof(RenameProfileButton_Click)}' not implemented");
    }

    private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show($"Feature '{nameof(DeleteProfileButton_Click)}' not implemented");
    }

    #endregion

    #region Helpers

    private async void ThemeManager_ThemeChanged(AppTheme appTheme)
    {
        UpdateThemeMenuChecks(appTheme);
        _appConfiguration.AppTheme = appTheme;
        await _appConfigurationStore.SaveConfigurationAsync(_appConfiguration);
    }

    private void UpdateThemeMenuChecks(AppTheme appTheme)
    {
        LightBlueThemeMenuItem.IsChecked = appTheme == AppTheme.LightBlue;
        DarkBlueThemeMenuItem.IsChecked = appTheme == AppTheme.DarkBlue;
    }

    private void RefreshEngineUi()
    {
        _isUpdatingEngineUi = true;

        try
        {
            EngineEnabledCheckBox.IsChecked = _engine.State != EngineState.Disabled;
            EngineStatusTextBlock.Text = _engine.State.ToString();

            Brush badgeBackground;
            Brush badgeForeground;

            switch (_engine.State)
            {
                case EngineState.Disabled:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(229, 231, 235));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(55, 65, 81));
                    break;

                case EngineState.Ready:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(220, 252, 231));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(22, 101, 52));
                    break;

                case EngineState.Running:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(219, 234, 254));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(30, 64, 175));
                    break;

                case EngineState.Scheduled:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(255, 237, 213));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(154, 52, 18));
                    break;

                case EngineState.Error:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(254, 226, 226));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(153, 27, 27));
                    break;

                default:
                    badgeBackground = new SolidColorBrush(Color.FromRgb(229, 231, 235));
                    badgeForeground = new SolidColorBrush(Color.FromRgb(55, 65, 81));
                    break;
            }

            EngineStatusBadge.Background = badgeBackground;
            EngineStatusTextBlock.Foreground = badgeForeground;
        }
        finally
        {
            _isUpdatingEngineUi = false;
        }
    }

    private void LoadStartupRegistrationUi()
    {
        _isUpdatingStartupUi = true;

        try
        {
            RunOnStartupCheckBox.IsChecked = _startupRegistrationService.IsEnabled();
        }
        finally
        {
            _isUpdatingStartupUi = false;
        }
    }

    private async Task LoadProfilesAsync()
    {
        IReadOnlyList<InputProfile> storedProfiles = await _inputProfileStore.GetAllAsync();
        List<InputProfile> profiles = storedProfiles.Count > 0
            ? [.. storedProfiles]
            : [_profileManager.ActiveProfile];

        _isUpdatingProfilesUi = true;

        try
        {
            ProfilesComboBox.ItemsSource = profiles;
            ProfilesComboBox.DisplayMemberPath = nameof(InputProfile.Name);

            InputProfile activeProfile = _profileManager.ActiveProfile;
            InputProfile? matchingProfile = profiles.FirstOrDefault(p => p.ProfileId == activeProfile.ProfileId);

            ProfilesComboBox.SelectedItem = matchingProfile ?? profiles[0];
        }
        finally
        {
            _isUpdatingProfilesUi = false;
        }
    }

    private async Task SaveLastActiveProfileAsync(InputProfile profile)
    {
        bool profileExists = await _inputProfileStore.ExistsAsync(profile.ProfileId);
        _appConfiguration.LastActiveProfileId = profileExists ? profile.ProfileId : null;
        await _appConfigurationStore.SaveConfigurationAsync(_appConfiguration);
    }

    private void LoadSettingsTabFromActiveProfile()
    {
        SettingsTabViewContent.LoadProfile(_profileManager.ActiveProfile);
    }

    private void RegisterActiveProfileHotkey()
    {
        try
        {
            _globalHotkey.UnregisterHotKey();

            if (_profileManager.ActiveProfile.StartStopHotkey is null)
            {
                return;
            }

            _globalHotkey.RegisterHotKey(_profileManager.ActiveProfile.StartStopHotkey);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.Message,
                "Hotkey Registration Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void GlobalHotkey_HotkeyPressed(object? sender, EventArgs e)
    {
        await _engine.ToggleExecutionAsync();
        Dispatcher.Invoke(RefreshEngineUi);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        _ = hwnd;

        handled = _globalHotkey.HandleWindowMessage((uint)msg, wParam, lParam);
        return IntPtr.Zero;
    }

    private void ShowWindow()
    {
        if (!IsVisible)
        {
            Show();
        }

        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
        Topmost = true;
        Topmost = false;
        Focus();
    }

    /// <summary>
    /// Allows the window to close during full application shutdown.
    /// </summary>
    public void PrepareForExit()
    {
        _allowClose = true;
    }

    /// <summary>
    /// Handles window closing.
    /// </summary>
    /// <param name="e">Closing event data.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_allowClose)
        {
            e.Cancel = true;
            Hide();
            return;
        }

        base.OnClosing(e);
    }

    #endregion
}