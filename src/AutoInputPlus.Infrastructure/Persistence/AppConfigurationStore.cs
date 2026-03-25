using System.Text.Json;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Infrastructure.Persistence;

/// <summary>
/// AutoInput implementation for global app configuration.
/// </summary>
public sealed class AppConfigurationStore : IAppConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configurationFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigurationStore"/> class.
    /// </summary>
    /// <param name="baseDirectory">
    /// Optional base directory used for persistence.
    /// When not provided, the user's roaming application data folder is used.
    /// </param>
    public AppConfigurationStore(string? baseDirectory = null)
    {
        string appDirectory = ResolveAppDirectory(baseDirectory);
        Directory.CreateDirectory(appDirectory);

        _configurationFilePath = Path.Combine(appDirectory, "config.json"); //TODO Move to constants
    }

    /// <inheritdoc/>
    public async Task<AppConfiguration> LoadConfigurationAsync()
    {
        if (!File.Exists(_configurationFilePath))
        {
            return new AppConfiguration();
        }

        string json = await File.ReadAllTextAsync(_configurationFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new AppConfiguration();
        }

        AppConfiguration? configuration = JsonSerializer.Deserialize<AppConfiguration>(json, SerializerOptions);
        return configuration ?? new AppConfiguration();
    }

    /// <inheritdoc/>
    public async Task SaveConfigurationAsync(AppConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string? directoryPath = Path.GetDirectoryName(_configurationFilePath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string json = JsonSerializer.Serialize(configuration, SerializerOptions);
        await File.WriteAllTextAsync(_configurationFilePath, json);
    }

    private static string ResolveAppDirectory(string? baseDirectory)
    {
        if (!string.IsNullOrWhiteSpace(baseDirectory))
        {
            return baseDirectory;
        }

        string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataDirectory, "AutoInputPlus"); //TODO Move to constants
    }
}