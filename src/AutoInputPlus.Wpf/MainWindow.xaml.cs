using System.ComponentModel;
using System.Windows;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    // TODO In vscode, A lot of method are red or not recognized. Figure out if there is a way to support this in the IDE.
    // Application still runs and compiles, its just visual. Maybe consider using Visual Studio instead?

    private bool _allowClose;

    /// <summary>
    /// Initializes the main window.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
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
    /// Shows the main window and selects the about tab.
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
    /// <param name="e">
    /// Closing event data.
    /// </param>
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