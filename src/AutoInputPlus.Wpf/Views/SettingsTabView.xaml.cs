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
    /// Raised after the active profile is changed and saved from this view.
    /// </summary>
    public event EventHandler? ActiveProfileUpdated;

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

        DisableScheduleUi();
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

            CaptureHotkeyButton.Content = profile.StartStopHotkey?.ToString() ?? "Set Hotkey";
            CaptureTargetKeyButton.Content = profile.TargetInputBinding is null
                ? "Set Target"
                : profile.TargetInputBinding.ToDisplayName();

            IntervalTextBox.Text = profile.IntervalMilliseconds.ToString(CultureInfo.CurrentCulture);
            HoldCheckBox.IsChecked = profile.HoldTargetEnabled;

            if (profile.HoldTargetEnabled)
            {
                profile.RunUntilStopActive = true;
                profile.RunUntilSetCountActive = false;
            }

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
        DisableScheduleUi();
    }

    #region Handlers

    private async void SingleInputModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.SequenceModeActive = false;
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private async void SequenceModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.SequenceModeActive = true;
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private void CaptureHotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        _isCapturingHotkey = true;
        _isCapturingTargetBinding = false;
        CaptureHotkeyButton.Content = "Press key...";
        Focus();
        _ = Keyboard.Focus(this);
    }

    private void CaptureTargetKeyButton_Click(object sender, RoutedEventArgs e)
    {
        _isCapturingTargetBinding = true;
        _isCapturingHotkey = false;
        CaptureTargetKeyButton.Content = "Press key...";
        Focus();
        _ = Keyboard.Focus(this);
    }

    private async void IntervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (profile.HoldTargetEnabled)
        {
            return;
        }

        if (!TryParsePositiveInteger(IntervalTextBox.Text, out int intervalMilliseconds))
        {
            return;
        }

        profile.IntervalMilliseconds = intervalMilliseconds;
        await SaveProfileAndNotifyAsync(profile, profileStore);
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
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private async void RunForSetCountRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        UpdateRunBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (profile.HoldTargetEnabled)
        {
            profile.RunUntilStopActive = true;
            profile.RunUntilSetCountActive = false;

            _isLoadingProfile = true;
            try
            {
                RunUntilStopRadioButton.IsChecked = true;
                RunForSetCountRadioButton.IsChecked = false;
            }
            finally
            {
                _isLoadingProfile = false;
            }

            UpdateRunBehaviorUi();
            await SaveProfileAndNotifyAsync(profile, profileStore);
            return;
        }

        profile.RunUntilStopActive = false;
        profile.RunUntilSetCountActive = true;
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private async void RunCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (profile.HoldTargetEnabled || profile.RunUntilStopActive)
        {
            return;
        }

        if (!TryParsePositiveInteger(RunCountTextBox.Text, out int stopInputCount))
        {
            return;
        }

        profile.StopInputCount = stopInputCount;
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private async void HoldCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        UpdateInputBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.HoldTargetEnabled = true;
        profile.RunUntilStopActive = true;
        profile.RunUntilSetCountActive = false;

        _isLoadingProfile = true;
        try
        {
            RunUntilStopRadioButton.IsChecked = true;
            RunForSetCountRadioButton.IsChecked = false;
        }
        finally
        {
            _isLoadingProfile = false;
        }

        UpdateRunBehaviorUi();
        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private async void HoldCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        UpdateInputBehaviorUi();

        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        profile.HoldTargetEnabled = false;
        await SaveProfileAndNotifyAsync(profile, profileStore);

        UpdateRunBehaviorUi();
    }

    private void ScheduleStartEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        DisableScheduleUi();
    }

    private void ScheduleStartEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        DisableScheduleUi();
    }

    private void ScheduleStopEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        DisableScheduleUi();
    }

    private void ScheduleStopEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        DisableScheduleUi();
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

            await SaveProfileAndNotifyAsync(profile, profileStore);
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

            await SaveProfileAndNotifyAsync(profile, profileStore);
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

        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private void StopCaptureMode()
    {
        _isCapturingHotkey = false;
        _isCapturingTargetBinding = false;
    }

    private static HotkeyModifiers GetCurrentModifiers()
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
        if (RunForSetCountRadioButton is null || RunCountTextBox is null || RunUntilStopRadioButton is null)
        {
            return;
        }

        bool holdEnabled = HoldCheckBox.IsChecked == true;

        if (holdEnabled)
        {
            RunUntilStopRadioButton.IsEnabled = true;
            RunForSetCountRadioButton.IsEnabled = false;
            RunUntilStopRadioButton.IsChecked = true;
            RunForSetCountRadioButton.IsChecked = false;
            RunCountTextBox.IsEnabled = false;
            return;
        }

        RunUntilStopRadioButton.IsEnabled = true;
        RunForSetCountRadioButton.IsEnabled = true;

        bool runForSetCount = RunForSetCountRadioButton.IsChecked == true;
        RunCountTextBox.IsEnabled = runForSetCount;
    }

    private void UpdateInputBehaviorUi()
    {
        bool holdEnabled = HoldCheckBox.IsChecked == true;
        IntervalTextBox.IsEnabled = !holdEnabled;
    }

    private void DisableScheduleUi()
    {
        if (ScheduleStartEnabledCheckBox is null)
        {
            return;
        }

        ScheduleStartEnabledCheckBox.IsChecked = false;
        ScheduleStopEnabledCheckBox.IsChecked = false;

        ScheduleStartDatePicker.IsEnabled = false;
        ScheduleStartHourTextBox.IsEnabled = false;
        ScheduleStartMinuteTextBox.IsEnabled = false;
        ScheduleStartSecondTextBox.IsEnabled = false;

        ScheduleStopDatePicker.IsEnabled = false;
        ScheduleStopHourTextBox.IsEnabled = false;
        ScheduleStopMinuteTextBox.IsEnabled = false;
        ScheduleStopSecondTextBox.IsEnabled = false;
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

    private async Task SaveProfileAndNotifyAsync(InputProfile profile, IInputProfileStore profileStore)
    {
        await profileStore.SaveProfileAsync(profile);
        ActiveProfileUpdated?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Mapping

    private static bool TryMapToMouseButton(WpfMouseButton mouseButton, out AutoInputPlus.Core.Enums.MouseButton mappedButton)
        => InputCaptureMapper.TryMapToMouseButton(mouseButton, out mappedButton);

    private static bool TryMapToInputKey(WpfKey key, out InputKey inputKey)
        => InputCaptureMapper.TryMapToInputKey(key, out inputKey);

    #endregion
}