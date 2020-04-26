using ICSharpCode.AvalonEdit.Highlighting;
using IDEThemes.Themes.Interfaces;
using System.Collections.Generic;

namespace IDEThemes.Themes.Interfaces
{
    public interface IRuleSet
    {
        HighlightingRuleSet RuleSet { get; }
        string Extension { get; }
        string Name { get; }
        void Initalize(IThemeColors colors);
    }
}
