using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Input.Windows.Mapping;

namespace AutoInputPlus.Input.Windows.Tests.Mapping;

/// <summary>
/// Tests for <see cref="KeyCodeMapper"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class KeyCodeMapperTests
{
    /// <summary>
    /// Try Map To Virtual Key When Letter Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.A, 0x41)]
    [InlineData(InputKey.Z, 0x5A)]
    [InlineData(InputKey.M, 0x4D)]
    public void TryMapToVirtualKeyWhenLetterReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Digit Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.D0, 0x30)]
    [InlineData(InputKey.D5, 0x35)]
    [InlineData(InputKey.D9, 0x39)]
    public void TryMapToVirtualKeyWhenDigitReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Special Key Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.Enter, 0x0D)]
    [InlineData(InputKey.Tab, 0x09)]
    [InlineData(InputKey.Escape, 0x1B)]
    [InlineData(InputKey.Space, 0x20)]
    [InlineData(InputKey.Backspace, 0x08)]
    [InlineData(InputKey.Delete, 0x2E)]
    [InlineData(InputKey.Insert, 0x2D)]
    [InlineData(InputKey.Home, 0x24)]
    [InlineData(InputKey.End, 0x23)]
    [InlineData(InputKey.PageUp, 0x21)]
    [InlineData(InputKey.PageDown, 0x22)]
    [InlineData(InputKey.Left, 0x25)]
    [InlineData(InputKey.Up, 0x26)]
    [InlineData(InputKey.Right, 0x27)]
    [InlineData(InputKey.Down, 0x28)]
    [InlineData(InputKey.PrintScreen, 0x2C)]
    [InlineData(InputKey.Pause, 0x13)]
    [InlineData(InputKey.CapsLock, 0x14)]
    public void TryMapToVirtualKeyWhenSpecialKeyReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Function Key Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.F1, 0x70)]
    [InlineData(InputKey.F5, 0x74)]
    [InlineData(InputKey.F12, 0x7B)]
    public void TryMapToVirtualKeyWhenFunctionKeyReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Modifier Or Windows Key Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.LeftCtrl, 0xA2)]
    [InlineData(InputKey.RightCtrl, 0xA3)]
    [InlineData(InputKey.LeftShift, 0xA0)]
    [InlineData(InputKey.RightShift, 0xA1)]
    [InlineData(InputKey.LeftAlt, 0xA4)]
    [InlineData(InputKey.RightAlt, 0xA5)]
    [InlineData(InputKey.LeftWin, 0x5B)]
    [InlineData(InputKey.RightWin, 0x5C)]
    public void TryMapToVirtualKeyWhenModifierOrWindowsKeyReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Numpad Key Returns Expected Code.
    /// </summary>
    [Theory]
    [InlineData(InputKey.NumPad0, 0x60)]
    [InlineData(InputKey.NumPad5, 0x65)]
    [InlineData(InputKey.NumPad9, 0x69)]
    [InlineData(InputKey.Add, 0x6B)]
    [InlineData(InputKey.Subtract, 0x6D)]
    [InlineData(InputKey.Multiply, 0x6A)]
    [InlineData(InputKey.Divide, 0x6F)]
    [InlineData(InputKey.Decimal, 0x6E)]
    [InlineData(InputKey.Separator, 0x6C)]
    public void TryMapToVirtualKeyWhenNumpadKeyReturnsExpectedCode(InputKey key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Unsupported Enum Value Returns False.
    /// </summary>
    [Fact]
    public void TryMapToVirtualKeyWhenUnsupportedEnumValueReturnsFalse()
    {
        InputKey unsupportedKey = (InputKey)9999;

        bool result = KeyCodeMapper.TryMapToVirtualKey(unsupportedKey, out ushort actual);

        Assert.False(result);
        Assert.Equal((ushort)0, actual);
    }
}