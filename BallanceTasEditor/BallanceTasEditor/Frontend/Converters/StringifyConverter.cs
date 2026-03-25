using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BallanceTasEditor.Frontend.Converters {

    [ValueConversion(typeof(int?), typeof(string))]
    public class StringifyIntegerConverter : IValueConverter {
        public static StringifyIntegerConverter Instance = new StringifyIntegerConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is null) {
                return "";
            } else if (value is int iv) {
                return iv.ToString();
            } else {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string s) {
                if (int.TryParse(s, out int iv)) return iv;
                else return null;
            } else {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
