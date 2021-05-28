using CodeBox.Enums;
using IDETHemes.Themes.Enums;

namespace IDEThemes.Themes.CSharpThemes
{
    public class BlueTheme: CSharpThemeBase
    {
        public override string FilePath { get; set; } = "CSharpBlueTheme.xshd";
        public BlueTheme(Languages lang) : base(lang, DefaultThemesEnum.LightTheme)
        {
        }
    }
}
