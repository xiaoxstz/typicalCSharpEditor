using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using IDEThemes.Themes.Interfaces;

namespace IDEThemes.Themes
{
    public abstract class AppTheme
    {
        public IThemeColors ThemeColors
        {
            get => ThemeColors;
            set
            {
                if (value != null)
                    ThemeColors = value;
            }
        }

        public Brush Background { get; private set; }
        public Brush LineNumbersForeground { get; private set; }
        public Brush AppBackground { get; private set; }

    }
}
