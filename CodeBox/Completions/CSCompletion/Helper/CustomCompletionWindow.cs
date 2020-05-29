
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Completions.CSCompletion
{
    /// <summary>
    /// The code completion window.
    /// </summary>
    public class CustomCompletionWindow : CompletionWindowBase
    {
        #region CompletionWindow
        private CompletionList completionList = new CompletionList();
        protected ToolTip toolTip { get; set; } = new ToolTip();
        /// <summary>
        /// Gets the completion list used in this completion window.
        /// </summary>
        public CompletionList CompletionList
        {
            get { return completionList; }
        }


        public CustomCompletionWindow(TextArea area):base(area)
        {
            this.CloseAutomatically = true;
            this.SizeToContent = SizeToContent.Height;
            this.Content = completionList;
            this.MaxHeight = 300;
            this.Width = 175;
            this.MinHeight = 15;
            this.MinWidth = 30;
            AttachEvents();
        }

        #region ToolTip handling


        protected override void OnClosed(EventArgs e)
        {
            if (toolTip != null)
                toolTip.IsOpen = false;
            toolTip = null;
            base.OnClosed(e);
        }
        #endregion

        public new void Close()
        {
            if (Dispatcher.CheckAccess())
                base.Close();
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(base.Close));
        }

        void completionList_InsertionRequested(object sender, EventArgs e)
        {
            Close();
            // The window must close before Complete() is called.
            // If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
            var item = completionList.SelectedItem;
            if (item != null)
                item.Complete(TextArea, new AnchorSegment(this.TextArea.Document, this.StartOffset, this.EndOffset - this.StartOffset), e);

        }

        void AttachEvents()
        {
            this.CompletionList.InsertionRequested += completionList_InsertionRequested;
            this.TextArea.Caret.PositionChanged += CaretPositionChanged;
            this.TextArea.MouseWheel += textArea_MouseWheel;
            this.TextArea.PreviewTextInput += textArea_PreviewTextInput;
        }

        /// <inheritdoc/>
        protected override void DetachEvents()
        {
            this.CompletionList.InsertionRequested -= completionList_InsertionRequested;
            this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
            this.TextArea.MouseWheel -= textArea_MouseWheel;
            this.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
            base.DetachEvents();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsLoaded)
            {
                base.OnKeyDown(e);
                if ((CompletionList.ListBox.SelectedIndex > -1 || e.Key != Key.Enter) && !e.Handled)
                {
                    completionList.HandleKey(e);
                }

            }
        }
        void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                                       new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                                       PreviewMouseWheelEvent, MouseWheelEvent,
                                       new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        UIElement GetScrollEventTarget()
        {
            if (completionList == null)
                return this;
            return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
        }

        /// <summary>
        /// Gets/Sets whether the completion window should close automatically.
        /// The default value is true.
        /// </summary>
        public bool CloseAutomatically { get; set; }

        /// <inheritdoc/>
        protected override bool CloseOnFocusLost
        {
            get { return this.CloseAutomatically; }
        }

        /// <summary>
        /// When this flag is set, code completion closes if the caret moves to the
        /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
        /// but not in dot-completion.
        /// Has no effect if CloseAutomatically is false.
        /// </summary>
        public bool CloseWhenCaretAtBeginning { get; set; }

        void CaretPositionChanged(object sender, EventArgs e)
        {
            int offset = this.TextArea.Caret.Offset;
            if (offset == this.StartOffset)
            {
                if (CloseAutomatically && CloseWhenCaretAtBeginning)
                {
                    Close();
                }
                else
                {
                    completionList.SelectItem(string.Empty);
                }
                return;
            }
            if (offset < this.StartOffset || offset > this.EndOffset)
            {
                if (CloseAutomatically)
                {
                    Close();
                }
            }
            else
            {
                TextDocument document = this.TextArea.Document;
                if (document != null)
                {
                    completionList.SelectItem(document.GetText(this.StartOffset, offset - this.StartOffset));
                }
            }
        }

        #endregion
    }
}