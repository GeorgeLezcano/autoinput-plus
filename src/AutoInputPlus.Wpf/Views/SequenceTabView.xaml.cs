using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoInputPlus.Core.Constants;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Wpf.Views.Dialogs;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using WpfMouseButton = System.Windows.Input.MouseButton;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for SequenceTabView.
/// </summary>
public partial class SequenceTabView : UserControl, IDisposable
{
    private static readonly Regex DigitsOnlyRegex = new("^[0-9]+$");
    private const int MaxSequenceNameLength = 100;

    private readonly ObservableCollection<SequenceStepRow> _stepRows = [];
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

    private IProfileManager? _profileManager;
    private IInputProfileStore? _inputProfileStore;
    private bool _isLoadingProfile;
    private bool _disposed;
    private SequenceStepRow? _capturingStepRow;

    /// <summary>
    /// Gets a value indicating whether the view is currently capturing a step target.
    /// </summary>
    public bool IsCapturingInput => _capturingStepRow is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceTabView"/> class.
    /// </summary>
    public SequenceTabView()
    {
        InitializeComponent();

        PreviewKeyDown += SequenceTabView_PreviewKeyDown;
        PreviewMouseDown += SequenceTabView_PreviewMouseDown;
        IsKeyboardFocusWithinChanged += SequenceTabView_IsKeyboardFocusWithinChanged;

        StepsDataGrid.ItemsSource = _stepRows;

        DataObject.AddPastingHandler(StepsDataGrid, NonNegativeIntegerTextBox_Pasting);
    }

    /// <summary>
    /// Assigns the services needed by the sequence view.
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
    /// Loads the current profile values into the sequence controls.
    /// </summary>
    /// <param name="profile">The active profile.</param>
    public void LoadProfile(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        _isLoadingProfile = true;

        try
        {
            CancelCaptureMode();

            SequenceListBox.ItemsSource = null;
            SequenceListBox.ItemsSource = profile.Sequences;
            SequenceListBox.DisplayMemberPath = nameof(Sequence.Name);

            if (profile.Sequences.Count == 0)
            {
                SequenceListBox.SelectedIndex = -1;
                StepsDataGrid.SelectedIndex = -1;
                RefreshStepRows(null);
                return;
            }

            int selectedIndex = Math.Clamp(profile.SelectedSequenceIndex, 0, profile.Sequences.Count - 1);
            profile.SelectedSequenceIndex = selectedIndex;
            SequenceListBox.SelectedIndex = selectedIndex;
            RefreshStepRows(profile.Sequences[selectedIndex]);
        }
        finally
        {
            _isLoadingProfile = false;
        }
    }

    #region Handlers

    private async void NewSequenceButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        Sequence newSequence = new()
        {
            Name = GenerateUniqueSequenceName(profile, AppConstants.DefaultSequenceName),
            Steps = []
        };

        profile.Sequences.Add(newSequence);
        profile.SelectedSequenceIndex = profile.Sequences.Count - 1;

