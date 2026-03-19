using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Extensions.DependencyInjection;
using Forms = System.Windows.Forms;
using WpfApplication = System.Windows.Application;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Manages the system tray icon and related interactions.
/// </summary>
public sealed class TrayIconManager : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NotifyIcon _notifyIcon;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrayIconManager"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    /// The application service provider.
    /// </param>
    public TrayIconManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _notifyIcon = new NotifyIcon
        {
            Icon = LoadTrayIcon() ?? SystemIcons.Application,
            Visible = true,
            Text = "AutoInput Plus", // TODO No hardcode
            ContextMenuStrip = BuildContextMenu()
        };

        _notifyIcon.MouseClick += OnNotifyIconMouseClick;
    }

    private void OnNotifyIconMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ShowSettings();
        }
    }

    private ContextMenuStrip BuildContextMenu() // TODO These will be someplace else, later language aware....
    {
        var menu = new ContextMenuStrip();

        var statusItem = new ToolStripMenuItem($"Engine: {GetEngineStatusText()}")
        {
            Enabled = false
        };

        var settingsItem = new Forms.ToolStripMenuItem("Settings");
        settingsItem.Click += (_, _) => ShowSettings();

        var aboutItem = new Forms.ToolStripMenuItem("About");
        aboutItem.Click += (_, _) => ShowAbout();

        var exitItem = new Forms.ToolStripMenuItem("Exit");
        exitItem.Click += (_, _) => ExitApplication();

        menu.Opening += (_, _) =>
        {
            statusItem.Text = $"Engine: {GetEngineStatusText()}"; //TODO NO hardcode
        };

        menu.Items.Add(statusItem);
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add(settingsItem);
        menu.Items.Add(aboutItem);
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add(exitItem);

        return menu;
    }

    private void ShowSettings()
    {
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.ShowSettingsPage();
        });
    }

    private void ShowAbout()
    {
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.ShowAboutPage();
        });
    }

    private void ExitApplication()
    {
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.PrepareForExit();
            mainWindow.Close();

            WpfApplication.Current.Shutdown();
        });
    }

    private static string GetEngineStatusText()
    {
        return "Disabled"; //TODO No hardcode
    }

    private static Icon? LoadTrayIcon()
    {
        try
        {
            Uri resourceUri = new("/Assets/logo.ico", UriKind.Relative); //TODO Can this be extracted from csproj using Metadata helpers from core? Cleaner and consistent.
            StreamResourceInfo? resourceInfo = WpfApplication.GetResourceStream(resourceUri);

            if (resourceInfo?.Stream is null)
                return null;

            using var memoryStream = new MemoryStream();
            resourceInfo.Stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return new Icon(memoryStream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Disposes tray resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();

        _disposed = true;
    }
}