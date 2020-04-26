using System.Linq;
using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDEThemes.Themes.Interfaces;

namespace IDEThemes.Themes.RuleSets
{
    public class CSharpRuleSet: IRuleSet
    {
        public HighlightingRuleSet RuleSet { get; private set; } = new HighlightingRuleSet();

        internal static IHighlightingDefinition cSharpDef { get; private set; }

        public string Extension { get; } = ".cs";
        public string Name { get; } = "CSharp";

        public CSharpRuleSet(IThemeColors colors)
        {
            Initalize(colors);
        }

        public void Initalize(IThemeColors colors)
        {
            SetRegexRules(); 
        }

        #region SetRegexRules

        private void SetRegexRules()
        {
            if (cSharpDef != null)
            {
                RuleSet = cSharpDef.MainRuleSet;
            }
            else
            {
                IHighlightingDefinition definition = GetDefinition();
                cSharpDef = definition;
                RuleSet = definition.MainRuleSet;
            }
        }

        private IHighlightingDefinition GetDefinition()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string xmlCSharpFile = assembly.GetManifestResourceNames()
                .First(fp => fp.Contains("CSharp.xshd")); //searching xshd file in resources of assembly

            using (var stream = assembly.GetManifestResourceStream(xmlCSharpFile))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    IHighlightingDefinition definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    return definition;
                }
            }
        }

        #endregion

    }
}
