using System.Windows;
using AutoInputPlus.Engine;
using AutoInputPlus.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AutoInputPlus.Wpf;

/// <summary>
/// Application entry point and dependency injection composition root.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Handles application startup.
    /// </summary>
    /// <param name="e">
    /// Startup event data.
    /// </param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
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
            .AddInfrastructureServices();

        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Handles application shutdown and disposes the service provider.
    /// </summary>
    /// <param name="e">
    /// Exit event data.
    /// </param>
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}