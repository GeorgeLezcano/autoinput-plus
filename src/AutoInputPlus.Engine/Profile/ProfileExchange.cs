using System.Text;
using System.Text.Json;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Profile;

/// <summary>
/// AutoInput implementation profile exchanges.
/// Handles imports, exports as well as generated
/// string validation.
/// </summary>
public sealed class ProfileExchange : IProfileExchange
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false
        //TODO Potentially a way to exclude ProfileId property from here? explore other options
    };

    /// <inheritdoc/>
    public Task<string> ExportProfileAsync(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        string jsonString = JsonSerializer.Serialize(profile, SerializerOptions);
        string encodedProfile = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

        return Task.FromResult(encodedProfile);
    }

    /// <inheritdoc/>
    public Task<InputProfile> ImportProfileAsync(string encodedProfile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encodedProfile);

        if (!TryDecodeAndDeserializeProfile(encodedProfile, out InputProfile? profile))
        {
            throw new InvalidOperationException("The provided profile string is invalid.");
        }

        return Task.FromResult(profile)!;
    }

    /// <inheritdoc/>
    public bool IsValidProfileString(string encodedProfile)
    {
        if (string.IsNullOrWhiteSpace(encodedProfile))
        {
            return false;
        }

        return TryDecodeAndDeserializeProfile(encodedProfile, out _);
    }

    private static bool TryDecodeAndDeserializeProfile(
        string encodedProfile,
        out InputProfile? profile)
    {
        profile = null;

        try
        {
            byte[] jsonBytes = Convert.FromBase64String(encodedProfile);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            profile = JsonSerializer.Deserialize<InputProfile>(jsonString, SerializerOptions);

            if (profile is null)
            {
                return false;
            }

            profile.ProfileId = Guid.NewGuid();
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}