using System.Runtime.Versioning;
using AutoInputPlus.Core.Interfaces;
using Microsoft.Win32;

namespace AutoInputPlus.Infrastructure.Services;

/// <summary>
/// Manages Windows startup registration for the application.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class StartupRegistrationService : IStartupRegistrationService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "AutoInputPlus";

    private readonly string _executablePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupRegistrationService"/> class.
    /// </summary>
    /// <param name="executablePath">
    /// The full executable path to register for startup.
    /// </param>
    public StartupRegistrationService(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        _executablePath = executablePath;
    }

    /// <inheritdoc/>
    public bool IsEnabled()
    {
        using RegistryKey? runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);

        string? existingValue = runKey?.GetValue(AppName) as string;

        if (string.IsNullOrWhiteSpace(existingValue))
        {
            return false;
        }

        return string.Equals(existingValue, BuildRegistryValue(), StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public void SetEnabled(bool enabled)
    {
        using RegistryKey runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? throw new InvalidOperationException("Unable to open the Windows startup registry key.");

        if (enabled)
        {
            runKey.SetValue(AppName, BuildRegistryValue());
            return;
        }

        if (runKey.GetValue(AppName) is not null)
        {
            runKey.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    private string BuildRegistryValue()
    {
        return $"\"{_executablePath}\"";
    }
}