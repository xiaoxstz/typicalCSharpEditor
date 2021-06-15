﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            DataContext = new MVM();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cbc.DefaultTheme = IDETHemes.Themes.Enums.DefaultThemesEnum.LightTheme;
        }
    }
}                
