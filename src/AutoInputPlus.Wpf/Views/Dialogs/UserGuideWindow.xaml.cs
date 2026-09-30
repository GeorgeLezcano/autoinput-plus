using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Resources;

namespace AutoInputPlus.Wpf.Views.Dialogs;

/// <summary>
/// The user guide help window.
/// </summary>
public partial class UserGuideWindow : Window
{
    private const string UserGuideUri = "/Assets/UserGuide.md";

    /// <summary>
    /// Default constructor.
    /// </summary>
    public UserGuideWindow()
    {
        InitializeComponent();

        LoadUserGuide();
    }

    private void LoadUserGuide()
    {
        Uri resourceUri = new(UserGuideUri, UriKind.Relative);

        StreamResourceInfo? resource = System.Windows.Application.GetResourceStream(resourceUri);
        if (resource is null)
        {
            return;
        }

        using StreamReader reader = new(resource.Stream);
        string markdown = reader.ReadToEnd();

        FlowDocumentScrollViewer viewer = new()
        {
            Document = CreateDocument(markdown),
            IsToolBarVisible = false,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        Content = viewer;
    }

    private static FlowDocument CreateDocument(string markdown)
    {
        FlowDocument document = new()
        {
            PagePadding = new Thickness(32),
            FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
            FontSize = 14
        };

        List? currentList = null;

        foreach (string rawLine in markdown.Split('\n'))
        {
            string line = rawLine.TrimEnd('\r');

            if (string.IsNullOrWhiteSpace(line))
            {
                currentList = null;
                continue;
            }

            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                currentList = null;
                document.Blocks.Add(CreateHeading(line[4..], 18));
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                currentList = null;
                document.Blocks.Add(CreateHeading(line[3..], 22));
                continue;
            }

            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                currentList = null;
                document.Blocks.Add(CreateHeading(line[2..], 28));
                continue;
            }

            if (line.StartsWith("- ", StringComparison.Ordinal))
            {
                if (currentList?.MarkerStyle != TextMarkerStyle.Disc)
                {
                    currentList = new List
                    {
                        MarkerStyle = TextMarkerStyle.Disc
                    };

                    document.Blocks.Add(currentList);
                }

                currentList.ListItems.Add(
                    new ListItem(CreateParagraph(line[2..])));

                continue;
            }

            int periodIndex = line.IndexOf('.');
            bool isNumberedItem =
                periodIndex > 0 &&
                int.TryParse(line[..periodIndex], out _) &&
                line.Length > periodIndex + 1 &&
                line[periodIndex + 1] == ' ';

            if (isNumberedItem)
            {
                if (currentList?.MarkerStyle != TextMarkerStyle.Decimal)
                {
                    currentList = new List
                    {
                        MarkerStyle = TextMarkerStyle.Decimal
                    };

                    document.Blocks.Add(currentList);
                }

                currentList.ListItems.Add(
                    new ListItem(CreateParagraph(line[(periodIndex + 2)..])));

                continue;
            }

            currentList = null;
            document.Blocks.Add(CreateParagraph(line));
        }

        return document;
    }

    private static Paragraph CreateHeading(string text, double fontSize)
    {
        Paragraph paragraph = CreateParagraph(text);
        paragraph.FontSize = fontSize;
        paragraph.FontWeight = FontWeights.SemiBold;
        paragraph.Margin = new Thickness(0, 16, 0, 8);

        return paragraph;
    }

    private static Paragraph CreateParagraph(string text)
    {
        Paragraph paragraph = new()
        {
            Margin = new Thickness(0, 4, 0, 8)
        };

        AddInlines(paragraph.Inlines, text);

        return paragraph;
    }

    private static void AddInlines(InlineCollection inlines, string text)
    {
        int position = 0;

        while (position < text.Length)
        {
            int boldStart = text.IndexOf("**", position, StringComparison.Ordinal);

            if (boldStart < 0)
            {
                inlines.Add(new Run(text[position..]));
                break;
            }

            if (boldStart > position)
            {
                inlines.Add(new Run(text[position..boldStart]));
            }

            int boldEnd = text.IndexOf(
                "**",
                boldStart + 2,
                StringComparison.Ordinal);

            if (boldEnd < 0)
            {
                inlines.Add(new Run(text[boldStart..]));
                break;
            }

            inlines.Add(
                new Bold(
                    new Run(text[(boldStart + 2)..boldEnd])));

            position = boldEnd + 2;
        }
    }
}