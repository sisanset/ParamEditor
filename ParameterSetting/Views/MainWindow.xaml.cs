using System.Windows;
using ParamEditor.Themes;

namespace ParamEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ThemeIcon=>ThemeManager.IsDarkTheme? "☀" : "🌙";

        public MainWindow()
        {
            InitializeComponent();
            ThemeToggleButton.Content = ThemeIcon;
        }
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ToggleTheme();
            ThemeToggleButton.Content = ThemeIcon;
        }
    }
}
