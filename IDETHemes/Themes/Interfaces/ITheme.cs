using ICSharpCode.AvalonEdit.Highlighting;
using IDEThemes.Themes.Interfaces;
using System.Windows.Media;

namespace IDETHemes.Themes.Interfaces
{
    interface ITheme
    {
        IThemeColors Colors { get; }
        HighlightingColor Default { get; }
        HighlightingColor LineNumbersForeground { get; }
        Brush Selection { get; }
        HighlightingColor Hyperlink { get; }
        HighlightingColor NonPrintabelCharacters { get; }
    }
}
