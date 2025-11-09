using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ParamEditor.Themes;

namespace ParamEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string ThemeIcon => ThemeManager.IsDarkTheme ? "☀" : "🌙";

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

        private static readonly Regex _floatRegex = new(@"^[+-]?(\d+)?([.]?\d*)?$");
        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            var vm = textBox?.DataContext as ViewModels.ParameterViewModel;
            if (vm?.Type == "float")
            {
                var preview = textBox.Text.Insert(textBox.SelectionStart, e.Text);
                if (!_floatRegex.IsMatch(preview))
                {
                    e.Handled = true;
                    return;
                }
                if (vm.DecimalPlaces.HasValue && preview.Contains('.'))
                {
                    var index = preview.IndexOf('.');
                    var decimalPart = preview[(index + 1)..];
                    if (decimalPart.Length > vm.DecimalPlaces.Value)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is ViewModels.ParameterViewModel vm)
            {
                vm.NormalizeDecimal();
            }
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
