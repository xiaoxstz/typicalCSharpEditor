using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CodeBox.Completions.CSCompletion.Snippets
{
    class ColorizeSnippet : DocumentColorizingTransformer
    {
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        delegate bool IsCorrectChar(char c);
        private IsCorrectChar CheckChar { get; set; }

        private Brush backgroundBrush;
        private Brush BackgroundBrush {
            get => backgroundBrush;
            set {
                if (value != null)
                    backgroundBrush = value;
                else
                    backgroundBrush = StandardBGBrush;
            }
        }

        private Brush StandardBGBrush { get; set; } = (Brush)new BrushConverter().ConvertFromInvariantString("#3a506a");

        private TextArea textArea { get; set; }
        public ColorizeSnippet(int startOffset, TextArea textArea, Brush backgroundBrush = null, bool isAllLine = true)
        {
            StartOffset = startOffset;
            this.textArea = textArea;
            BackgroundBrush = backgroundBrush;
            if (isAllLine)
                CheckChar = (c) => c != '\n';
            else
                CheckChar = char.IsWhiteSpace;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (textArea.Document.TextLength == 0 || StartOffset > line.EndOffset || StartOffset < line.Offset)
            {
                Deactivate();
                return;
            }
            SetEndOffset(line);
            ChangeVisualElements(StartOffset - line.Offset, EndOffset, (VisualLineElement element) =>
            {
                element.TextRunProperties.SetBackgroundBrush(BackgroundBrush);
            });
            textArea.Selection = Selection.Create(textArea, StartOffset, EndOffset);
        }


        private void SetEndOffset(DocumentLine line)
        {
            int i = StartOffset - line.Offset;
            string text = CurrentContext.Document.GetText(line);
            while (i < text.Length && CheckChar(text[i]))
                i++;
            EndOffset = i + line.Offset;
        }

        private void Deactivate()
        {
            textArea.TextView.LineTransformers.Remove(this);
        }
    }

}

