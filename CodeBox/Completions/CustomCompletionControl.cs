using CodeBox.Completions;
using Completions.CSharpCompletion;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Completions
{
    public class CustomCompletionControl: CustomCompletionWindow, INotifyPropertyChanged
    {
        /// <summary>
        /// Global theme for completion windows.
        /// </summary>
        public static CompletionTheme Theme { get; set; } = new CompletionTheme();
        #region Brushes

        #region SelectionBrush
        private Brush selectionBrush;
        public Brush SelectionBrush
        {
            get { return selectionBrush ?? Brushes.Red; }
            set
            {
                if (value != null)
                    selectionBrush = value;
                else
                    selectionBrush = Brushes.Red;
                OnPropertyChanged("SelectionBrush");
            }
        }
        #endregion

        #region ForegroundBrush

        private Brush foregroundBrush;
        public Brush ForegroundBrush
        {
            get { return foregroundBrush ?? Brushes.White; }
            set
            {
                if (value != null)
                    foregroundBrush = value;
                else
                    foregroundBrush = Brushes.White;
                OnPropertyChanged("ForegroundBrush");
            }
        }

        #endregion

        #region BackgroundBrush

        private readonly Brush backgroundDef = (Brush)new BrushConverter().ConvertFrom("#1b1c1b");

        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get { return backgroundBrush ?? backgroundDef; }
            set
            {
                if (value != null)
                    backgroundBrush = value;
                else
                    backgroundBrush = backgroundDef;
                OnPropertyChanged("BackgroundBrush");
            }
        }

        #endregion

        #region BorderBrush

        private Brush borderBrush;
        public new Brush BorderBrush
        {
            get { return borderBrush ?? Brushes.Gray; }
            set
            {
                if (value != null)
                    borderBrush = value;
                else
                    borderBrush = Brushes.Gray;
                OnPropertyChanged("BorderBrush");
            }
        }

        #endregion

        #endregion
        #region Initializing

        #region Constructors

        /// <summary>
        /// Initialize completion window with default colors
        /// </summary>
        /// <param name="area"></param>
        public CustomCompletionControl(TextArea area) : base(area)
        {
            Initialize();
        }

        /// <summary>
        /// Initializing colors from editor style
        /// </summary>
        /// <param name="editor"></param>

        public CustomCompletionControl(TextEditor editor) : base(editor.TextArea)
        {
            Initialize();
            InitializeControl(CompletionList.ListBox, editor.Background, editor.Foreground);
            InitializeControl(toolTip, Theme.CompletionBackground, Theme.CompletionForeground, Theme.CompletionBorder);
        }


        /// <summary>
        /// Setting style by colors
        /// </summary>
        public CustomCompletionControl(TextArea area, Brush background = null, Brush foreground = null, 
            Brush borderBrush=null, Thickness borderThickness=default(Thickness)) : base(area)
        {
            Initialize();
            InitializeControl(CompletionList.ListBox, background, foreground, borderBrush, borderThickness);
            InitializeControl(toolTip, background, foreground, borderBrush, borderThickness);
        }
        #endregion


        private void Initialize()
        {
            InitializeStyles();
            InitializeWindow();
            InitializeStandardCompletions();
            InitializeBrushes();
        }

        

        private void InitializeControl(Control c, Brush background = null, Brush foreground = null,
            Brush borderBrush = null, Thickness borderThickness = default(Thickness))
        {
            c.Background = background ?? c.Background;
            c.Foreground = foreground ?? c.Foreground;
            c.BorderBrush = borderBrush ?? c.BorderBrush;
            c.BorderThickness = c.BorderThickness;
        }
        private void InitializeWindow()
        {
            CompletionList.ListBox.SelectionChanged += Changed;
            SetCompletionWindowOffset();

        }
        private void InitializeStandardCompletions()
        {
                var data = CompletionList.CompletionData;
                var standard = CSharpStandardCompletoins.GetKeyWords();
                for (int i = 0; i < standard.Count; i++)
                    data.Add(standard[i]);
        }
        private void InitializeStyles()
        {
            CompletionList.Style = FindResource("CompletionListStyle") as Style;
            toolTip.Style = FindResource("CompletionToolTipStyle") as Style;
            Style = FindResource("CompletionWindowStyle") as Style;
        }
        private void InitializeBrushes()
        {
            BackgroundBrush = Theme.CompletionBackground;
            BorderBrush = Theme.CompletionBorder;
            Foreground = Theme.CompletionForeground;
            SelectionBrush = Theme.CompletionSelectionBrush;
        }

        #endregion
        #region Events
        private void Changed(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list.Items.Count == 0)
                this.Close();
            for (int i = 0; i < list.Items.Count; i++)
            {
                var temp = list.Items[i] as CSharpCompletion.CSharpCompletion;
                temp.SelectionColor = Brushes.Transparent;
            }
            var cur = list.SelectedValue;
            if (cur != null)
            {
                var temp = cur as CSharpCompletion.CSharpCompletion;
                temp.SelectionColor = SelectionBrush;
                toolTip.Content = "AAAAAAAAAAAAA";
                toolTip.IsOpen = true;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Space)
                CompletionList.RequestInsertion(e);
            if (e.Key.ToString().Length > 1 && e.Key != Key.Up && e.Key != Key.Down)
                Close();
        }    
        #endregion
        #region WordOffset
        private void SetCompletionWindowOffset()
        {
            string text = GetCurrentLineText();
            StartOffset = GetWordStartOffset(StartOffset, text);
            EndOffset = GetWordEndOffset(StartOffset, text);
        }


        private string GetCurrentLineText()
        {
            int line = TextArea.Caret.Line;
            int column = TextArea.Caret.Column;
            DocumentLine currentLine = TextArea.Document.GetLineByNumber(line);
            string text = TextArea.Document.GetText(currentLine.Offset, currentLine.Length);
            return text;
        }

        private int GetWordStartOffset(int startOffset, string text)
        {
            int lineOffset = TextArea.Document.GetLineByOffset(startOffset).Offset;
            startOffset -= lineOffset;
            int i = startOffset - 1;
            while (i > -1 && char.IsLetterOrDigit(text[i]))
                i--;
            return i + 1 + lineOffset;
        }

        private int GetWordEndOffset(int startOffset, string text)
        {
            int lineOffset = TextArea.Document.GetLineByOffset(startOffset).Offset;
            int endOffset = startOffset - lineOffset;
            for (; endOffset < text.Length
                && !char.IsWhiteSpace(text[endOffset]);
                endOffset++) ;
            return endOffset + lineOffset;
        }
        #endregion

        #region OnPropertyChanged


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
