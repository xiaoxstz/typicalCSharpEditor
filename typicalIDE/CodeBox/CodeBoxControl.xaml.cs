using ICSharpCode.AvalonEdit.Folding;
using IDEThemes.Themes.CSharpThemes;
using IDEThemes.Themes.Interfaces;
using IDETHemes.Themes.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using typicalIDE.CodeBox.Enums;
using typicalIDE.CodeBox.Folding;

namespace typicalIDE.CodeBox
{
    public partial class CodeBoxControl : UserControl
    {
        BraceFoldingStrategy braceFolding { get; set; } = new BraceFoldingStrategy();
        private FoldingManager foldingManager { get; set; }

        public CodeBoxControl()
        {
            InitializeComponent();
            foldingManager = FoldingManager.Install(textEditor.TextArea);
            Theme.SetTheme(textEditor);
        }
        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            braceFolding.UpdateFoldings(foldingManager, textEditor.Document);  
        }

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

