using ICSharpCode.AvalonEdit.Folding;
using IDEThemes.Themes.CSharpThemes;
using IDEThemes.Themes.Interfaces;
using IDETHemes.Themes.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using typicalIDE.CodeBox.Enums;
using typicalIDE.CodeBox.Folding;
using System.Linq;
using typicalIDE.CodeBox.Indents;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace typicalIDE.CodeBox
{
    public partial class CodeBoxControl : UserControl
    {
        private BraceFoldingStrategy braceFolding { get; set; } = new BraceFoldingStrategy();
        private FoldingManager foldingManager { get; set; }

        public CodeBoxControl()
        {
            InitializeComponent();
            textEditor.TextArea.IndentationStrategy = new CSharpIndent();
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            foldingManager = FoldingManager.Install(textEditor.TextArea);
            Theme.SetTheme(textEditor);
        }

        #region Methods
        #region TextChanged
        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            CheckBraces();
            braceFolding.UpdateFoldings(foldingManager, textEditor.Document);
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

        #region CheckBraces

        private const char OPEN_BRACKET = '{';
        private const string CLOSE_BRACKET = "  }";

        private void CheckBraces()
        {
            DocumentLine currentLine = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
            string currentText = textEditor.Document.GetText(currentLine.Offset, currentLine.EndOffset - currentLine.Offset);
            if (currentText.Length > 0 && currentText.Last() == OPEN_BRACKET)
            {
                textEditor.Document.Insert(currentLine.EndOffset, CLOSE_BRACKET;
                textEditor.TextArea.Caret.Column--;
            }
        }

        #endregion
        #endregion

        #region DependencyProperties

        #region DefaultTheme

        public static readonly DependencyProperty DefaultThemeProperty =
    DependencyProperty.Register("DefaultTheme", typeof(DefaultThemesEnum), typeof(CodeBoxControl),
        new PropertyMetadata(DefaultThemesEnum.DarkTheme));

        public DefaultThemesEnum DefaultTheme
        {
            get { return (DefaultThemesEnum)GetValue(DefaultThemeProperty); }
            set { SetValue(DefaultThemeProperty, value); }
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

    }
}

