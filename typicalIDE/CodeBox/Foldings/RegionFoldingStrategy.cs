using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            string t = document.Text;
            int tLength = t.Length;
            var regionMatches = Regex.Matches(t, $@"{REGION}((.|\n|\r)*){END_REGION}", RegexOptions.Singleline);
            for(int i = 0; i < regionMatches.Count; i++)
            {
                int startIndex = regionMatches[i].Index;
                int endIndex = startIndex + regionMatches[i].Length;
                char symbolAfterRegion = t[startIndex + REGION.Length];
                if (char.IsWhiteSpace(symbolAfterRegion))
                {
                    if ((endIndex == tLength) ||
                        (endIndex < tLength && char.IsWhiteSpace(t[endIndex])))// check last symbols of #endregion string
                    {
                        Match m = Regex.Match(regionMatches[i].Value, $@"{REGION}(.*?)\n");
                        string displayName = m.Groups[1].Value.Remove(0, 1);
                        newFoldings.Add(new NewFolding() { StartOffset = startIndex, EndOffset = endIndex, Name = displayName});
                    }
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}
