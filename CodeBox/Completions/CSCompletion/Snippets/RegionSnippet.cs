using ICSharpCode.AvalonEdit.Editing;
using Indents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox.Completions.CSCompletion.Snippets
{
    class RegionSnippet:CodeSnippet
    {
        public RegionSnippet(TextArea area) : base(area)
        {
            Text = "#region";
            Description = "Code snippet for #region";
            InsertString = $"#region MyRegion\n\n#endregion";
            SelectionStrings.Add("MyRegion", 8);
        }
    }
}
