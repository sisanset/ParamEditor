using System.Collections.ObjectModel;

namespace ParamEditor.ViewModels
{
    internal class ParameterGroupViewModel
    {
        public string GroupName { get; }
        public ObservableCollection<ParameterViewModel> Parameters { get; } = new();
        public ParameterGroupViewModel(string groupName)
        {
            GroupName = groupName;
        }
    }
}
