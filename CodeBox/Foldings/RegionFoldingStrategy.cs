using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Foldings
{
    class RegionFoldingStrategy: AbstractFolding
    {
        private const string REGION = "#region";
        private const string END_REGION = "#endregion";

        public override IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();
            string t = document.Text;
            int tLength = t.Length;
            var pattern = new Regex($@"{REGION}((.|\n|\r)*){END_REGION}", RegexOptions.Multiline);
            List<Match> regionMatches = new List<Match>();
            MatchCollection temp = pattern.Matches(t);
            while(temp.Count > 0)
            {
                regionMatches.Add(temp[0]);
                temp = pattern.Matches(t, temp[0].Groups[1].Index);
            }


            for (int i = 0; i < regionMatches.Count; i++)
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
                        if (m.Groups[1].Value.Length > 0)
                        {
                            string displayName = m.Groups[1].Value.Remove(0, 1);
                            newFoldings.Add(new NewFolding() { StartOffset = startIndex, EndOffset = endIndex, Name = displayName });
                        }
                    }
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}
