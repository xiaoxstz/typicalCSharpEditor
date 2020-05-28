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
        private int StartOffset { get; set; }
        private int EndOffset { get; set; }

        delegate bool IsCorrectChar(char c);
        private IsCorrectChar CheckChar { get; set; }

        private TextArea textArea { get; set; }
        public ColorizeSnippet(int startOffset, TextArea textArea, bool isAllLine = true)
        {
            StartOffset = startOffset;
            this.textArea = textArea;
            if (isAllLine)
                CheckChar = (c) => c != '\n';
            else
                CheckChar = char.IsWhiteSpace;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            SetEndOffset(line);
            if (EndOffset <= StartOffset || StartOffset < line.Offset)
            {
                DeactivateSnippet();
                return;
            }
            int k = EndOffset;
            while (k > StartOffset)
            {
                base.ChangeLinePart(
                    StartOffset,
                    EndOffset,
                    (VisualLineElement element) =>
                    {
                        element.TextRunProperties.SetBackgroundBrush(Brushes.Green);
                    });
                k--;
            }
        }

        private void SetEndOffset(DocumentLine line)
        {
            int i = StartOffset;
            string text = CurrentContext.Document.GetText(line);
            while (i < text.Length && CheckChar(text[i]))
                i++;
            EndOffset = i;
        }

        private void DeactivateSnippet()
        {
            textArea.TextView.LineTransformers.Clear();
        }
    }

}

