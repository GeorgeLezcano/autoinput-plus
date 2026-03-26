using UserControl = System.Windows.Controls.UserControl;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for SequenceTabView.
/// </summary>
public partial class SequenceTabView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceTabView"/> class.
    /// </summary>
    public SequenceTabView()
    {
        InitializeComponent();
    }

    #region Handlers

    private void NewSequenceButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
           $"Feature '{nameof(NewSequenceButton_Click)}' not implemented");
    }

    private void RenameSequenceButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
           $"Feature '{nameof(RenameSequenceButton_Click)}' not implemented");
    }

    private void DeleteSequenceButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
           $"Feature '{nameof(DeleteSequenceButton_Click)}' not implemented");
    }

    private void AddStepButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
           $"Feature '{nameof(AddStepButton_Click)}' not implemented");
    }

    private void RemoveStepButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
           $"Feature '{nameof(RemoveStepButton_Click)}' not implemented");
    }

    #endregion
}