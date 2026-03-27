using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Extensions;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using UserControl = System.Windows.Controls.UserControl;
using WpfMouseButton = System.Windows.Input.MouseButton;
using WpfKey = System.Windows.Input.Key;
using DataFormats = System.Windows.DataFormats;
using InputBinding = AutoInputPlus.Core.Models.InputBinding;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using DataObject = System.Windows.DataObject;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for SettingsTabView.
/// </summary>
public partial class SettingsTabView : UserControl
{
    private static readonly Regex DigitsOnlyRegex = new("^[0-9]+$");

    private IProfileManager? _profileManager;
    private IInputProfileStore? _inputProfileStore;

    private bool _isLoadingProfile;
    private bool _isCapturingHotkey;
    private bool _isCapturingTargetBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsTabView"/> class.
    /// </summary>
    public SettingsTabView()
    {
        InitializeComponent();

        PreviewKeyDown += SettingsTabView_PreviewKeyDown;
        PreviewMouseDown += SettingsTabView_PreviewMouseDown;

        IntervalTextBox.PreviewTextInput += IntegerTextBox_PreviewTextInput;
        RunCountTextBox.PreviewTextInput += IntegerTextBox_PreviewTextInput;

        DataObject.AddPastingHandler(IntervalTextBox, IntegerTextBox_Pasting);
        DataObject.AddPastingHandler(RunCountTextBox, IntegerTextBox_Pasting);
    }

    /// <summary>
    /// Assigns the services needed by the settings view.
    /// </summary>
    /// <param name="profileManager">The profile manager.</param>
    /// <param name="inputProfileStore">The profile store.</param>
    public void SetContext(IProfileManager profileManager, IInputProfileStore inputProfileStore)
    {
        ArgumentNullException.ThrowIfNull(profileManager);
        ArgumentNullException.ThrowIfNull(inputProfileStore);

        _profileManager = profileManager;
        _inputProfileStore = inputProfileStore;
    }

    /// <summary>
    /// Loads the current profile values into the settings controls.
    /// </summary>
    /// <param name="profile">The active profile.</param>
    public void LoadProfile(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        _isLoadingProfile = true;

        try
        {
            SingleInputModeRadioButton.IsChecked = !profile.SequenceModeActive;
            SequenceModeRadioButton.IsChecked = profile.SequenceModeActive;

            CaptureHotkeyButton.Content = profile.StartStopHotkey.ToString();
            CaptureTargetKeyButton.Content = profile.TargetInputBinding.ToDisplayName();

            IntervalTextBox.Text = profile.IntervalMilliseconds.ToString(CultureInfo.CurrentCulture);
            HoldCheckBox.IsChecked = profile.HoldTargetEnabled;

            RunUntilStopRadioButton.IsChecked = profile.RunUntilStopActive;
            RunForSetCountRadioButton.IsChecked = profile.RunUntilSetCountActive;
            RunCountTextBox.Text = profile.StopInputCount.ToString(CultureInfo.CurrentCulture);

            ScheduleStartEnabledCheckBox.IsChecked = profile.ScheduleStartEnabled;
            ScheduleStopEnabledCheckBox.IsChecked = profile.ScheduleStopEnabled;
        }
        finally
        {
            _isLoadingProfile = false;
        }

        UpdateInputBehaviorUi();
        UpdateRunBehaviorUi();
        StopCaptureMode();
    }

    #region Hanlders

