using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using CodeBox;
using Completions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDEThemes.Themes.Interfaces;
using CodeBox.Completions;
using CodeBox.Enums;
using IDETHemes.Themes;
using IDETHemes.Themes.Enums;

namespace IDEThemes.Themes.CSharpThemes
{
    public abstract class ThemeBase: ITheme
    {

        #region InterfaceProperties
        public IList<HighlightingColor> Colors { get; protected set; } = new List<HighlightingColor>();
        public HighlightingRuleSet RuleSet { get; }
        public Brush Background { get; protected set; }
        public Brush Foreground { get; protected set; }
        public Brush LineNumbersForeground { get; protected set; }
        public Brush Selection { get; protected set; }
        public Brush Hyperlink { get; protected set; }
        public Brush NonPrintabelCharacters { get; protected set; }
        public Brush CompletionBorder { get; protected set; }
        public Brush CompletionBackground { get; protected set; }
        public Brush CompletionForeground { get; protected set; }
        public Brush CompletionSelectionBrush { get; protected set; }
        #endregion

        #region Override Properties

        public abstract string FilePath { get; set; }
        protected IHighlightingDefinition definition { get; set; }

        #endregion

        #region Ctor
        private const string DEFAULT_RESOURCE = "CSharpDarkTheme.xshd";
        public ThemeBase(Languages lang, DefaultThemesEnum dte)
        {
            FilePath = $"{lang}{dte}.xshd";
            Assembly assembly = Assembly.GetExecutingAssembly();
            string xmlCSharpFile = assembly.GetManifestResourceNames()
                .First(fp => fp.Contains(FilePath));
            definition = GetDefiniion(xmlCSharpFile, assembly);
            RuleSet = definition.MainRuleSet;
            Colors = definition.NamedHighlightingColors.ToList();
            SetProperties();
        }



        #endregion

        #region SetTheme

        public void SetTheme(TextEditor editor)
        {
            SetDefinitionColors(definition);
            SetDefinitionSpans(definition);
            editor.SyntaxHighlighting = definition;
            editor.Background = Background;
            editor.Foreground = Foreground;
            editor.TextArea.SelectionBrush = Selection;
            editor.TextArea.TextView.NonPrintableCharacterBrush = NonPrintabelCharacters;
            editor.TextArea.TextView.LinkTextForegroundBrush = Hyperlink;
            editor.TextArea.TextView.LinkTextUnderline = true;
            editor.LineNumbersForeground = LineNumbersForeground;
        }

    
        public void SetTheme(TextEditor editor, CompletionTheme th)
        {
            SetTheme(editor);
            if (th != null)
            {
                th.CompletionBackground = CompletionBackground;
                th.CompletionForeground = CompletionForeground;
                th.CompletionSelectionBrush = CompletionSelectionBrush;
                th.CompletionBorder = CompletionBorder;
            }
        }
        #endregion

        #region Definition manipulation methods
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
        #endregion

        #region SetProperties

        private void SetProperties()
        {
            DependencyProperty foreground = TextElement.ForegroundProperty;
            DependencyProperty background = TextElement.BackgroundProperty;
            for (int i = Colors.Count - 1; i > 0; i--)
            {
                switch (Colors[i].Name)
                {
                    case "Default":
                        {
                            Background = GetHCBrush(Colors[i], background);
                            Foreground = GetHCBrush(Colors[i], foreground);
                        }
                        break;
                    case "LineNumbersForeground":
                        LineNumbersForeground = GetHCBrush(Colors[i], foreground);
                        break;
                    case "Selection":
                        Selection = GetHCBrush(Colors[i], TextElement.BackgroundProperty);
                        break;
                    case "Hyperlink":
                        Hyperlink = GetHCBrush(Colors[i], foreground);
                        break;
                    case "NonPrintableCharacter":
                        NonPrintabelCharacters = GetHCBrush(Colors[i], foreground);
                        break;
                    case"CompletionBorder":
                        CompletionBorder = GetHCBrush(Colors[i], background);
                        break;
                    case "CompletionColors":
                        {
                            CompletionBackground = GetHCBrush(Colors[i], background);
                            CompletionForeground = GetHCBrush(Colors[i], foreground);
                            break;
                        }
                    case "CompletionSelectionBrush":
                        CompletionSelectionBrush = GetHCBrush(Colors[i], background);
                        break;
                }
            }
        }

        private Brush GetHCBrush(HighlightingColor color, DependencyProperty prop)
        {
            if (prop == TextElement.ForegroundProperty)
                return (Brush)new BrushConverter().ConvertFromInvariantString(color.Foreground.ToString());
            if(prop == TextElement.BackgroundProperty)
                return (Brush)new BrushConverter().ConvertFromInvariantString(color.Background.ToString());
            throw new Exception("Incorrect property");
        }
        #endregion

    }
}
