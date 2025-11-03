using ParamEditor.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ParamEditor.ViewModels
{
    public class ParameterViewModel : INotifyPropertyChanged
    {
        private string? _value;
        private bool _isValid = true;
        public ParameterDefinition Definition { get; }
        public string Name => Definition.Name;
        public string Type => Definition.Type;
        public string Description => Definition.Description;
        public string View => $"{Name}({Description})";
        public string? Unit => Definition.Unit;
        public string? Value
        {
            get => _value; set
            {
                if (_value != value)
                {
                    _value = value;
                    Validate();
                    OnPropertyChanged();
                }
            }
        }
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }
        public string ToolTipText
        {
            get
            {
                var text = Description ?? string.Empty;
                if (Type == "int" || Type == "float")
                {
                    var range = string.Empty;
                    if (Definition.Range?.Length == 2)
                    {
                        range = $"許容範囲:{Definition.Range[0]}-{Definition.Range[1]}";
                    }
                    text = string.IsNullOrEmpty(text) ? range : $"{text}\n{range}";
                }
                return text;
            }
        }
        public ParameterViewModel(ParameterDefinition def)
        {
            Definition = def;
            Validate();
        }
        public void RefreshValidation()
        {
            Validate();
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(IsValid));
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                IsValid = false;
                return;
            }

            switch (Definition.Type)
            {
                case "int":
                case "float":
                    if (!double.TryParse(Value, out double v))
                    { IsValid = false; return; }
                    if (Definition.Range != null && (v < Definition.Range[0] || v > Definition.Range[1]))
                    { IsValid = false; return; }
                    break;
                case "bool":
                    if (Value != "true" && Value != "false")
                    { IsValid = false; return; }
                    break;
                case "enum":
                    if (Definition.Values == null || !Definition.Values.Contains(Value))
                    { IsValid = false; return; }
                    break;
                case "ip":
                    string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9]{1,2})\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9]{1,2})$";
                    IsValid = Regex.IsMatch(Value, pattern);
                    return;
            }

            IsValid = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