    private async void SingleInputModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.SequenceModeActive = false;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void SequenceModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.SequenceModeActive = true;
        await profileStore.SaveProfileAsync(profile);
    }

    private void CaptureHotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        _isCapturingHotkey = true;
        _isCapturingTargetBinding = false;
        CaptureHotkeyButton.Content = "Press key...";
        Focus();
        Keyboard.Focus(this);
    }

    private void CaptureTargetKeyButton_Click(object sender, RoutedEventArgs e)
    {
        _isCapturingTargetBinding = true;
        _isCapturingHotkey = false;
        CaptureTargetKeyButton.Content = "Press key...";
        Focus();
        Keyboard.Focus(this);
    }

    private async void IntervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (!TryParsePositiveInteger(IntervalTextBox.Text, out int intervalMilliseconds))
        {
            return;
        }

        profile.IntervalMilliseconds = intervalMilliseconds;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void RunUntilStopRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        UpdateRunBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.RunUntilStopActive = true;
        profile.RunUntilSetCountActive = false;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void RunForSetCountRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        UpdateRunBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.RunUntilStopActive = false;
        profile.RunUntilSetCountActive = true;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void RunCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (!TryParsePositiveInteger(RunCountTextBox.Text, out int stopInputCount))
        {
            return;
        }

        profile.StopInputCount = stopInputCount;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void HoldCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        UpdateInputBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.HoldTargetEnabled = true;
        await profileStore.SaveProfileAsync(profile);
    }

    private async void HoldCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        UpdateInputBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.HoldTargetEnabled = false;
        await profileStore.SaveProfileAsync(profile);
    }

    private void ScheduleStartEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        // TODO Future schedule implementation.
    }

    private void ScheduleStartEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        // TODO Future schedule implementation.
    }

    private void ScheduleStopEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        // TODO Future schedule implementation.
    }

    private void ScheduleStopEnabledCheckBox_Uchecked(object sender, RoutedEventArgs e)
    {
        // TODO Future schedule implementation.
    }

    #endregion

    #region Capture

    private async void SettingsTabView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_isCapturingHotkey)
        {
            e.Handled = true;

            if (!TryMapToInputKey(e.Key, out InputKey key))
            {
                CaptureHotkeyButton.Content = "Unsupported";
                return;
            }

            HotkeyModifiers modifiers = GetCurrentModifiers();
            Hotkey hotkey = new(key, modifiers);

            if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
            {
                StopCaptureMode();
                return;
            }

            profile.StartStopHotkey = hotkey;
            CaptureHotkeyButton.Content = hotkey.ToString();
            StopCaptureMode();

            await profileStore.SaveProfileAsync(profile);
            return;
        }

        if (_isCapturingTargetBinding)
        {
            e.Handled = true;

            if (!TryMapToInputKey(e.Key, out InputKey key))
            {
                CaptureTargetKeyButton.Content = "Unsupported";
                return;
            }

            if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
            {
                StopCaptureMode();
                return;
            }

            InputBinding binding = InputBinding.FromKey(key);
            profile.TargetInputBinding = binding;
            CaptureTargetKeyButton.Content = binding.ToDisplayName();
            StopCaptureMode();

            await profileStore.SaveProfileAsync(profile);
        }
    }

    private async void SettingsTabView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!_isCapturingTargetBinding)
        {
            return;
        }

        e.Handled = true;

        if (!TryMapToMouseButton(e.ChangedButton, out AutoInputPlus.Core.Enums.MouseButton mouseButton))
        {
            CaptureTargetKeyButton.Content = "Unsupported";
            return;
        }

        if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            StopCaptureMode();
            return;
        }

        InputBinding binding = InputBinding.FromMouseButton(mouseButton);
        profile.TargetInputBinding = binding;
        CaptureTargetKeyButton.Content = binding.ToDisplayName();
        StopCaptureMode();

        await profileStore.SaveProfileAsync(profile);
    }

    private void StopCaptureMode()
    {
        _isCapturingHotkey = false;
        _isCapturingTargetBinding = false;
    }

    private HotkeyModifiers GetCurrentModifiers()
    {
        HotkeyModifiers modifiers = HotkeyModifiers.None;

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            modifiers |= HotkeyModifiers.Control;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            modifiers |= HotkeyModifiers.Shift;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
        {
            modifiers |= HotkeyModifiers.Alt;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
        {
            modifiers |= HotkeyModifiers.Windows;
        }

        return modifiers;
    }

    #endregion

    #region Validation / UI

    private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !DigitsOnlyRegex.IsMatch(e.Text);
    }

    private void IntegerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        string pastedText = (string)e.DataObject.GetData(DataFormats.Text)!;

        if (!DigitsOnlyRegex.IsMatch(pastedText))
        {
            e.CancelCommand();
        }
    }

    private void UpdateRunBehaviorUi()
    {
        bool runForSetCount = RunForSetCountRadioButton.IsChecked == true;
        RunCountTextBox.IsEnabled = runForSetCount;
    }

    private void UpdateInputBehaviorUi()
    {
        bool holdEnabled = HoldCheckBox.IsChecked == true;
        IntervalTextBox.IsEnabled = !holdEnabled;
    }

    private static bool TryParsePositiveInteger(string? text, out int value)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.CurrentCulture, out value))
        {
            return false;
        }

        return value > 0;
    }

    private bool TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore)
    {
        profile = null!;
        profileStore = null!;

        if (_profileManager is null || _inputProfileStore is null)
        {
            return false;
        }

        profile = _profileManager.ActiveProfile;
        profileStore = _inputProfileStore;
        return true;
    }

    #endregion

    #region Mapping

    private static bool TryMapToMouseButton(WpfMouseButton mouseButton, out AutoInputPlus.Core.Enums.MouseButton mappedButton)
    {
        switch (mouseButton)
        {
            case WpfMouseButton.Left:
                mappedButton = AutoInputPlus.Core.Enums.MouseButton.Left;
                return true;

            case WpfMouseButton.Right:
                mappedButton = AutoInputPlus.Core.Enums.MouseButton.Right;
                return true;

            case WpfMouseButton.Middle:
                mappedButton = AutoInputPlus.Core.Enums.MouseButton.Middle;
                return true;

            default:
                mappedButton = default;
                return false;
        }
    }

    private static bool TryMapToInputKey(WpfKey key, out InputKey inputKey)
    {
        if (key == WpfKey.System)
        {
            key = Keyboard.FocusedElement is not null ? KeyInterop.KeyFromVirtualKey(KeyInterop.VirtualKeyFromKey(Key.System)) : key;
        }

        switch (key)
        {
            case WpfKey.A: inputKey = InputKey.A; return true;
            case WpfKey.B: inputKey = InputKey.B; return true;
            case WpfKey.C: inputKey = InputKey.C; return true;
            case WpfKey.D: inputKey = InputKey.D; return true;
            case WpfKey.E: inputKey = InputKey.E; return true;
            case WpfKey.F: inputKey = InputKey.F; return true;
            case WpfKey.G: inputKey = InputKey.G; return true;
            case WpfKey.H: inputKey = InputKey.H; return true;
            case WpfKey.I: inputKey = InputKey.I; return true;
            case WpfKey.J: inputKey = InputKey.J; return true;
            case WpfKey.K: inputKey = InputKey.K; return true;
            case WpfKey.L: inputKey = InputKey.L; return true;
            case WpfKey.M: inputKey = InputKey.M; return true;
            case WpfKey.N: inputKey = InputKey.N; return true;
            case WpfKey.O: inputKey = InputKey.O; return true;
            case WpfKey.P: inputKey = InputKey.P; return true;
            case WpfKey.Q: inputKey = InputKey.Q; return true;
            case WpfKey.R: inputKey = InputKey.R; return true;
            case WpfKey.S: inputKey = InputKey.S; return true;
            case WpfKey.T: inputKey = InputKey.T; return true;
            case WpfKey.U: inputKey = InputKey.U; return true;
            case WpfKey.V: inputKey = InputKey.V; return true;
            case WpfKey.W: inputKey = InputKey.W; return true;
            case WpfKey.X: inputKey = InputKey.X; return true;
            case WpfKey.Y: inputKey = InputKey.Y; return true;
            case WpfKey.Z: inputKey = InputKey.Z; return true;

            case WpfKey.D0: inputKey = InputKey.D0; return true;
            case WpfKey.D1: inputKey = InputKey.D1; return true;
            case WpfKey.D2: inputKey = InputKey.D2; return true;
            case WpfKey.D3: inputKey = InputKey.D3; return true;
            case WpfKey.D4: inputKey = InputKey.D4; return true;
            case WpfKey.D5: inputKey = InputKey.D5; return true;
            case WpfKey.D6: inputKey = InputKey.D6; return true;
            case WpfKey.D7: inputKey = InputKey.D7; return true;
            case WpfKey.D8: inputKey = InputKey.D8; return true;
            case WpfKey.D9: inputKey = InputKey.D9; return true;

            case WpfKey.NumPad0: inputKey = InputKey.NumPad0; return true;
            case WpfKey.NumPad1: inputKey = InputKey.NumPad1; return true;
            case WpfKey.NumPad2: inputKey = InputKey.NumPad2; return true;
            case WpfKey.NumPad3: inputKey = InputKey.NumPad3; return true;
            case WpfKey.NumPad4: inputKey = InputKey.NumPad4; return true;
            case WpfKey.NumPad5: inputKey = InputKey.NumPad5; return true;
            case WpfKey.NumPad6: inputKey = InputKey.NumPad6; return true;
            case WpfKey.NumPad7: inputKey = InputKey.NumPad7; return true;
            case WpfKey.NumPad8: inputKey = InputKey.NumPad8; return true;
            case WpfKey.NumPad9: inputKey = InputKey.NumPad9; return true;
            case WpfKey.Multiply: inputKey = InputKey.Multiply; return true;
            case WpfKey.Add: inputKey = InputKey.Add; return true;
            case WpfKey.Separator: inputKey = InputKey.Separator; return true;
            case WpfKey.Subtract: inputKey = InputKey.Subtract; return true;
            case WpfKey.Decimal: inputKey = InputKey.Decimal; return true;
            case WpfKey.Divide: inputKey = InputKey.Divide; return true;

            case WpfKey.F1: inputKey = InputKey.F1; return true;
            case WpfKey.F2: inputKey = InputKey.F2; return true;
            case WpfKey.F3: inputKey = InputKey.F3; return true;
            case WpfKey.F4: inputKey = InputKey.F4; return true;
            case WpfKey.F5: inputKey = InputKey.F5; return true;
            case WpfKey.F6: inputKey = InputKey.F6; return true;
            case WpfKey.F7: inputKey = InputKey.F7; return true;
            case WpfKey.F8: inputKey = InputKey.F8; return true;
            case WpfKey.F9: inputKey = InputKey.F9; return true;
            case WpfKey.F10: inputKey = InputKey.F10; return true;
            case WpfKey.F11: inputKey = InputKey.F11; return true;
            case WpfKey.F12: inputKey = InputKey.F12; return true;

            case WpfKey.Back: inputKey = InputKey.Backspace; return true;
            case WpfKey.Tab: inputKey = InputKey.Tab; return true;
            case WpfKey.Enter: inputKey = InputKey.Enter; return true;
            case WpfKey.Pause: inputKey = InputKey.Pause; return true;
            case WpfKey.CapsLock: inputKey = InputKey.CapsLock; return true;
            case WpfKey.Escape: inputKey = InputKey.Escape; return true;
            case WpfKey.Space: inputKey = InputKey.Space; return true;
            case WpfKey.PageUp: inputKey = InputKey.PageUp; return true;
            case WpfKey.PageDown: inputKey = InputKey.PageDown; return true;
            case WpfKey.End: inputKey = InputKey.End; return true;
            case WpfKey.Home: inputKey = InputKey.Home; return true;
            case WpfKey.Left: inputKey = InputKey.Left; return true;
            case WpfKey.Up: inputKey = InputKey.Up; return true;
            case WpfKey.Right: inputKey = InputKey.Right; return true;
            case WpfKey.Down: inputKey = InputKey.Down; return true;
            case WpfKey.Insert: inputKey = InputKey.Insert; return true;
            case WpfKey.Delete: inputKey = InputKey.Delete; return true;
            case WpfKey.PrintScreen: inputKey = InputKey.PrintScreen; return true;

            case WpfKey.LWin: inputKey = InputKey.LeftWin; return true;
            case WpfKey.RWin: inputKey = InputKey.RightWin; return true;

            case WpfKey.LeftShift: inputKey = InputKey.LeftShift; return true;
            case WpfKey.RightShift: inputKey = InputKey.RightShift; return true;
            case WpfKey.LeftCtrl: inputKey = InputKey.LeftCtrl; return true;
            case WpfKey.RightCtrl: inputKey = InputKey.RightCtrl; return true;
            case WpfKey.LeftAlt: inputKey = InputKey.LeftAlt; return true;
            case WpfKey.RightAlt: inputKey = InputKey.RightAlt; return true;

            default:
                inputKey = default;
                return false;
        }
    }

    #endregion
}