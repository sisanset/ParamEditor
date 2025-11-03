using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using ParamEditor.Models;
using ParamEditor.Themes;
using System.Windows;

namespace ParamEditor.ViewModels
{
    internal class MainViewModel
    {
        public ObservableCollection<ParameterGroupViewModel> Groups { get; } = new();
        public ICommand SaveCommand { get; }
        private readonly string schemaPath = "schema.yaml";
        private readonly string dataPath = "data.yaml";


        public MainViewModel()
        {
            ThemeManager.ThemeChanged += (_, _) =>
            {
              foreach(var group in Groups)
                {
                    foreach(var param in group.Parameters)
                    {
                        param.RefreshValidation();
                    }
                }
            };
            var schema = SchemaLoader.Load(schemaPath);
            var values = ParameterValueLoader.LoadValues(dataPath);
            var grouped = schema.Parameters.GroupBy(p => p.Group);
            foreach (var group in grouped)
            {
                var groupVM = new ParameterGroupViewModel(group.Key);
                foreach (var def in group)
                {
                    var vm = new ParameterViewModel(def);
                    if (values.TryGetValue(def.Name, out var val))
                        vm.Value = val;
                    groupVM.Parameters.Add(vm);
                }
                Groups.Add(groupVM);
            }
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object? _)
        {
            var invalids=Groups.SelectMany(g=>g.Parameters).Where(p => !p.IsValid).ToList();
            if (invalids.Any())
            {
                var msg = "以下のパラメータの値が不正です。修正してください。\n" +
                    string.Join("\n", invalids.Select(p => $"- {p.Name}: {p.Value}"));
                MessageBox.Show(msg, "保存できません", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var dict = new Dictionary<string, string?>();
            foreach (var g in Groups)
            {
                foreach (var p in g.Parameters)
                    dict[p.Name] = p.Value;
            }
            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(dict);
            File.WriteAllText(dataPath, yaml);
        }


    }
}
