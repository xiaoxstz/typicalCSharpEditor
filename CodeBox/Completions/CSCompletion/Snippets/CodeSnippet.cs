using CodeBox.Completions.CSCompletion.Snippets;
using Completions.CSCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CodeBox.Completions.CSCompletion
{
    public class CodeSnippet: CSharpCompletion
    {
        public string InsertString { get; protected set; }
        public Dictionary<string, int> SelectionStrings { get; protected set; } = new Dictionary<string, int>();

        protected int CaretPosition { get; set; } = -1;
        
        private int CurrentIndex { get; set; }
        private TextArea textArea { get; set; }
        public CodeSnippet(TextArea textArea):base(textArea.Document.Text, CompletionTypes.Snippet)
        {
            this.textArea = textArea;
        }

        private int StartOffset { get; set; }

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
            if (Keys.Count > 0)
            {
                int nextIndex = CurrentIndex + 1;
                if (nextIndex == SelectionStrings.Count)
                    nextIndex = 0;
                CurrentIndex = nextIndex;
                SetSelections();
            }
        }

        private void SetSelections()
        {
            foreach (KeyValuePair<string, int> s in SelectionStrings.ToArray())
            {
                ColorizeSnippet snip = new ColorizeSnippet(s.Value, textArea);
                textArea.TextView.LineTransformers.Add(snip);
            }
        }
        

        public override void Complete(TextArea textArea, ISegment completionSegment,
    EventArgs insertionRequestEventArgs)
        {
            CodeBoxControl.IsSnippetCompletion = true;
            StartOffset = textArea.Caret.Offset - completionSegment.Length;
            SetStringsOffset();
            textArea.Document.Replace(completionSegment.Offset, completionSegment.Length,
              InsertString, OffsetChangeMappingType.CharacterReplace);
            if (CaretPosition != -1)
                textArea.Caret.Offset = CaretPosition + StartOffset;
            Indent(textArea);
            ChangeSelection();
            CodeBoxControl.EnterAction = EnterAction;
            CodeBoxControl.TabAction = ChangeSelection;
        }

        protected virtual void Indent(TextArea area)
        {
            area.IndentationStrategy.IndentLine(area.Document, area.Document.GetLineByNumber(area.Caret.Line));
        }

        public static void Clear(TextArea area)
        {
            CodeBoxControl.IsSnippetCompletion = false;
            CodeBoxControl.TabAction = null;
            IList<IVisualLineTransformer> trans = area.TextView.LineTransformers;
            for (int i = 0; i < trans.Count; i++)
                if (trans[i].GetType() == typeof(ColorizeSnippet))
                    trans.Remove(trans[i]);
        }

        protected virtual void EnterAction() { }

    }
}