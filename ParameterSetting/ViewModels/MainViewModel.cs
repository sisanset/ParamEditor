using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace ParamEditor.ViewModels
{
    internal class MainViewModel
    {
        public ObservableCollection<ParameterViewModel> Parameters { get; } = new();
        public ICommand SaveCommand { get; }
        public MainViewModel()
        {
            var schema = Models.SchemaLoader.Load("schema.yaml");
            foreach (var def in schema.Parameters)
                Parameters.Add(new ParameterViewModel(def));
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object? _)
        {
            var dict = new Dictionary<string, string?>();
            foreach (var p in Parameters)
                dict[p.Name] = p.Value;
            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(dict);
            File.WriteAllText("data.yaml", yaml);
        }


    }
}
