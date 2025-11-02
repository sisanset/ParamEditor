using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ParamEditor.Models;
using System.IO;

namespace ParamEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SchemaRoot _schema;
        private Dictionary<string, object?> _currentValues = new();

        public MainWindow()
        {
            InitializeComponent();

            _schema = SchemaLoader.Load("schema.yaml");
            BuildEditorUI(_schema);
        }

        private void BuildEditorUI(SchemaRoot schema)
        {
            foreach (var p in schema.Parameters)
            {
                var label = new TextBlock
                {
                    Text = $"{p.Name} ({p.Description})",
                    Margin = new Thickness(0, 10, 0, 5),
                    FontWeight = FontWeights.Bold
                };
                EditorPanel.Children.Add(label);

                FrameworkElement input;

                switch (p.Type)
                {
                    case "float":
                    case "int":
                        var box = new TextBox { Width = 200 };
                        box.TextChanged += (s, e) =>
                        {
                            if (double.TryParse(box.Text, out double val))
                            {
                                if (p.Range != null &&
                                    (val < p.Range[0] || val > p.Range[1]))
                                    box.Background = System.Windows.Media.Brushes.LightCoral;
                                else
                                    box.ClearValue(TextBox.BackgroundProperty);
                            }
                            else
                                box.Background = System.Windows.Media.Brushes.LightCoral;

                            _currentValues[p.Name] = box.Text;
                        };
                        input = box;
                        break;

                    case "bool":
                        var check = new CheckBox();
                        check.Checked += (s, e) => _currentValues[p.Name] = true;
                        check.Unchecked += (s, e) => _currentValues[p.Name] = false;
                        input = check;
                        break;

                    case "enum":
                        var combo = new ComboBox { Width = 200, ItemsSource = p.Values };
                        combo.SelectionChanged += (s, e) => _currentValues[p.Name] = combo.SelectedItem;
                        input = combo;
                        break;

                    case "ip":
                        var ipBox = new TextBox { Width = 200 };
                        ipBox.TextChanged += (s, e) =>
                        {
                            string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9]{1,2})\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9]{1,2})$";
                            if (Regex.IsMatch(ipBox.Text, pattern))
                                ipBox.ClearValue(TextBox.BackgroundProperty);
                            else
                                ipBox.Background = System.Windows.Media.Brushes.LightCoral;
                            _currentValues[p.Name] = ipBox.Text;
                        };
                        input = ipBox;
                        break;

                    default:
                        input = new TextBox { Width = 200 };
                        break;
                }

                EditorPanel.Children.Add(input);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(_currentValues);
            File.WriteAllText("data.yaml", yaml);
            MessageBox.Show("保存しました。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
