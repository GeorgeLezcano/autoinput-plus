using System.Windows;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Wpf.Views.Dialogs;

/// <summary>
/// Interaction logic for ImportProfileWindow.
/// </summary>
public partial class ImportProfileWindow : Window
{
    private readonly IProfileExchange _profileExchange;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportProfileWindow"/> class.
    /// </summary>
    /// <param name="profileExchange">
    /// Profile exchange service used to validate and import encoded profile strings.
    /// </param>
    public ImportProfileWindow(IProfileExchange profileExchange)
    {
        ArgumentNullException.ThrowIfNull(profileExchange);

        InitializeComponent();

        _profileExchange = profileExchange;
    }

    /// <summary>
    /// Gets the encoded profile string entered by the user.
    /// </summary>
    public string EncodedProfile => ImportTextBox.Text.Trim();

    /// <summary>
    /// Shows a validation or error message.
    /// </summary>
    /// <param name="message">
    /// The message to display.
    /// </param>
    public void ShowValidationMessage(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        ValidationTextBlock.Text = message;
        ValidationTextBlock.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Clears the validation or error message.
    /// </summary>
    public void ClearValidationMessage()
    {
        ValidationTextBlock.Text = string.Empty;
        ValidationTextBlock.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Validates the encoded profile string and only closes when it is valid.
    /// </summary>
    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        string encodedProfile = ImportTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(encodedProfile))
        {
            ShowValidationMessage("Paste an encoded profile string before importing.");
            return;
        }

        ImportButton.IsEnabled = false;
        ClearValidationMessage();

        try
        {
            await _profileExchange.ImportProfileAsync(encodedProfile);

            DialogResult = true;
        }
        catch (Exception ex)
        {
            ShowValidationMessage(ex.Message);
        }
        finally
        {
            ImportButton.IsEnabled = true;
        }
    }

    /// <summary>
    /// Cancels the dialog.
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    /// <summary>
    /// Clears validation text when the input changes.
    /// </summary>
    private void ImportTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (ValidationTextBlock.Visibility == Visibility.Visible)
        {
            ClearValidationMessage();
        }
    }
}