using Common.Enums;
using IDETHemes.Themes.Enums;

namespace IDEThemes.Themes.CSharpThemes
{
    public class BlueTheme: ThemeBase
    {
        public override string FilePath { get; set; } = "CSharpBlueTheme.xshd";
        public BlueTheme(Languages lang) : base(lang, DefaultThemesEnum.LightTheme)
        {
        }
    }
}
