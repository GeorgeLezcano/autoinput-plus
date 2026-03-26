using System.Text.Json;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Core.Serialization;

namespace AutoInputPlus.Infrastructure.Persistence;

/// <summary>
/// AutoInput implementation for input profiles.
/// It handles persistence and retrieval.
/// </summary>
public sealed class InputProfileStore : IInputProfileStore
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private readonly string _profilesDirectoryPath;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="baseDirectory"></param>
    public InputProfileStore(string? baseDirectory = null)
    {
        string appDirectory = ResolveAppDirectory(baseDirectory);
        _profilesDirectoryPath = Path.Combine(appDirectory, "Profiles"); //TODO Move to constants

        Directory.CreateDirectory(_profilesDirectoryPath);
    }

    /// <inheritdoc/>
    public Task DeleteProfileAsync(Guid profileId)
    {
        string profileFilePath = GetProfileFilePath(profileId);

        if (File.Exists(profileFilePath))
        {
            File.Delete(profileFilePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(Guid profileId)
    {
        string profileFilePath = GetProfileFilePath(profileId);
        return Task.FromResult(File.Exists(profileFilePath));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<InputProfile>> GetAllAsync()
    {
        if (!Directory.Exists(_profilesDirectoryPath))
        {
            return [];
        }

        string[] files = Directory.GetFiles(_profilesDirectoryPath, "*.json"); //TODO Move filter to constants
        List<InputProfile> profiles = [];

        foreach (string file in files)
        {
            InputProfile? profile = await TryLoadProfileFromFileAsync(file);

            if (profile is not null)
            {
                profiles.Add(profile);
            }
        }

        return [.. profiles.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <inheritdoc/>
    public async Task<InputProfile> LoadProfileAsync(Guid profileId)
    {
        string filePath = GetProfileFilePath(profileId);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Profile '{profileId}' not found.", filePath);
        }

        InputProfile? profile = await TryLoadProfileFromFileAsync(filePath) 
            ?? throw new InvalidOperationException($"Profile '{profileId}' could not be loaded.");
            
        return profile;
    }

    /// <inheritdoc/>
    public async Task SaveProfileAsync(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (profile.ProfileId == Guid.Empty)
        {
            profile.ProfileId = Guid.NewGuid();
        }

        Directory.CreateDirectory(_profilesDirectoryPath);

        string json = JsonSerializer.Serialize(profile, SerializerOptions);
        string filePath = GetProfileFilePath(profile.ProfileId);

        await File.WriteAllTextAsync(filePath, json);
    }

    /// <inheritdoc/>
    private static async Task<InputProfile?> TryLoadProfileFromFileAsync(string filePath)
    {
        string json = await File.ReadAllTextAsync(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<InputProfile>(json, SerializerOptions);
    }

    private string GetProfileFilePath(Guid profileId)
        => Path.Combine(_profilesDirectoryPath, $"{profileId}.json");

    private static string ResolveAppDirectory(string? baseDirectory)
    {
        if (!string.IsNullOrWhiteSpace(baseDirectory))
        {
            return baseDirectory;
        }

        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "AutoInputPlus");
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        options.Converters.Add(new InputBindingJsonConverter());
        return options;
    }
}