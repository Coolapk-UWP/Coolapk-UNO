using System;
using Windows.UI.Xaml.Data;

namespace CoolapkUNO.Helpers.ValueConverters
{
    public class HTMLToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => value.ToString().CSStoString();

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}
