using System.Windows;
using System.Windows.Controls;

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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cntrl.SelectionOffset = 0;
            cntrl.SelectedTextLength = cntrl.Text.Length;
        }
    }
}                
