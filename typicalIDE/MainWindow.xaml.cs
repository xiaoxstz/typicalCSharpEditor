using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IDEThemes.Themes.Default;
using IDEThemes.Themes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace typicalIDE
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var ass = Assembly.GetExecutingAssembly();
            using (var stream = ass.GetManifestResourceStream(a))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            textEditor.TextArea.TextView
            colors.SetTheme(textEditor.SyntaxHighlighting);

            



            var b = textEditor.SyntaxHighlighting.Properties;
            foreach(var prop in b)
            {
                if(prop.Key == "Background")
                {
                    textEditor.Background =(Brush)new BrushConverter().ConvertFromInvariantString(prop.Value); 
                }
            }

        }
    }
}                
