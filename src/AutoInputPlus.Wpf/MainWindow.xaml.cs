using System.ComponentModel;
using System.Windows;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Wpf.Services;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    private bool _allowClose;

    /// <summary>
    /// Initializes the main window.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

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

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        UpdateThemeMenuChecks(ThemeManager.CurrentTheme);
    }

    private void MainWindow_Closed(object? sender, System.EventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
    }

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

    private void LightBlueThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(AppTheme.LightBlue);
    }

    private void DarkBlueThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(AppTheme.DarkBlue);
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
}