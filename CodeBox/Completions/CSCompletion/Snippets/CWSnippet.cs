using ICSharpCode.AvalonEdit.Editing;

namespace CodeBox.Completions.CSCompletion.Snippets
{
    class CWSnippet: CodeSnippet
    {
        public CWSnippet(TextArea area):base(area)
        {
            Text = "cw";
            Description = "Code snippet for Console.WriteLine();";
            InsertString = $"Console.WriteLine();";
            CaretPosition = 18;
        }
    }
}
