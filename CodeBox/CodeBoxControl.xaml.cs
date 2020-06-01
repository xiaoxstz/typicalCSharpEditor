using ICSharpCode.AvalonEdit.Folding;
using IDEThemes.Themes.CSharpThemes;
using IDEThemes.Themes.Interfaces;
using IDETHemes.Themes.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using Foldings;
using System.Linq;
using System.Windows.Input;
using Indents;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Completions;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBox
{
    public partial class CodeBoxControl : UserControl
    {
        public CodeBoxControl()
        {
            InitializeComponent();
            InitializeResources();
            textEditor.TextArea.IndentationStrategy = new CSharpIndent(textEditor.TextArea.Caret);
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            Theme.SetTheme(textEditor, CustomCompletionControl.Theme);
            SetFolding();
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
        }

        #region Events
        #region TextChanged
        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            CheckAutoSymbols();
            List<NewFolding> foldings = new List<NewFolding>();
            braceFolding.UpdateFoldings(foldings, textEditor.Document);
            regionFolding.UpdateFoldings(foldings, textEditor.Document);
            foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            foldingManager.UpdateFoldings(foldings, -1);
        }

        #endregion

        #region Caret_PositionChanged
        private int lastXPosition = 1;
        private int lastYPosition = 1;

    private void Caret_PositionChanged(object sender, EventArgs e)
        {
            Caret caret = sender as Caret;
            if (lastYPosition != caret.Line)
            {
                DocumentLine line = textEditor.Document.GetLineByNumber(caret.Line);
                if (line.Length > 0 && lastXPosition > caret.Column &&
                   line.Length >= lastXPosition)
                {
                    lastXPosition = line.Length - 1;
                    caret.Column = lastXPosition;
                }
            }
            lastYPosition = caret.Line;
            lastXPosition = caret.Column;

        }
        #endregion
                
        #region TextEditor_PreviewTextInput


        private void TextEditor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            IsBrace = e.Text == "{";
            IsBracket = e.Text == "(";
        }

        #endregion
        #endregion

        #region Methods

        #region SetFolding

        #region Properties

        private BraceFoldingStrategy braceFolding { get; set; } = new BraceFoldingStrategy();
        private RegionFoldingStrategy regionFolding { get; set; } = new RegionFoldingStrategy();
        private FoldingManager foldingManager { get; set; }

        #endregion

        private void SetFolding()
        {
            foldingManager = FoldingManager.Install(textEditor.TextArea);
            FoldingMargin m = new FoldingMargin() { FoldingManager = foldingManager };
            m.FoldingMarkerBackgroundBrush = Brushes.Transparent;
            m.SelectedFoldingMarkerBackgroundBrush = Brushes.Transparent;
            m.FoldingMarkerBrush = textEditor.Foreground;
            m.SelectedFoldingMarkerBrush = textEditor.Foreground;
            var lm = textEditor.TextArea.LeftMargins;
            lm.Remove(lm.Last());
            textEditor.TextArea.LeftMargins.Add(m);
        }

        #endregion

        #region AutoSymbols

        private bool IsBracket { get; set; }
        private bool IsBrace { get; set; }

        private void CheckAutoSymbols()
        {
            CheckBraces();
            CheckBrackets();
            IsBrace = false;
            IsBracket = false;
        }

        private const char OPEN_BRACE = '{';
        private const string CLOSE_BRACE = "  }";
        private void CheckBraces()
        {
            if (IsBrace)
                AutoSymbolsPattern(OPEN_BRACE, CLOSE_BRACE);
        }

        private const char OPEN_BRACKET = '(';
        private const string CLOSE_BRACKET = ")";
        private void CheckBrackets()
        {
            if (IsBracket)
                AutoSymbolsPattern(OPEN_BRACKET, CLOSE_BRACKET);
        }

        private void AutoSymbolsPattern(char ch, string insertString)
        {
            DocumentLine currentLine = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
            string currentText = textEditor.Document.GetText(currentLine.Offset, currentLine.Length);
            string noSpacesText = currentText.Replace(" ", "");
            if (noSpacesText.Length > 0 && noSpacesText.Last() == ch)
            {
                for (int i = currentText.Length - 1; currentText[i] == ' '; i--)
                    currentText = currentText.Remove(i, 1);
                textEditor.Document.Replace(currentLine.Offset, currentLine.Length, currentText + insertString);
                textEditor.TextArea.Caret.Column = textEditor.Document.GetLineByNumber(currentLine.LineNumber).Length;
            }
        }
        #endregion

        #region Completion
        private CustomCompletionControl completionWindow { get; set; }

        #region Events

        async void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow == null && !char.IsWhiteSpace(e.Text[0]))
            {
                completionWindow = new CustomCompletionControl(textEditor.TextArea);
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
                await Task.Delay(700);
                completionWindow?.Show();
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (
                    !char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }
        #endregion

        #endregion

        #region InitalizeResources

        private void InitializeResources()
        {
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
            new Uri("HandyControl;component/Themes/Styles/Base/ListBoxBaseStyle.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
           new Uri("CodeBox;component/Completions/CSCompletion/Styles/CompletionWindowStyle.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
          new Uri("HandyControl;component/Themes/SkinDefault.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
          new Uri("HandyControl;component/Themes/Theme.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
          new Uri("CodeBox;component/Completions/CSCompletion/Styles/CompletionListStyle.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("CodeBox;component/Completions/CSCompletion/Styles/CompletionToolTipStyle.xaml", UriKind.Relative)) as ResourceDictionary);

        }

        #endregion

        #endregion

        #region DependencyProperties

        #region DefaultTheme

        public static readonly DependencyProperty DefaultThemeProperty =
    DependencyProperty.Register("DefaultTheme", typeof(DefaultThemesEnum), typeof(CodeBoxControl),
        new PropertyMetadata(DefaultThemesEnum.DarkTheme, new PropertyChangedCallback(DefaultThemePropertyChanged)));

        public DefaultThemesEnum DefaultTheme
        {
            get { return (DefaultThemesEnum)GetValue(DefaultThemeProperty); }
            set { SetValue(DefaultThemeProperty, value); }
        }


        private static void DefaultThemePropertyChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            CodeBoxControl cb = d as CodeBoxControl;
            cb.DefaultThemeChanged(e);
        }

        private void DefaultThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            switch(DefaultTheme)
            {
                case DefaultThemesEnum.BlueTheme:

                    Theme = new BlueTheme();
                    break;
                case DefaultThemesEnum.LightTheme:
                    Theme = new LightTheme();
                    break;
                default:
                    Theme = new DarkTheme();
                    break;
            }
            Theme.SetTheme(textEditor, CustomCompletionControl.Theme);
        }

        #endregion

        #region Language
        public static readonly DependencyProperty LanguageProperty =
   DependencyProperty.Register("Language", typeof(Languages), typeof(CodeBoxControl),
       new PropertyMetadata(Languages.CSharp));

        public Languages Language
        {
            get { return (Languages)GetValue(DefaultThemeProperty); }
            set { SetValue(DefaultThemeProperty, value); }
        }
        #endregion

        #region Theme

        public static readonly DependencyProperty ThemeProperty =
    DependencyProperty.Register("Theme", typeof(ITheme), typeof(CodeBoxControl),
        new PropertyMetadata(new DarkTheme()));

        public ITheme Theme
        {
            get { return (ITheme)GetValue(ThemeProperty); }
            set
            {
                value.SetTheme(textEditor);
                SetValue(ThemeProperty, value);
            }
        }



        #endregion

        #endregion

        public static Action TabAction { get; set; }
        public static bool IsSnippetCompletion { get; set; }
        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab && IsSnippetCompletion)
            {
                e.Handled = true;
                TabAction();
            }
            else if((e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Return) && IsSnippetCompletion)
            {
                IsSnippetCompletion = false;
                TabAction = null;
            }
        }
    }
}
