using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Wpf.Services;
using AutoInputPlus.Wpf.Views.Dialogs;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    private bool _allowClose;
    private bool _isUpdatingEngineUi;
    private readonly IEngine _engine;
    private readonly IProfileExchange _profileExchange;
    private readonly IProfileManager _profileManager;
    private readonly IInputProfileStore _inputProfileStore;

    /// <summary>
    /// Initializes the main window.
    /// </summary>
    public MainWindow(
        IEngine engine,
        IProfileExchange profileExchange,
        IProfileManager profileManager,
        IInputProfileStore inputProfileStore)
    {
        ArgumentNullException.ThrowIfNull(engine);

        InitializeComponent();

        _engine = engine;
        _profileExchange = profileExchange;
        _profileManager = profileManager;
        _inputProfileStore = inputProfileStore;

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
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

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        UpdateThemeMenuChecks(ThemeManager.CurrentTheme);
        RefreshEngineUi();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
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

        // TODO This should already exist at first startup. Rethink this part and where it goes...
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


    private void ImportProfile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ImportProfileWindow(_profileExchange)
        {
            Owner = this
        };

        dialog.ShowDialog();
    }

    private async void EngineEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingEngineUi)
        {
            return;
        }

        await _engine.EnableAsync();
        RefreshEngineUi();
    }

    private async void EngineEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingEngineUi)
        {
            return;
        }

        await _engine.DisableAsync();
        RefreshEngineUi();
    }

    #endregion

    #region Helpers

    private void ThemeManager_ThemeChanged(AppTheme appTheme)
    {
        UpdateThemeMenuChecks(appTheme);
    }

    private void UpdateThemeMenuChecks(AppTheme appTheme)
    {
        if (LightBlueThemeMenuItem is null || DarkBlueThemeMenuItem is null)
        {
            return;
        }

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

            switch (_engine.State) //TODO Consider moving this colors somplece else...
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