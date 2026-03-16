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
    public Task<string> ExportProfile(InputProfile profile)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<InputProfile> ImportProfile(string encodedProfile)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool IsValidProfileString(string encodedProfile)
    {
        throw new NotImplementedException();
    }
}