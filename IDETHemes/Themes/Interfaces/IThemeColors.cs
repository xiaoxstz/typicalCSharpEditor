using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using IDEThemes.Themes.Interfaces;

namespace IDEThemes.Themes.Interfaces
{
    public interface IThemeColors
    {
        IList<HighlightingColor> Colors { get; }
        IDictionary<string, HighlightingColor> Properties { get; } 
        HighlightingRuleSet RuleSet { get; }
        void SetTheme(IHighlightingDefinition definition);
    }
}
