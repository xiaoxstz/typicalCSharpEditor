using CodeBox.Completions.CSCompletion.Snippets;
using Completions.CSCompletion;
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

namespace CodeBox.Completions.CSCompletion
{
    public class CodeSnippet: CSharpCompletion
    {
        public string InsertString { get; protected set; }
        public Dictionary<string, int> SelectionStrings { get; protected set; } = new Dictionary<string, int>();
        
        private string CurrentString { get; set; }
        private TextArea textArea { get; set; }
        public CodeSnippet(TextArea textArea):base(textArea.Document.Text, CompletionTypes.Snippet)
        {
            this.textArea = textArea;
            CodeBoxControl.TabAction = ChangeSelection;
        }

        private int StartOffset { get; set; }
        private int EndOffset { get; set; }

        private void SetStringsOffset()
        {
            foreach (KeyValuePair<string, int> s in SelectionStrings.ToArray())
            {
                SelectionStrings[s.Key] += StartOffset;
            }
        }

        private void ChangeSelection()
        {
            List<string> Keys = SelectionStrings.Keys.ToList();
            int currentIndex = Keys.IndexOf(CurrentString);
            int nextIndex = currentIndex + 1;
            if (nextIndex == SelectionStrings.Count)
                nextIndex = 0;
            CurrentString = Keys[nextIndex];
            Select();
            SetSelections();

        }

        private void SetSelections()
        {
            foreach (KeyValuePair<string, int> s in SelectionStrings.ToArray())
            {
                ColorizeSnippet snip = new ColorizeSnippet(s.Value, textArea);
                textArea.TextView.LineTransformers.Add(snip);
            }
        }

        private void Select()
        {
            int endOffset = SelectionStrings[CurrentString] + CurrentString.Length;
            textArea.Selection = Selection.Create(textArea, SelectionStrings[CurrentString], endOffset);
            textArea.Caret.Offset = endOffset;
        }

        public override void Complete(TextArea textArea, ISegment completionSegment,
    EventArgs insertionRequestEventArgs)
        {
            CodeBoxControl.IsSnippetCompletion = true;
            StartOffset = textArea.Caret.Offset - completionSegment.Length;
            EndOffset = StartOffset + InsertString.Length;
            SetStringsOffset();
            textArea.Document.Replace(completionSegment.Offset, completionSegment.Length,
              InsertString, OffsetChangeMappingType.CharacterReplace);
            ChangeSelection();
        }
    }
}