        await SaveProfileAsync(profile, profileStore);
        LoadProfile(profile);
    }

    private async void RenameSequenceButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetSelectedSequenceContext(out InputProfile profile, out IInputProfileStore profileStore, out Sequence selectedSequence))
        {
            return;
        }

        TextInputDialogWindow dialog = new(
            title: "Rename Sequence",
            prompt: "Sequence name",
            initialValue: selectedSequence.Name,
            validator: ValidateSequenceName)
        {
            Owner = Window.GetWindow(this)
        };

        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult != true)
        {
            return;
        }

        selectedSequence.Name = dialog.Value;
        await SaveProfileAsync(profile, profileStore);
        LoadProfile(profile);
    }

    private async void DeleteSequenceButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetSelectedSequenceContext(out InputProfile profile, out IInputProfileStore profileStore, out Sequence selectedSequence))
        {
            return;
        }

        int removedIndex = profile.Sequences.IndexOf(selectedSequence);
        if (removedIndex < 0)
        {
            return;
        }

        profile.Sequences.RemoveAt(removedIndex);

        if (profile.Sequences.Count == 0)
        {
            profile.SelectedSequenceIndex = 0;
        }
        else
        {
            profile.SelectedSequenceIndex = Math.Min(removedIndex, profile.Sequences.Count - 1);
        }

        await SaveProfileAsync(profile, profileStore);
        LoadProfile(profile);
    }

    private async void AddStepButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetSelectedSequenceContext(out InputProfile profile, out IInputProfileStore profileStore, out Sequence selectedSequence))
        {
            return;
        }

        selectedSequence.Steps.Add(new SequenceStep
        {
            Name = GetDefaultSequenceStepName(selectedSequence.Steps.Count + 1),
            TargetType = SequenceStepTargetType.Keyboard,
            Key = null,
            MouseButton = null,
            MouseWheelDelta = 0,
            IsHold = false,
            DurationMilliseconds = 0,
            DelayAfterMilliseconds = 0,
            IsEnabled = true
        });

        NormalizeStepNames(selectedSequence);

        await SaveProfileAsync(profile, profileStore);
        LoadProfile(profile);

        if (_stepRows.Count > 0)
        {
            StepsDataGrid.SelectedIndex = _stepRows.Count - 1;
            StepsDataGrid.ScrollIntoView(StepsDataGrid.SelectedItem);
        }
    }

    private async void RemoveStepButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetSelectedSequenceContext(out InputProfile profile, out IInputProfileStore profileStore, out Sequence selectedSequence))
        {
            return;
        }

        if (StepsDataGrid.SelectedItem is not SequenceStepRow selectedRow)
        {
            return;
        }

        int removedIndex = selectedSequence.Steps.IndexOf(selectedRow.Step);
        if (removedIndex < 0)
        {
            return;
        }

        selectedSequence.Steps.RemoveAt(removedIndex);
        NormalizeStepNames(selectedSequence);

        await SaveProfileAsync(profile, profileStore);
        LoadProfile(profile);

        if (_stepRows.Count > 0)
        {
            StepsDataGrid.SelectedIndex = Math.Min(removedIndex, _stepRows.Count - 1);
        }
    }

    private async void SequenceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if (SequenceListBox.SelectedItem is not Sequence selectedSequence)
        {
            profile.SelectedSequenceIndex = 0;
            RefreshStepRows(null);
            await SaveProfileAsync(profile, profileStore);
            return;
        }

        int selectedIndex = profile.Sequences.IndexOf(selectedSequence);
        if (selectedIndex < 0)
        {
            return;
        }

        profile.SelectedSequenceIndex = selectedIndex;
        RefreshStepRows(selectedSequence);
        await SaveProfileAsync(profile, profileStore);
    }

    private void StepsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = null;
    }

    private void StepTargetButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not SequenceStepRow row)
        {
            return;
        }

        CancelCaptureMode();
        _capturingStepRow = row;
        row.IsCapturingTarget = true;
        RefreshStepRowDisplays();
        Focus();
        _ = Keyboard.Focus(this);
    }

    private async void StepHoldCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if ((sender as FrameworkElement)?.DataContext is not SequenceStepRow row)
        {
            return;
        }

        if (!row.Step.IsHold)
        {
            row.Step.DurationMilliseconds = 0;
        }

        await SaveProfileAsync(profile, profileStore);
        RefreshStepRowDisplays();
    }

    private async void StepDurationTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if ((sender as TextBox)?.DataContext is not SequenceStepRow row)
        {
            return;
        }

        if (!TryParseNonNegativeInteger((sender as TextBox)?.Text, out int durationMilliseconds))
        {
            return;
        }

        row.Step.DurationMilliseconds = durationMilliseconds;
        await SaveProfileAsync(profile, profileStore);
    }

    private async void StepDelayAfterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isLoadingProfile || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        if ((sender as TextBox)?.DataContext is not SequenceStepRow row)
        {
            return;
        }

        if (!TryParseNonNegativeInteger((sender as TextBox)?.Text, out int delayAfterMilliseconds))
        {
            return;
        }

        row.Step.DelayAfterMilliseconds = delayAfterMilliseconds;
        await SaveProfileAsync(profile, profileStore);
    }

    private void EditableCellControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        if (FindVisualParent<DataGridRow>(element) is DataGridRow row)
        {
            row.IsSelected = true;
        }
    }

    #endregion

    #region Capture

    private async void SequenceTabView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_capturingStepRow is null || !TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            return;
        }

        e.Handled = true;

        if (!InputCaptureMapper.TryMapToInputKey(e.Key, out InputKey key))
        {
            return;
        }

        _capturingStepRow.Step.TargetType = SequenceStepTargetType.Keyboard;
        _capturingStepRow.Step.Key = key;
        _capturingStepRow.Step.MouseButton = null;
        _capturingStepRow.IsCapturingTarget = false;

        await SaveProfileAsync(profile, profileStore);
        CompleteCaptureMode();
        RefreshStepRowDisplays();
    }

    private async void SequenceTabView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_capturingStepRow is null)
        {
            return;
        }

        if (!TryGetProfileContext(out InputProfile profile, out IInputProfileStore profileStore))
        {
            CancelCaptureMode();
            return;
        }

        if (e.ChangedButton != WpfMouseButton.Left &&
            e.ChangedButton != WpfMouseButton.Right &&
            e.ChangedButton != WpfMouseButton.Middle)
        {
            return;
        }

        e.Handled = true;

        if (!InputCaptureMapper.TryMapToMouseButton(e.ChangedButton, out Core.Enums.MouseButton mouseButton))
        {
            return;
        }

        _capturingStepRow.Step.TargetType = SequenceStepTargetType.MouseButton;
        _capturingStepRow.Step.MouseButton = mouseButton;
        _capturingStepRow.Step.Key = null;
        _capturingStepRow.IsCapturingTarget = false;

        await SaveProfileAsync(profile, profileStore);
        CompleteCaptureMode();
        RefreshStepRowDisplays();
    }

    private void CompleteCaptureMode()
    {
        if (_capturingStepRow is not null)
        {
            _capturingStepRow.IsCapturingTarget = false;
        }

        _capturingStepRow = null;
    }

    private void CancelCaptureMode()
    {
        CompleteCaptureMode();
        RefreshStepRowDisplays();
    }

    private void SequenceTabView_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isFocusedWithin && !isFocusedWithin && IsCapturingInput)
        {
            CancelCaptureMode();
        }
    }

    #endregion

    #region Validation / UI

    private void NonNegativeIntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !DigitsOnlyRegex.IsMatch(e.Text);
    }

    private void NonNegativeIntegerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
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

    private void RefreshStepRows(Sequence? sequence)
    {
        _stepRows.Clear();

        if (sequence is null)
        {
            return;
        }

        for (int index = 0; index < sequence.Steps.Count; index++)
        {
            _stepRows.Add(new SequenceStepRow(sequence.Steps[index], index + 1));
        }

        StepsDataGrid.SelectedIndex = -1;
    }

    private void RefreshStepRowDisplays()
    {
        foreach (SequenceStepRow row in _stepRows)
        {
            row.Refresh();
        }
    }

    private static void NormalizeStepNames(Sequence sequence)
    {
        for (int index = 0; index < sequence.Steps.Count; index++)
        {
            sequence.Steps[index].Name = GetDefaultSequenceStepName(index + 1);
        }
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

    private bool TryGetSelectedSequenceContext(out InputProfile profile, out IInputProfileStore profileStore, out Sequence selectedSequence)
    {
        selectedSequence = null!;

        if (!TryGetProfileContext(out profile, out profileStore))
        {
            return false;
        }

        if (SequenceListBox.SelectedItem is not Sequence sequence)
        {
            return false;
        }

        selectedSequence = sequence;
        return true;
    }

    private async Task SaveProfileAsync(InputProfile profile, IInputProfileStore profileStore)
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
    }

    private string? ValidateSequenceName(string rawName)
    {
        string trimmedName = rawName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return "Name is required.";
        }

        if (trimmedName.Length > MaxSequenceNameLength)
        {
            return $"Name must be {MaxSequenceNameLength} characters or less.";
        }

        return null;
    }

    private static bool TryParseNonNegativeInteger(string? text, out int value)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.CurrentCulture, out value))
        {
            return false;
        }

        return value >= 0;
    }

    private static string GenerateUniqueSequenceName(InputProfile profile, string baseName)
    {
        string normalizedBaseName = string.IsNullOrWhiteSpace(baseName)
            ? AppConstants.DefaultSequenceName
            : baseName.Trim();

        List<string> existingNames = profile.Sequences
            .Select(sequence => sequence.Name)
            .ToList();

        if (!existingNames.Any(name => string.Equals(name, normalizedBaseName, StringComparison.OrdinalIgnoreCase)))
        {
            return normalizedBaseName;
        }

        int suffix = 2;
        string candidateName;

        do
        {
            candidateName = $"{normalizedBaseName} {suffix}";
            suffix++;
        }
        while (existingNames.Any(name => string.Equals(name, candidateName, StringComparison.OrdinalIgnoreCase)));

        return candidateName;
    }

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

    private static string GetDefaultSequenceStepName(int stepNumber)
    {
        return $"Step {stepNumber}";
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

    #endregion

    private sealed class SequenceStepRow(SequenceStep step, int stepNumber) : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public SequenceStep Step { get; } = step;

        public int StepNumber { get; } = stepNumber;

        public bool IsCapturingTarget { get; set; }

        public string TargetDisplay
        {
            get
            {
                if (IsCapturingTarget)
                {
                    return "Press key...";
                }

                return Step.TargetType switch
                {
                    SequenceStepTargetType.Keyboard when Step.Key.HasValue => Step.Key.Value.ToString(),
                    SequenceStepTargetType.MouseButton when Step.MouseButton.HasValue => Step.MouseButton.Value.ToString(),
                    SequenceStepTargetType.MouseWheel => "Mouse Wheel",
                    _ => "Set Target"
                };
            }
        }

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(TargetDisplay)));
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Step)));
        }
    }
}