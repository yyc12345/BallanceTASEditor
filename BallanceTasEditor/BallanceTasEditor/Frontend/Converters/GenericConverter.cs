using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BallanceTasEditor.Frontend.Converters {
    [ValueConversion(typeof(int), typeof(string))]
    public class FpsConverter : IValueConverter {
        public static FpsConverter Instance = new FpsConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is int tv) {
                if (tv <= 0) return DependencyProperty.UnsetValue;
                else return Backend.FpsConverter.ToDelta(tv).ToString();
            } else {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 将IsEnable转换为Visibility。
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConverter : IValueConverter {
        public static VisibilityConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool bv) {
                if (bv) return Visibility.Visible;
                else return Visibility.Collapsed;
            } else {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

}
