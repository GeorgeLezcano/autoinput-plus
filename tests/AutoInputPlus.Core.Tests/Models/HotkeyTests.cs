using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Tests.Models;

/// <summary>
/// Tests for Hotkey class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class HotKeyTests
{
    /// <summary>
    /// HotkeyValuesGetCorrectlyInitialized.
    /// </summary>
    [Fact]
    public void HotkeyValuesGetCorrectlyInitialized()
    {
        var expectedKey = InputKey.A;
        var expectedMod = HotkeyModifiers.Shift;
        var hotkey = new Hotkey(InputKey.A, HotkeyModifiers.Shift);

        Assert.Equal(expectedKey, hotkey.Key);
        Assert.Equal(expectedMod, hotkey.Modifiers);

    }

    /// <summary>
    /// HotkeyToStringDisplaysTheKey
    /// </summary>
    [Fact]
    public void HotkeyToStringDisplaysTheKey()
    {
        string expected = "A";

        var hotkey = new Hotkey(InputKey.A);

        Assert.Equal(expected, hotkey.ToString());

    }

    /// <summary>
    /// HotkeyToStringDisplaysTheKeyPlusModifiers
    /// </summary>
    [Fact]
    public void HotkeyToStringDisplaysTheKeyPlusModifiers()
    {
        string expected = "Alt+A";

        var hotkey = new Hotkey(InputKey.A, HotkeyModifiers.Alt);

        Assert.Equal(expected, hotkey.ToString());

    }
    
}