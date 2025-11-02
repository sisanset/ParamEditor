using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using ParamEditor.Models;

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
