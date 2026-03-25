using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
        //TODO Probably ignore GUID ProfileId from InputProfile since its local.
    };

    /// <inheritdoc/>
    public async Task<string> ExportProfileAsync(InputProfile profile)
    {
        string jsonString = JsonSerializer.Serialize(profile, serializerOptions);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
    }

    /// <inheritdoc/>
    public async Task<InputProfile> ImportProfileAsync(string encodedProfile)
    {
        if (!IsValidProfileString(encodedProfile))
        {
            throw new InvalidEnumArgumentException("Invalid profile string.");
        }

        byte[] jsonBytes = Convert.FromBase64String(encodedProfile);
        string jsonString = Encoding.UTF8.GetString(jsonBytes);

        // TODO Do I save it here? maybe...since there is no "save" button in the app, its autosaves.

        return JsonSerializer.Deserialize<InputProfile>(jsonString, serializerOptions)!; //TODO Does this populate ProfileId?
    }

    /// <inheritdoc/>
    public bool IsValidProfileString(string encodedProfile)
    {
        if (string.IsNullOrWhiteSpace(encodedProfile)) return false;

        //TODO More validation? Maybe max/min length, or control characters?

        return true;
    }
}