using System.Windows;
using Clipboard = System.Windows.Clipboard;

namespace AutoInputPlus.Wpf.Views.Dialogs;

/// <summary>
/// Interaction logic for ExportProfileWindow.
/// </summary>
public partial class ExportProfileWindow : Window
{
    private const string CopyIcon = "\uE8C8";
    private const string CheckIcon = "\uE73E";

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProfileWindow"/> class.
    /// </summary>
    /// <param name="exportedProfile">
    /// The exported encoded profile string to display.
    /// </param>
    public ExportProfileWindow(string exportedProfile)
    {
        ArgumentNullException.ThrowIfNull(exportedProfile);

        InitializeComponent();

        ExportTextBox.Text = exportedProfile;
    }

    /// <summary>
    /// Copies the exported profile string to the clipboard and closes the window.
    /// </summary>
    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ExportTextBox.Text))
        {
            return;
        }

        Clipboard.SetText(ExportTextBox.Text);
        Close();
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}