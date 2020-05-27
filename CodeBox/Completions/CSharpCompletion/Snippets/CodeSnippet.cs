using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CodeBox.Completions.CSharpCompletion.Snippets
{
    class CodeSnippet
    {
        public string InsertString { get; set; }

        public IList<string> SelectionStrings { get; set; } = new List<string>();
        private TextArea textArea { get; set; }
        public CodeSnippet(TextArea textArea)
        {
            this.textArea = textArea;
        }

        public void Insert()
        {
            textArea.Document.Insert(textArea.Caret.Offset, InsertString);
            ColorizeAvalonEdit c = new ColorizeAvalonEdit();
            textArea.TextView.LineTransformers.Add(new ColorizeAvalonEdit());

        }



        public class ColorizeAvalonEdit : DocumentColorizingTransformer
        {


            protected override void ColorizeLine(DocumentLine line)
            {
                int lineStartOffset = line.Offset;
                string text = CurrentContext.Document.GetText(line);
                int start = 0;
                int index;
                while ((index = text.IndexOf("AvalonEdit", start)) >= 0)
                {
                    base.ChangeLinePart(
                        0, // startOffset
                        5, // endOffset
                        (VisualLineElement element) => {
                    // This lambda gets called once for every VisualLineElement
                    // between the specified offsets.
                    Typeface tf = element.TextRunProperties.Typeface;
                    // Replace the typeface with a modified version of
                    // the same typeface
                    element.TextRunProperties.SetTypeface(new Typeface(
                                tf.FontFamily,
                                FontStyles.Italic,
                                FontWeights.Bold,
                                tf.Stretch
                            ));
                        });
                    start = index + 1; // search for next occurrence
                }
            }
        }

    }

  
}
