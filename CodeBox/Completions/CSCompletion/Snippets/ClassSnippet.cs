using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
namespace CodeBox.Completions.CSCompletion.Snippets
{
    class ClassSnippet: CodeSnippet
    {
        private TextArea area { get; set; }
        public ClassSnippet(TextArea area): base(area)
        {
            this.area = area;
            Text = "class";
            Description = "Code snippet for class";
            InsertString = "class MyClass\n{\n}";
            SelectionStrings.Add("MyClass", 6);
            CaretPosition = 6;
        }

        public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            base.Complete(textArea, completionSegment, insertionRequestEventArgs);

        }

        protected override void Indent(TextArea area)
        {
            DocumentLine currentLine = area.Document.GetLineByNumber(area.Caret.Line + 1);
            area.IndentationStrategy.IndentLine(area.Document, currentLine);
            area.IndentationStrategy.IndentLine(area.Document, currentLine.NextLine);

        }

        protected override void EnterAction()
        {
            area.Caret.Line += 2;
        }
    }
}
