using CodeBox.Completions;
using CodeBox.Enums;
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
        void SetTheme(TextEditor editor, CompletionTheme theme);
        Brush Background { get;}
        Brush Foreground { get; }
        Brush LineNumbersForeground { get; }
        Brush Selection { get; }
        Brush Hyperlink { get; }
        Brush NonPrintabelCharacters { get; }
        Brush CompletionBorder { get; }
        Brush CompletionBackground { get; }
        Brush CompletionForeground { get; }
        Brush CompletionSelectionBrush { get; }
    }
}
