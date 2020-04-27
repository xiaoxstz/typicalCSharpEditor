using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDEThemes.Themes.Interfaces;

namespace IDEThemes.Themes.CSharpThemes
{
    public abstract class CSharpThemeBase: ITheme
    {
        #region InterfaceProperties
        public IList<HighlightingColor> Colors { get; protected set; } = new List<HighlightingColor>();
        public HighlightingRuleSet RuleSet { get; }
        public Brush Background { get; protected set; }
        public Brush Foreground { get; protected set; }
        public HighlightingColor LineNumbersForeground { get; protected set; }
        public Brush Selection { get; protected set; }
        public HighlightingColor Hyperlink { get; protected set; }
        public HighlightingColor NonPrintabelCharacters { get; protected set; }
        #endregion

        protected IHighlightingDefinition definition { get; set; }

        public CSharpThemeBase()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string xmlCSharpFile = assembly.GetManifestResourceNames()
                .First(fp => fp.Contains("CSharpDarkThemeColors.xshd"));
            definition = GetDefiniion(xmlCSharpFile, assembly);
            RuleSet = definition.MainRuleSet;
            Colors = definition.NamedHighlightingColors.ToList();
        }

        public void SetTheme(TextEditor editor)
        {
            SetDefinitionColors(definition);
            SetDefinitionSpans(definition);
            editor.SyntaxHighlighting = definition;
            editor.Background = Background;
            editor.Foreground = Foreground;
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

        protected IHighlightingDefinition GetDefiniion(string xmlCSharpFile, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(xmlCSharpFile))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }


    }
}
