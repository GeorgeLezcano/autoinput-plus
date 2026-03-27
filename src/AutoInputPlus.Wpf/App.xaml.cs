using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Engine;
using AutoInputPlus.Engine.Profile;
using AutoInputPlus.Infrastructure;
using AutoInputPlus.Input.Windows;
using AutoInputPlus.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Application entry point and dependency injection composition root.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "WPF Application lifetime is managed by OnExit, where disposable fields are released.")]
public partial class App : Application
{
    private const string SingleInstanceMutexName = @"Global\AutoInputPlus_SingleInstance";

    private Mutex? _singleInstanceMutex;
    private bool _ownsMutex;
    private ServiceProvider? _serviceProvider;
    private TrayIconManager? _trayIconManager;
    private IEngine? _engine;

    /// <summary>
    /// Constructor
    /// </summary>
    public App()
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    /// <summary>
    /// Handles application startup.
    /// </summary>
    /// <param name="e">
    /// Startup event data.
    /// </param>
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            _singleInstanceMutex = new Mutex(
                initiallyOwned: true,
                name: SingleInstanceMutexName,
                createdNew: out bool createdNew);

            _ownsMutex = createdNew;

            if (!createdNew)
            {
                System.Windows.MessageBox.Show("AutoInput Plus is already running.");
                Shutdown();
                return;
            }

            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            _trayIconManager = _serviceProvider.GetRequiredService<TrayIconManager>();
            _engine = _serviceProvider.GetRequiredService<IEngine>();

            IAppConfigurationStore appConfigurationStore =
                _serviceProvider.GetRequiredService<IAppConfigurationStore>();
            IInputProfileStore inputProfileStore =
                _serviceProvider.GetRequiredService<IInputProfileStore>();
            IProfileManager profileManager =
                _serviceProvider.GetRequiredService<IProfileManager>();

            AppConfiguration configuration = await appConfigurationStore.LoadConfigurationAsync();
            IReadOnlyList<InputProfile> profiles = await inputProfileStore.GetAllAsync();

            InputProfile activeProfile;

            if (profiles.Count > 0)
            {
                activeProfile = profiles.FirstOrDefault(p => p.ProfileId == configuration.LastActiveProfileId)
                    ?? profiles[0];
            }
            else
            {
                activeProfile = ProfileManager.CreateDefaultProfile();
            }

            profileManager.SetActiveProfile(activeProfile);

            try
            {
                ThemeManager.ApplyTheme(configuration.AppTheme);
            }
            catch (ArgumentOutOfRangeException)
            {
                ThemeManager.ApplyTheme(AppTheme.LightBlue);
            }

            if (configuration.EngineEnabled)
            {
                await _engine.EnableAsync();
            }
            else
            {
                await _engine.DisableAsync();
            }

            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.ToString(),
                "Startup Crash",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    /// <summary>
    /// Configures application services.
    /// </summary>
    /// <param name="services">
    /// The service collection to configure.
    /// </param>
    private static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddEngineServices()
            .AddInfrastructureServices()
            .AddWindowsInputServices();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<TrayIconManager>();
    }

    /// <summary>
    /// Handles application shutdown and disposes the service provider.
    /// </summary>
    /// <param name="e">
    /// Exit event data.
    /// </param>
    protected override void OnExit(ExitEventArgs e)
    {
        _trayIconManager?.Dispose();
        _serviceProvider?.Dispose();

        if (_ownsMutex)
        {
            _singleInstanceMutex?.ReleaseMutex();
        }

        _singleInstanceMutex?.Dispose();

        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(
        object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(
            e.Exception.ToString(),
            "UI Thread Crash",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
        Shutdown();
    }

    private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.ToString(),
                "Unhandled Exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(
            e.Exception.ToString(),
            "Task Exception",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.SetObserved();
    }
}