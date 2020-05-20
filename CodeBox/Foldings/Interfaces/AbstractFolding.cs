using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;

namespace Foldings
{
    internal abstract class AbstractFolding
    {
        public virtual void UpdateFoldings(List<NewFolding> foldings, TextDocument document)
        {
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document);
            foldings.AddRange(newFoldings);
        }

        public abstract IEnumerable<NewFolding> CreateNewFoldings(ITextSource document);
    }
}
