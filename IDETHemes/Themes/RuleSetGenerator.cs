using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDETHemes.Themes
{
    internal static class RuleSetGenerator
    {
        internal static string GetRuleSetRegex(string _keywords)
        {
            //  \\b(?> this | base)\\b
            string[] keywords = _keywords.Split(' ');
            string regString = @"\b(?>";
            for (int i = 0; i < keywords.Length; i++)
                regString += keywords[i] + "|";
            regString = regString.Remove(regString.Length - 1, 1);
            regString += @")\b";
            return regString;
        }
    }
}
