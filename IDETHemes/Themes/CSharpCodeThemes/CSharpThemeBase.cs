using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDEThemes.Themes.Interfaces;
using IDEThemes.Themes.RuleSets;

namespace IDEThemes.Themes.CSharpCodeThemes
{
    internal abstract class CSharpThemeBase: IThemeColors
    {
        public IList<HighlightingColor> Colors { get; protected set; } = new List<HighlightingColor>();
        public IDictionary<string, HighlightingColor> Properties { get; protected set; } = new Dictionary<string, HighlightingColor>()
        {
            {"Default", new HighlightingColor() },
            {"LineNumbersForeground", new HighlightingColor() },
            {"Selection", new HighlightingColor() },
            {"Hyperlink", new HighlightingColor() },
            {"NonPrintabelCharacters", new HighlightingColor() }
        };
        public HighlightingRuleSet RuleSet { get; }

        public CSharpThemeBase()
        {
            RuleSet = new CSharpRuleSet(this).RuleSet;
        }

        public void SetTheme(IHighlightingDefinition definition)
        {
            if (definition == null)
                definition = CSharpRuleSet.cSharpDef;
            SetDefinitionColors(definition);
            SetDefinitionSpans(definition);
        }
        private void SetDefinitionSpans(IHighlightingDefinition definition)
        {
            List<HighlightingSpan> spans = definition.MainRuleSet.Spans.ToList();
            spans.Clear();
            spans.AddRange(RuleSet.Spans);
        }
        private void SetDefinitionColors(IHighlightingDefinition definition)
        {
            List<HighlightingRule> rules = definition.MainRuleSet.Rules.ToList();
            rules.Clear();
            rules.ToList().AddRange(RuleSet.Rules);
        }

        protected List<HighlightingColor> GetColorsByXml(string xmlCSharpFile, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(xmlCSharpFile))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    IHighlightingDefinition definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    return definition.NamedHighlightingColors.ToList();
                }
            }
        }
    }
}
