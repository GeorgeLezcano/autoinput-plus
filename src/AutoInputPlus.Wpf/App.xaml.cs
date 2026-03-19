using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AutoInputPlus.Engine;
using AutoInputPlus.Infrastructure;
using AutoInputPlus.Input.Windows;
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

    /// <summary>
    /// Handles application startup.
    /// </summary>
    /// <param name="e">
    /// Startup event data.
    /// </param>
    protected override void OnStartup(StartupEventArgs e)
    {
        _singleInstanceMutex = new Mutex(
            initiallyOwned: true,
            name: SingleInstanceMutexName,
            createdNew: out bool createdNew);

        _ownsMutex = createdNew;

        if (!createdNew)
        {
            Shutdown();
            return;
        }

        base.OnStartup(e);

        var services = new ServiceCollection();

        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();

        _trayIconManager = _serviceProvider.GetRequiredService<TrayIconManager>();
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
}