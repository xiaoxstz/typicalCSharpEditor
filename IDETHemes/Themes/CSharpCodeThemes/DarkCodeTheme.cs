using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using IDETHemes.Themes.Helper;

namespace IDEThemes.Themes.CSharpCodeThemes
{
    internal class DarkCodeTheme: CSharpThemeBase
    {
        public DarkCodeTheme()
        {
            SetColors();
        }

        private void SetColors()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string xmlCSharpFile = assembly.GetManifestResourceNames()
                .First(fp => fp.Contains("CSharpDarkThemeColors.xshd"));
            Colors = GetColorsByXml(xmlCSharpFile, assembly); 
        }

        private const string BACKGROUND_COLOR = "#1b1c1b";

        private void SetProperties()
        {
            foreach (KeyValuePair<string, HighlightingColor> prop in Properties)
            {
                switch(prop.Key)
                {
                    case "Default":
                        Properties[prop.Key] = new HighlightingColor
                        { Background = new CustomizedBrush(Color.FromRgb(30, 30, 30)),
                            Foreground = new CustomizedBrush(System.Drawing.Color.White)
                        };
                        break;
                }
            }
        }
    }
}
