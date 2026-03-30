using System.Windows;

namespace AutoInputPlus.Wpf.Views.Dialogs;

/// <summary>
/// Reusable dialog for entering or renaming simple text values.
/// </summary>
public partial class TextInputDialogWindow : Window
{
    private readonly Func<string, string?> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextInputDialogWindow"/> class.
    /// </summary>
    /// <param name="title">The window title.</param>
    /// <param name="prompt">The prompt shown above the text field.</param>
    /// <param name="initialValue">The initial text value.</param>
    /// <param name="validator">Validation callback. Return null when valid; otherwise return an error message.</param>
    public TextInputDialogWindow(
        string title,
        string prompt,
        string initialValue,
        Func<string, string?> validator)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(initialValue);
        ArgumentNullException.ThrowIfNull(validator);

        InitializeComponent();

        Title = title;
        PromptTextBlock.Text = prompt;
        ValueTextBox.Text = initialValue;
        ValueTextBox.SelectAll();
        _validator = validator;
    }

    /// <summary>
    /// Gets the accepted text value.
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        string rawValue = ValueTextBox.Text;
        string? validationMessage = _validator(rawValue);

        if (validationMessage is not null)
        {
            ValidationTextBlock.Text = validationMessage;
            ValidationTextBlock.Visibility = Visibility.Visible;
            return;
        }

        ValidationTextBlock.Visibility = Visibility.Collapsed;
        Value = rawValue.Trim();
        DialogResult = true;
        Close();
    }
}