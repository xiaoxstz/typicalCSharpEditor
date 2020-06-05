using CodeBox.Completions.CSCompletion.Snippets;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using System.Collections.Generic;
using System.Linq;

namespace Completions.CSCompletion
{
    
    internal sealed class CSharpStandardCompletions
    {
        internal static IList<ICompletionData> GetStandard(TextArea area)
        {
            List<ICompletionData> list = GetKeyWords().ToList();
            list.AddRange(GetSnippets(area));
            return list;
        }

        private static string keywords = "abstract delegate internal partial public readonly ref sealed static";
        internal static IList<ICompletionData> GetKeyWords()
        {
            IList<ICompletionData> list = new List<ICompletionData>();
            string[] splited = keywords.Split(' ');
            for(int i = 0; i < splited.Length; i++)
               list.Add(new CSharpCompletion(splited[i], CompletionTypes.Keyword));
            return list;
        }

        internal static IList<ICompletionData> GetSnippets(TextArea area)
        {
            IList<ICompletionData> snippets = new List<ICompletionData>()
            {
                new RegionSnippet(area), new CWSnippet(area), new ClassSnippet(area)
            };
            return snippets;
        }
    }
}
