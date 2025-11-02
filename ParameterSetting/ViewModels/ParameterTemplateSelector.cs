using System.Windows;
using System.Windows.Controls;

namespace ParamEditor.ViewModels
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextTemplate { get; set; }
        public DataTemplate? BoolTemplate { get; set; }
        public DataTemplate? EnumTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ParameterViewModel vm)
            {
                return vm.Type switch
                {
                    "bool" => BoolTemplate,
                    "enum" => EnumTemplate,
                    _ => TextTemplate
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
