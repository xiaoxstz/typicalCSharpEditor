using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.Generic;

namespace Completions.CSCompletion
{
    
    internal sealed class CSharpStandardCompletions
    {
        private static string keywords = "abstract delegate internal partial public readonly ref sealed static";
        internal static IList<ICompletionData> GetKeyWords()
        {
            IList<ICompletionData> list = new List<ICompletionData>();
            string[] splited = keywords.Split(' ');
            for(int i = 0; i < splited.Length; i++)
               list.Add(new CSharpCompletion(splited[i], CompletionTypes.Keyword));
            return list;
        }

        private static string snippets = "#region";
        internal static IList<ICompletionData> GetSnippets()
        {
            IList<ICompletionData> list = new List<ICompletionData>();
            string[] splited = snippets.Split(' ');
            for (int i = 0; i < splited.Length; i++)
                list.Add(new CSharpCompletion(splited[i], CompletionTypes.Snippet));
            return list;
        }
    }
}
