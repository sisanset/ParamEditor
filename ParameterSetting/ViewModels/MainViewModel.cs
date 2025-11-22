using ParamEditor.Models;
using ParamEditor.Themes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using YamlDotNet.Serialization.NamingConventions;

namespace ParamEditor.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ParameterGroupViewModel> Groups { get; } = new();
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand LoadCommand { get; }
        private readonly string schemaPath = "schema.yaml";
        private readonly SchemaRoot schemaRoot;
        public string DataPath
        {
            get => Properties.Settings.Default.LastParamFilePath ?? "data.json";
            set
            {
                Properties.Settings.Default.LastParamFilePath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            ThemeManager.ThemeChanged += (_, _) =>
            {
                foreach (var group in Groups)
                {
                    foreach (var param in group.Parameters)
                    {
                        param.RefreshValidation();
                    }
                }
            };
            schemaRoot = SchemaLoader.Load(schemaPath);
            Load_(DataPath);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            LoadCommand = new RelayCommand(Load);
        }
        private void SaveAs(object? _)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "名前を付けて保存",
                FileName = "data",
                DefaultExt = ".json",
                Filter = "JSONファイル (*.json)|*.json|YAMLファイル (*.yaml;*.yml)|*.yaml;*.yml"
            };
            if (dlg.ShowDialog() == true)
            {
                if (Save_(dlg.FileName))
                { DataPath = dlg.FileName; }
            }
        }
        private bool Save_(string path)
        {
            var invalids = Groups.SelectMany(g => g.Parameters).Where(p => !p.IsValid).ToList();
            if (invalids.Count != 0)
            {
                var msg = "以下のパラメータの値が不正です。修正してください。\n" +
                    string.Join("\n", invalids.Select(p => $"- {p.Name}: {p.Value}"));
                MessageBox.Show(msg, "保存できません", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            var dict = new Dictionary<string, string?>();
            foreach (var g in Groups)
            {
                foreach (var p in g.Parameters)
                    dict[p.Name] = p.Value;
            }
            ParameterValueLoader.SaveParameters(path, dict);
            return true;
        }
        private void Save(object? _) => Save_(DataPath);
        private void Load(object? _)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "パラメータファイルを開く",
                FileName = "data",
                DefaultExt = ".json",
                Filter = "JSONファイル (*.json)|*.json|YAMLファイル (*.yaml;*.yml)|*.yaml;*.yml"
            };
            if (dlg.ShowDialog() == false)
            { return; }

            Load_(dlg.FileName);
            DataPath = dlg.FileName;
        }

        private void Load_(string path)
        {
            var values = ParameterValueLoader.LoadParameters(path, schemaRoot);
            var grouped = schemaRoot.Parameters.GroupBy(p => p.Group);
            Groups.Clear();
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
            foreach (var group in Groups)
            {
                foreach (var param in group.Parameters)
                {
                    if (string.IsNullOrEmpty(param.Relation)) { continue; }
                    var relatedParam = Groups
                        .SelectMany(g => g.Parameters)
                        .Where(p => p.Relation == param.Relation);
                    foreach (var p in relatedParam)
                    {
                        param.AddRelation(p);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
