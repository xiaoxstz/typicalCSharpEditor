using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace typicalIDE.CodeBox.Foldings
{
    class RegionFoldingStrategy
    {
        private const string REGION = "#region";
        private const string END_REGION = "#endregion";
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            var splited = document.Text.Split(' ', '\r', '\t', '\n');
            splited = splited.Where(el => el.Length > 0).ToArray();
            int chCounter = 0;
            for(int i = 0; i < splited.Length; i++)
            {
                chCounter += splited[i].Length + 1;
                var text = splited[i].Replace("\t", "");
                text = text.Replace("\r", "").Replace("\n", "");
                if (text == REGION)
                {
                    startOffsets.Push(chCounter);
                }
                else if(text == END_REGION && startOffsets.Count > 0)
                {
                    int startOffset = chCounter;
                    
                    newFoldings.Add(new NewFolding(startOffset, i + 1));
                }
                else if (splited[i] == "\n" || splited[i] == "\r")
                {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}
