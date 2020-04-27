using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using System.Windows.Media;

namespace IDEThemes.Themes.Interfaces
{
    public interface ITheme
    {
        IList<HighlightingColor> Colors { get; }
        HighlightingRuleSet RuleSet { get; }
        void SetTheme(TextEditor editor);
        Brush Background { get;}
        Brush Foreground { get; }
        HighlightingColor LineNumbersForeground { get; }
        Brush Selection { get; }
        HighlightingColor Hyperlink { get; }
        HighlightingColor NonPrintabelCharacters { get; }
    }
}
