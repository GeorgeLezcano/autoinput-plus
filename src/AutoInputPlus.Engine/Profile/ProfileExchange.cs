using System.Text;
using System.Text.Json;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Core.Serialization;

namespace AutoInputPlus.Engine.Profile;

/// <summary>
/// AutoInput implementation profile exchanges.
/// Handles imports, exports as well as generated
/// string validation.
/// </summary>
public sealed class ProfileExchange : IProfileExchange
{
    //TODO Potentially move to constants file
    private const string ImportFailedMessage = "Import failed. The provided profile string is invalid.";
    private const string ExportFailedMessage = "Export failed. The profile could not be prepared for sharing.";

    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    /// <inheritdoc/>
    public Task<string> ExportProfileAsync(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        try
        {
            string jsonString = JsonSerializer.Serialize(profile, SerializerOptions);
            string encodedProfile = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            return Task.FromResult(encodedProfile);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException(ExportFailedMessage, ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(ExportFailedMessage, ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExportFailedMessage, ex);
        }
    }

    /// <inheritdoc/>
    public Task<InputProfile> ImportProfileAsync(string encodedProfile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encodedProfile);

        if (!TryDecodeAndDeserializeProfile(encodedProfile, out InputProfile? profile))
        {
            throw new InvalidOperationException(ImportFailedMessage);
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

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return false;
            }

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
        catch (Exception)
        {
            return false;
        }
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = false
        };

        options.Converters.Add(new InputBindingJsonConverter());
        return options;
    }
}