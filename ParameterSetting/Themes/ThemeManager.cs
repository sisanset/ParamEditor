using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Serialization;
namespace ParamEditor.Themes
{
    internal class ThemeManager
    {
        public static event EventHandler? ThemeChanged;
        private static readonly Uri DarkThemeUri = new("/ParamEditor;component/Themes/DarkTheme.xaml", UriKind.Relative);
        private static readonly Uri LightThemeUri = new("/ParamEditor;component/Themes/LightTheme.xaml", UriKind.Relative);
        public static bool IsDarkTheme { get; private set; } = true;
        public static void ApplyTheme(bool useDark)
        {
            var app = Application.Current;
            var existing = app.Resources.MergedDictionaries.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.Contains("DarkTheme.xaml") ||
                 d.Source.OriginalString.Contains("LightTheme.xaml")));

            if (existing != null)
            {
                app.Resources.MergedDictionaries.Remove(existing);
            }
            var newTheme = new ResourceDictionary
            {
                Source = useDark ? DarkThemeUri : LightThemeUri
            };
            app.Resources.MergedDictionaries.Add(newTheme);
            IsDarkTheme = useDark;
            Properties.Settings.Default.LastTheme = IsDarkTheme ? "Dark" : "Light";
            Properties.Settings.Default.Save();

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }
        public static void ToggleTheme()
        {
            ApplyTheme(!IsDarkTheme);
        }
        public static void LoadLastTheme()
        {
            var lastTheme = Properties.Settings.Default.LastTheme;
            bool useDark = lastTheme == "Dark";
            ApplyTheme(useDark);
        }
    }
}
