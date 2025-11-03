using System.Windows;
using ParamEditor.Themes;

namespace ParamEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.LoadLastTheme();
        }
    }

}
