using System.Windows;
using AutoInputPlus.Core.Metadata;
using UserControl = System.Windows.Controls.UserControl;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Interaction logic for AboutTabView.
/// </summary>
public partial class AboutTabView : UserControl
{
    private const string CopyIcon = "\uE8C8";
    private const string CheckIcon = "\uE73E";

    /// <summary>
    /// Initializes a new instance of the <see cref="AboutTabView"/> class.
    /// </summary>
    public AboutTabView()
    {
        InitializeComponent();
        PopulateFields();
    }

    /// <summary>
    /// Populates the about tab fields from project file.
    /// </summary>
    private void PopulateFields()
    {
        ProductTextBlock.Text = AppMetadata.GetValue("Product");
        AuthorTextBlock.Text = AppMetadata.GetValue("Authors");
        RepositoryTextBlock.Text = AppMetadata.GetValue("RepositoryUrl");
        VersionTextBlock.Text = AppMetadata.GetVersion();
    }

    /// <summary>
    /// Copies the repository URL to the clipboard and briefly shows a success icon.
    /// </summary>
    private async void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
    {
        string textToCopy = RepositoryTextBlock.Text;

        if (string.IsNullOrWhiteSpace(textToCopy) || textToCopy == "-")
        {
            return;
        }

        System.Windows.Clipboard.SetText(textToCopy);

        CopyToClipboardButton.Content = CheckIcon;

        await Task.Delay(TimeSpan.FromSeconds(1.5));

        CopyToClipboardButton.Content = CopyIcon;
    }
}