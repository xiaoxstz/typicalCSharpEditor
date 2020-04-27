using System.Windows.Media;

namespace IDEThemes.Themes.CSharpThemes
{
    public class DarkTheme: CSharpThemeBase
    {
        public DarkTheme()
        {
            SetProperties();
        }
        private void SetProperties()
        {
            for(int i = Colors.Count - 1; i > 0; i--)
            {
                switch(Colors[i].Name)
                {
                    case "Default":
                        {
                            Background = (Brush)new BrushConverter().ConvertFromInvariantString(Colors[i].Background.ToString());
                            Foreground = (Brush)new BrushConverter().ConvertFromInvariantString(Colors[i].Foreground.ToString());
                        }
                        break;
                    case "LineNumbersForeground":
                        LineNumbersForeground = Colors[i];
                        break;
                    case "Selection":
                        {
                            Selection = (Brush)new BrushConverter().ConvertFromInvariantString(Colors[i].Background.ToString());
                        }

                        break;
                    case "Hyperlink":
                        Hyperlink = Colors[i];
                        break;
                    case "NonPrintableCharacter":
                        NonPrintabelCharacters = Colors[i];
                        break;
                }
            }
        }
    }
}
