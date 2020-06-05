using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;

namespace CodeBox.Completions.CSCompletion.Snippets
{
    class RegionSnippet:CodeSnippet
    {
        public RegionSnippet(TextArea area) : base(area)
        {
            Text = "#region";
            Description = "Code snippet for #region";
            InsertString = $"#region MyRegion\n#endregion";
            SelectionStrings.Add("MyRegion", 8);
        }
        public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            base.Complete(textArea, completionSegment, insertionRequestEventArgs);
            CodeBoxControl.EnterAction = null;
        }
    }
}
