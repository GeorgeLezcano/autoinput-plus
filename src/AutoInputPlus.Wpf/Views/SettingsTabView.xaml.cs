using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for SettingsTabView.
/// </summary>
public partial class SettingsTabView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsTabView"/> class.
    /// </summary>
    public SettingsTabView()
    {
        InitializeComponent();
    }

    #region Hanlders

    private void SingleInputModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void SequenceModeRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(SequenceModeRadioButton_Checked)}' not implemented");
    }

    private void CaptureHotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(CaptureHotkeyButton_Click)}' not implemented");
    }

    private void CaptureTargetKeyButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(CaptureTargetKeyButton_Click)}' not implemented");
    }

    private void IntervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        //TODO
    }

    private void RunUntilStopRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void RunForSetCountRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(RunForSetCountRadioButton_Checked)}' not implemented");
    }

    private void RunCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        //TODO
    }

    private void HoldCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(HoldCheckBox_Checked)}' not implemented");
    }

    private void HoldCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(HoldCheckBox_Unchecked)}' not implemented");
    }

    private void ScheduleStartEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(ScheduleStartEnabledCheckBox_Checked)}' not implemented");
    }

    private void ScheduleStartEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(ScheduleStartEnabledCheckBox_Unchecked)}' not implemented");
    }

    private void ScheduleStopEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(ScheduleStopEnabledCheckBox_Checked)}' not implemented");
    }

    private void ScheduleStopEnabledCheckBox_Uchecked(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"Feature '{nameof(ScheduleStopEnabledCheckBox_Uchecked)}' not implemented");
    }

    #endregion
}