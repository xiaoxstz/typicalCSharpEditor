using ICSharpCode.AvalonEdit.Folding;
using IDEThemes.Themes.CSharpThemes;
using IDEThemes.Themes.Interfaces;
using IDETHemes.Themes.Enums;
using System;
using System.Windows;
using CodeBox.Enums;
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
using CodeBox.Completions.CSCompletion;
using ICSharpCode.AvalonEdit;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeBox
{
    public class CodeBoxControl : TextEditor, INotifyPropertyChanged
    {

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion


        public CodeBoxControl()
        {
            InitializeResources();
            TextArea.IndentationStrategy = new CSharpIndent(TextArea.Caret);
            TextArea.Caret.PositionChanged += Caret_PositionChanged;
            Theme.SetTheme(this, CustomCompletionControl.Theme);
            SetFolding();
            TextArea.TextEntered += textEditor_TextArea_TextEntered;
            TextArea.TextEntering += textEditor_TextArea_TextEntering;
            KeyDown += textEditor_KeyDown;
            PreviewKeyDown += textEditor_prKeyDown;
            TextArea.SelectionChanged += SelectionChanged;
            FontFamily = new FontFamily("Consolas");
            FontSize = 15;
            ShowLineNumbers = true;
            TextChanged += TextEditor_TextChanged;
            PreviewTextInput += TextEditor_PreviewTextInput;


            Document.UndoStack.SizeLimit = 0;
        }

        #region Events
        #region TextChanged
        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            UndoOperation op = new UndoOperation(CaretOffset, Text);
            UndoOperations.Push(op);
            Text = Document.Text;
            CheckAutoSymbols();
            List<NewFolding> foldings = new List<NewFolding>();
            braceFolding.UpdateFoldings(foldings, Document);
            regionFolding.UpdateFoldings(foldings, Document);
            foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            foldingManager.UpdateFoldings(foldings, -1);
        }

        #endregion

        #region SelectionChanged
        private void SelectionChanged(object sender, EventArgs e)
        {
            SelectedTextLength = SelectionLength;
            SelectionOffset = SelectionStart;
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
                DocumentLine line = Document.GetLineByNumber(caret.Line);
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
            foldingManager = FoldingManager.Install(TextArea);
            FoldingMargin m = new FoldingMargin() { FoldingManager = foldingManager };
            m.FoldingMarkerBackgroundBrush = Brushes.Transparent;
            m.SelectedFoldingMarkerBackgroundBrush = Brushes.Transparent;
            m.FoldingMarkerBrush = Foreground;
            m.SelectedFoldingMarkerBrush = Foreground;
            var lm = TextArea.LeftMargins;
            lm.Remove(lm.Last());
            TextArea.LeftMargins.Add(m);
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
            DocumentLine currentLine = Document.GetLineByNumber(TextArea.Caret.Line);
            string currentText = Document.GetText(currentLine.Offset, currentLine.Length);
            string noSpacesText = currentText.Replace(" ", "");
            if (noSpacesText.Length > 0 && noSpacesText.Last() == ch)
            {
                for (int i = currentText.Length - 1; currentText[i] == ' '; i--)
                    currentText = currentText.Remove(i, 1);
                Document.Replace(currentLine.Offset, currentLine.Length, currentText + insertString);
                CaretOffset--;
            }
        }
        #endregion

        #region Completion
        private CustomCompletionControl completionWindow { get; set; }

        #region Events

        async void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow == null && !char.IsWhiteSpace(e.Text[0]) && IsCompletionEnable)
            {
                completionWindow = new CustomCompletionControl(TextArea);
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
            if (e.Text.Length > 0 && completionWindow != null && IsCompletionEnable)
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
            //Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
            //new Uri("HandyControl;component/Themes/Styles/Base/ListBoxBaseStyle.xaml", UriKind.Relative)) as ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
           new Uri("CodeBox;component/Completions/CSCompletion/Styles/CompletionWindowStyle.xaml", UriKind.Relative)) as ResourceDictionary);

            //  Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
            //new Uri("HandyControl;component/Themes/SkinDefault.xaml", UriKind.Relative)) as ResourceDictionary);

            //  Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
            //new Uri("HandyControl;component/Themes/Theme.xaml", UriKind.Relative)) as ResourceDictionary);

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
            switch (DefaultTheme)
            {
                case DefaultThemesEnum.DarkTheme:
                    Theme = new DarkTheme(ProgrammingLanguage);
                    break;
                case DefaultThemesEnum.LightTheme:
                    Theme = new LightTheme(ProgrammingLanguage);
                    break;
                default:
                    Theme = new BlueTheme(ProgrammingLanguage);
                    break;
            }
            Theme.SetTheme(this, CustomCompletionControl.Theme);
        }

        #endregion

        #region Language
        public static readonly DependencyProperty ProgrammingLanguageProperty =
