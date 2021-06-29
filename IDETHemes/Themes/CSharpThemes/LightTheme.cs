using CodeBox.Enums;
using IDEThemes.Themes.CSharpThemes;
using System;
using IDETHemes.Themes.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEThemes.Themes.CSharpThemes
{
    public class LightTheme: ThemeBase
    {
        public override string FilePath { get; set; }
        public LightTheme(Languages lang) : base(lang, DefaultThemesEnum.LightTheme)
        {
        }
    }
}
