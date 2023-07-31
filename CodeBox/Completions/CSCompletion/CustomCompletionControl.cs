using CodeBox.Completions;
using CodeBox.Completions.CSCompletion.Snippets;
using Completions.CSCompletion;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common.Completions;

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
            InitializeControl(CompletionList.ListBox, Theme.CompletionBackground, Theme.CompletionForeground, Theme.CompletionBorder);
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
            InitializeBrushes();
            InitializeStyles();
            InitializeWindow();
            InitializeStandardCompletions();
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
            CompletionList.ListBox.SelectionChanged += SelectionChanged;
            SetCompletionWindowOffset();
        }

        private void InitializeStandardCompletions()
        {
                var data = CompletionList.CompletionData;
                var standard = CSharpStandardCompletions.GetStandard(TextArea);
                for (int i = 0; i < standard.Count; i++)
                    data.Add(standard[i]);
        }
        private void InitializeStyles()
        {
            //CompletionList.Style = FindResource("CompletionListStyle") as Style;
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

        /// <summary>
        /// Showing with duration
        /// </summary>
        public new void Show()
        {
            ICompletionData selected = CompletionList.SelectedItem;
            CompletionList.ListBox.ClearSelection();
            CompletionList.ListBox.SelectedItem = selected;
            base.Show();
        }


        #region Events

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            toolTip.IsOpen = false;
            ListBox list = CompletionList.ListBox;
            if (list.Items.Count == 0)
                this.Close();
            for (int i = 0; i < list.Items.Count; i++)
            {
                CSharpCompletion item = list.Items[i] as CSharpCompletion;
                item.SelectionColor = Theme.CompletionBackground;
            }
            CSharpCompletion currentItem = list.SelectedValue as CSharpCompletion;
            if (currentItem != null)
            {
                CSharpCompletion data = currentItem;
                data.SelectionColor = SelectionBrush;
                CreateToolTip(data);
            }
        }

        private void CreateToolTip(ICompletionData data)
        {
            toolTip = new ToolTip();
            toolTip.Style = FindResource("CompletionToolTipStyle") as Style;
            InitializeControl(toolTip, Theme.CompletionBackground, Theme.CompletionForeground, Theme.CompletionBorder);
            toolTip.PlacementTarget = this;
            toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            toolTip.Content = data.Description.ToString();
            toolTip.IsOpen = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            base.OnKeyDown(e);
            if (e.Key.ToString().Length > 1 && e.Key != Key.Up && e.Key != Key.Down)//for changing items and autocomplete keys
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
            DocumentLine currentLine = TextArea.Document.GetLineByNumber(line);
            string text = TextArea.Document.GetText(currentLine.Offset, currentLine.Length);
            return text;
        }

        private int GetWordStartOffset(int startOffset, string text)
        {
            int lineOffset = TextArea.Document.GetLineByOffset(startOffset).Offset;
            startOffset -= lineOffset;
            int i = startOffset - 1;
            while (i > -1 && !char.IsWhiteSpace(text[i]))
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
