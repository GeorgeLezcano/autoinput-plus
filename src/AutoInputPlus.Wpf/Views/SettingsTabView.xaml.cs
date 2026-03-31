using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using InputBinding = AutoInputPlus.Core.Models.InputBinding;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using WpfKey = System.Windows.Input.Key;
using WpfMouseButton = System.Windows.Input.MouseButton;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for SettingsTabView.
/// </summary>
public partial class SettingsTabView : UserControl, IDisposable
{
    private static readonly Regex PositiveIntegerRegex = new("^[0-9]+$");

    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

    private IProfileManager? _profileManager;
    private IInputProfileStore? _inputProfileStore;
    private bool _isLoadingProfile;
    private bool _disposed;
    private bool _isCapturingHotkey;
    private bool _isCapturingTargetBinding;

    /// <summary>
    /// Occurs when the active profile has been updated and persisted.
    /// </summary>
    public event EventHandler? ActiveProfileUpdated;

    /// <summary>
    /// Gets a value indicating whether the view is currently capturing a hotkey or target input.
    /// </summary>
    public bool IsCapturingInput => _isCapturingHotkey || _isCapturingTargetBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsTabView"/> class.
    /// </summary>
    public SettingsTabView()
    {
        InitializeComponent();

        PreviewKeyDown += SettingsTabView_PreviewKeyDown;
        PreviewMouseDown += SettingsTabView_PreviewMouseDown;
        IsKeyboardFocusWithinChanged += SettingsTabView_IsKeyboardFocusWithinChanged;

        IntervalTextBox.PreviewTextInput += IntegerTextBox_PreviewTextInput;
        RunCountTextBox.PreviewTextInput += IntegerTextBox_PreviewTextInput;

        DataObject.AddPastingHandler(IntervalTextBox, IntegerTextBox_Pasting);
        DataObject.AddPastingHandler(RunCountTextBox, IntegerTextBox_Pasting);

        DisableScheduleUi();
        UpdateInputBehaviorUi();
        UpdateRunBehaviorUi();
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
            CancelCaptureMode();

            CaptureHotkeyButton.Content = profile.StartStopHotkey?.ToString() ?? "Set Hotkey";
            CaptureTargetKeyButton.Content = profile.TargetInputBinding?.ToDisplayName() ?? "Set Target";
            IntervalTextBox.Text = profile.IntervalMilliseconds.ToString(CultureInfo.CurrentCulture);
            RunCountTextBox.Text = Math.Max(1, profile.StopInputCount).ToString(CultureInfo.CurrentCulture);

            HoldCheckBox.IsChecked = profile.HoldTargetEnabled;

            if (profile.SequenceModeActive)
            {
                SequenceModeRadioButton.IsChecked = true;
                SingleInputModeRadioButton.IsChecked = false;
            }
            else
            {
                SingleInputModeRadioButton.IsChecked = true;
                SequenceModeRadioButton.IsChecked = false;
            }

            if (profile.RunUntilSetCountActive)
            {
                RunForSetCountRadioButton.IsChecked = true;
                RunUntilStopRadioButton.IsChecked = false;
            }
            else
            {
                RunUntilStopRadioButton.IsChecked = true;
                RunForSetCountRadioButton.IsChecked = false;
            }

            UpdateInputBehaviorUi();
            UpdateRunBehaviorUi();
            DisableScheduleUi();
        }
        finally
        {
            _isLoadingProfile = false;
        }
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
    }

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
        CancelCaptureMode();

        _isCapturingHotkey = true;
        _isCapturingTargetBinding = false;
        CaptureHotkeyButton.Content = "Press key...";
        Focus();
        _ = Keyboard.Focus(this);
    }

    private void CaptureTargetKeyButton_Click(object sender, RoutedEventArgs e)
    {
        CancelCaptureMode();

        _isCapturingTargetBinding = true;
        _isCapturingHotkey = false;
        CaptureTargetKeyButton.Content = "Press key...";
        Focus();
        _ = Keyboard.Focus(this);
    }

    private async void SettingsTabView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_isCapturingHotkey)
        {
            e.Handled = true;

            if (!TryMapToInputKey(e.Key, out InputKey key))
            {
                return;
            }

            HotkeyModifiers modifiers = GetCurrentModifiers();
            Hotkey hotkey = new(key, modifiers);

            if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
            {
                CancelCaptureMode();
                return;
            }

            profile.StartStopHotkey = hotkey;
            CaptureHotkeyButton.Content = hotkey.ToString();
            CompleteCaptureMode();

            await SaveProfileAndNotifyAsync(profile, profileStore);
            return;
        }

        if (_isCapturingTargetBinding)
        {
            e.Handled = true;

            if (!TryMapToInputKey(e.Key, out InputKey key))
            {
                return;
            }

            if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
            {
                CancelCaptureMode();
                return;
            }

            InputBinding binding = InputBinding.FromKey(key);
            profile.TargetInputBinding = binding;
            CaptureTargetKeyButton.Content = binding.ToDisplayName();
            CompleteCaptureMode();

            await SaveProfileAndNotifyAsync(profile, profileStore);
        }
    }

    private async void SettingsTabView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!_isCapturingTargetBinding)
        {
            if (IsCapturingInput && !IsClickOnCaptureButton(e.OriginalSource))
            {
                CancelCaptureMode();
            }

            return;
        }

        e.Handled = true;

        if (!TryMapToMouseButton(e.ChangedButton, out AutoInputPlus.Core.Enums.MouseButton mouseButton))
        {
            return;
        }

        if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            CancelCaptureMode();
            return;
        }

        InputBinding binding = InputBinding.FromMouseButton(mouseButton);
        profile.TargetInputBinding = binding;
        CaptureTargetKeyButton.Content = binding.ToDisplayName();
        CompleteCaptureMode();

        await SaveProfileAndNotifyAsync(profile, profileStore);
    }

    private static HotkeyModifiers GetCurrentModifiers()
    {
        HotkeyModifiers modifiers = HotkeyModifiers.None;
        ModifierKeys keyboardModifiers = Keyboard.Modifiers;

        if (keyboardModifiers.HasFlag(ModifierKeys.Control))
        {
            modifiers |= HotkeyModifiers.Control;
        }

        if (keyboardModifiers.HasFlag(ModifierKeys.Alt))
        {
            modifiers |= HotkeyModifiers.Alt;
        }

        if (keyboardModifiers.HasFlag(ModifierKeys.Shift))
        {
            modifiers |= HotkeyModifiers.Shift;
        }

        return modifiers;
    }

    private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !PositiveIntegerRegex.IsMatch(e.Text);
    }

    private void IntegerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        string pastedText = (string)e.DataObject.GetData(DataFormats.Text)!;
        if (!PositiveIntegerRegex.IsMatch(pastedText))
        {
            e.CancelCommand();
        }
    }

    private void CompleteCaptureMode()
    {
        _isCapturingHotkey = false;
        _isCapturingTargetBinding = false;
    }

    private void CancelCaptureMode()
    {
        CompleteCaptureMode();
        RestoreCaptureButtonContent();
    }

    private void RestoreCaptureButtonContent()
    {
        if (!TryGetProfileContext(out InputProfile profile, out _))
        {
            CaptureHotkeyButton.Content = "Set Hotkey";
            CaptureTargetKeyButton.Content = "Set Target";
            return;
        }

        CaptureHotkeyButton.Content = profile.StartStopHotkey?.ToString() ?? "Set Hotkey";
        CaptureTargetKeyButton.Content = profile.TargetInputBinding?.ToDisplayName() ?? "Set Target";
    }

    private void SettingsTabView_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isFocusedWithin && !isFocusedWithin && IsCapturingInput)
        {
            CancelCaptureMode();
        }
    }

    private void UpdateRunBehaviorUi()
    {
        if (RunUntilStopRadioButton is null || HoldCheckBox is null || RunCountTextBox is null)
        {
            return;
        }

        bool runUntilStop = RunUntilStopRadioButton.IsChecked == true;
        RunCountTextBox.IsEnabled = !runUntilStop && HoldCheckBox.IsChecked != true;
    }

    private void UpdateInputBehaviorUi()
    {
        if (HoldCheckBox is null || IntervalTextBox is null || RunUntilStopRadioButton is null || RunForSetCountRadioButton is null)
        {
            return;
        }

        bool holdEnabled = HoldCheckBox.IsChecked == true;
        IntervalTextBox.IsEnabled = !holdEnabled;
        RunUntilStopRadioButton.IsEnabled = true;
        RunForSetCountRadioButton.IsEnabled = !holdEnabled;
    }

    private void DisableScheduleUi()
    {
        if (ScheduleStartEnabledCheckBox is not null)
        {
            ScheduleStartEnabledCheckBox.IsEnabled = false;
            ScheduleStartEnabledCheckBox.IsChecked = false;
        }

        if (ScheduleStopEnabledCheckBox is not null)
        {
            ScheduleStopEnabledCheckBox.IsEnabled = false;
            ScheduleStopEnabledCheckBox.IsChecked = false;
        }

        if (ScheduleStartDatePicker is not null)
        {
            ScheduleStartDatePicker.IsEnabled = false;
        }

        if (ScheduleStartHourTextBox is not null)
        {
            ScheduleStartHourTextBox.IsEnabled = false;
        }

        if (ScheduleStartMinuteTextBox is not null)
        {
            ScheduleStartMinuteTextBox.IsEnabled = false;
        }

        if (ScheduleStartSecondTextBox is not null)
        {
            ScheduleStartSecondTextBox.IsEnabled = false;
        }

        if (ScheduleStopDatePicker is not null)
        {
            ScheduleStopDatePicker.IsEnabled = false;
        }

        if (ScheduleStopHourTextBox is not null)
        {
            ScheduleStopHourTextBox.IsEnabled = false;
        }

        if (ScheduleStopMinuteTextBox is not null)
        {
            ScheduleStopMinuteTextBox.IsEnabled = false;
        }

        if (ScheduleStopSecondTextBox is not null)
        {
            ScheduleStopSecondTextBox.IsEnabled = false;
        }
    }

    private void ScheduleStartEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (ScheduleStartEnabledCheckBox is not null)
        {
            ScheduleStartEnabledCheckBox.IsChecked = false;
        }
    }

    private void ScheduleStartEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
    }

    private void ScheduleStopEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (ScheduleStopEnabledCheckBox is not null)
        {
            ScheduleStopEnabledCheckBox.IsChecked = false;
        }
    }

    private void ScheduleStopEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
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
        await _saveSemaphore.WaitAsync();

        try
        {
            await profileStore.SaveProfileAsync(profile);
        }
        finally
        {
            _saveSemaphore.Release();
        }

        ActiveProfileUpdated?.Invoke(this, EventArgs.Empty);
    }

    private static bool TryParsePositiveInteger(string? text, out int value)
    {
        bool parsed = int.TryParse(text, NumberStyles.None, CultureInfo.CurrentCulture, out value);
        return parsed && value > 0;
    }

    #region Mapping

    private static bool TryMapToMouseButton(WpfMouseButton mouseButton, out AutoInputPlus.Core.Enums.MouseButton mappedButton)
        => InputCaptureMapper.TryMapToMouseButton(mouseButton, out mappedButton);

    private static bool TryMapToInputKey(WpfKey key, out InputKey inputKey)
        => InputCaptureMapper.TryMapToInputKey(key, out inputKey);

    #endregion

    private static bool IsClickOnCaptureButton(object originalSource, Button captureHotkeyButton, Button captureTargetKeyButton)
    {
        if (originalSource is not DependencyObject dependencyObject)
        {
            return false;
        }

        Button? clickedButton = FindVisualParent<Button>(dependencyObject);
        return clickedButton == captureHotkeyButton || clickedButton == captureTargetKeyButton;
    }

    private bool IsClickOnCaptureButton(object originalSource)
        => IsClickOnCaptureButton(originalSource, CaptureHotkeyButton, CaptureTargetKeyButton);

    private static T? FindVisualParent<T>(DependencyObject child)
        where T : DependencyObject
    {
        DependencyObject? current = child;

        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _saveSemaphore.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}