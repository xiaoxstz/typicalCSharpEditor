using CodeBox.Enums;
using IDETHemes.Themes.Enums;

namespace IDEThemes.Themes.CSharpThemes
{
    public class DarkTheme: CSharpThemeBase
    {
        public override string FilePath { get; set; }
        public DarkTheme(Languages lang) : base(lang, DefaultThemesEnum.DarkTheme)
        {
        }
    }
}
