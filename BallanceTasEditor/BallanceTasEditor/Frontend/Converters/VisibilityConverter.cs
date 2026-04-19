using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BallanceTasEditor.Frontend.Converters {

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class IsVisibleToVisibilityConverter : IValueConverter {
        public static readonly IsVisibleToVisibilityConverter Instance = new IsVisibleToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var susBool = value as bool?;
            if (susBool is null) {
                return Binding.DoNothing;
            } else {
                return susBool.Value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }
}