DependencyProperty.Register("ProgrammingLanguage", typeof(Languages), typeof(CodeBoxControl),
   new PropertyMetadata(Languages.CSharp, new PropertyChangedCallback(DefaultThemePropertyChanged)));

        public Languages ProgrammingLanguage
        {
            get { return (Languages)GetValue(ProgrammingLanguageProperty); }
            set { SetValue(ProgrammingLanguageProperty, value); }
        }




        #endregion

        #region Theme

        public static readonly DependencyProperty ThemeProperty =
    DependencyProperty.Register("Theme", typeof(ITheme), typeof(CodeBoxControl),
        new PropertyMetadata(new DarkTheme(Languages.CSharp)));

        public ITheme Theme
        {
            get { return (ITheme)GetValue(ThemeProperty); }
            set
            {
                value.SetTheme(this);
                SetValue(ThemeProperty, value);
            }
        }

        #endregion

        #region UndoStack

        public static readonly DependencyProperty UndoOperationsProperty =
    DependencyProperty.Register("UndoOperations", typeof(UndoStack), typeof(CodeBoxControl),
        new PropertyMetadata(new UndoStack()));

        /// <summary>
        /// Stack with the history of operations (contains undo and redo operations -> ctrl+z; ctrl+y).
        /// </summary>
        public UndoStack UndoOperations
        {
            get { return (UndoStack)GetValue(UndoOperationsProperty); }
            set
            {
                SetValue(ThemeProperty, value);
            }
        }

        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(CodeBoxControl),
          new FrameworkPropertyMetadata
          {
              DefaultValue = default(string),
              BindsTwoWayByDefault = true,
              PropertyChangedCallback = OnTextChanged

          });

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged("Text");
            }
        }

        private static void OnTextChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            CodeBoxControl UserControl1Control = d as CodeBoxControl;
            UserControl1Control.OnTextChanged(e);
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                int caretOffset = TextArea.Caret.Offset;
                Document.Text = e.NewValue.ToString();
                if (caretOffset >= Document.Lines.Last().EndOffset)
                    caretOffset = Document.Lines.Last().EndOffset;
                CaretOffset = caretOffset;
            }
        }

        #endregion

        #endregion


        #region SelectedTextLength
        public static readonly DependencyProperty SelectedTextLengthProperty =
                DependencyProperty.Register("SelectedTextLength", typeof(int), typeof(CodeBoxControl),
                   new FrameworkPropertyMetadata
                   {
                       DefaultValue = default(int),
                       BindsTwoWayByDefault = true
                   });

        public int SelectedTextLength
        {
            get { return (int)GetValue(SelectedTextLengthProperty); }
            set
            {
                SetValue(SelectedTextLengthProperty, value);
                OnPropertyChanged("SelectedTextLength");
            }
        }



        #endregion

        #region SelectionOffset
        public static readonly DependencyProperty SelectionOffsetProperty =
                DependencyProperty.Register("SelectionOffset", typeof(int), typeof(CodeBoxControl),
                  new FrameworkPropertyMetadata
                  {
                      DefaultValue = default(int),
                      BindsTwoWayByDefault = true
                  });

        public int SelectionOffset
        {
            get { return (int)GetValue(SelectionOffsetProperty); }
            set
            {
                SetValue(SelectionOffsetProperty, value);
                OnPropertyChanged("SelectionOffset");
            }
        }

        #endregion

        #region IsCompletionEnable
        public static readonly DependencyProperty IsCompletionEnableProperty =
   DependencyProperty.Register("IsCompletionEnable", typeof(bool), typeof(CodeBoxControl),
       new PropertyMetadata(false));

        public bool IsCompletionEnable
        {
            get { return (bool)GetValue(IsCompletionEnableProperty); }
            set { SetValue(IsCompletionEnableProperty, value); }
        }
        #endregion



        public static Action TabAction { get; set; }//for changing words in snippet
        public static Action EnterAction { get; set; }//for code after filling snippet
        public static bool IsSnippetCompletion { get; set; }
        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsCompletionEnable)
            {
                if (e.Key == Key.Enter && IsSnippetCompletion && EnterAction != null)
                {
                    e.Handled = true;
                    EnterAction();
                    EnterAction = null;
                }
                if (e.Key == Key.Tab && IsSnippetCompletion)
                {
                    e.Handled = true;
                    TabAction();
                }
                else if ((e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Return || e.Key == Key.Back
                    || e.Key == Key.LeftCtrl || e.Key == Key.LeftAlt) && IsSnippetCompletion)
                    CodeSnippet.Clear(TextArea);
            }
           
        }

        private void textEditor_prKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.Z)
                && UndoOperations.Count != 0)
                UndoOperations.Undo(Document, TextArea);
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.Y)
               && UndoOperations.RedoCount != 0)
                UndoOperations.Redo(Document, TextArea);
        }
    }
}
