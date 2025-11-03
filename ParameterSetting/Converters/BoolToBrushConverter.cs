using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ParamEditor.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public BoolToBrushConverter()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = value is bool b && b;
            var trueBlush = (Brush)Application.Current.Resources["BoolTrueBrush"];
            var falseBrush = (Brush)Application.Current.Resources["BoolFalseBrush"];
            return isTrue ? trueBlush : falseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
