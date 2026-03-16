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
    /// <inheritdoc/>
    public Task<string> ExportProfileAsync(InputProfile profile)
    {
        //TODO Export logic, encode the data into a single string.

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<InputProfile> ImportProfileAsync(string encodedProfile)
    {
        // TODO Decode the provided string, validate it with IsValidProfileString then load it
        // or potentially save it too.

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool IsValidProfileString(string encodedProfile)
    {
        // TODO Validation logic, figure out encoding/decoding logic to use.
        // Research for existing libraries, or maybe make a custom encoder. 
        throw new NotImplementedException();
    }
}