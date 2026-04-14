using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BallanceTasEditor.Frontend.Converters {

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class CellIsSelectedToColorConverter : IValueConverter {
        public static readonly CellIsSelectedToColorConverter Instance = new CellIsSelectedToColorConverter();

        private static readonly SolidColorBrush SELECTED_BRUSH = new(Color.FromRgb(255, 152, 0));
        private static readonly SolidColorBrush DESELECTED_BRUSH = new(Color.FromRgb(158, 158, 158));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var isSelected = value as bool?;
            if (isSelected is null) {
                return Binding.DoNothing;
            } else {
                if (isSelected.Value) {
                    return SELECTED_BRUSH;
                } else {
                    return DESELECTED_BRUSH;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class CellIsSetToColorConverter : IValueConverter {
        public static readonly CellIsSetToColorConverter Instance = new CellIsSetToColorConverter();

        private static readonly SolidColorBrush SET_BRUSH = new(Color.FromRgb(30, 144, 255));
        private static readonly SolidColorBrush UNSET_BRUSH = new(Color.FromRgb(255, 255, 255));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var isSet = value as bool?;
            if (isSet is null) {
                return Binding.DoNothing;
            } else {
                if (isSet.Value) {
                    return SET_BRUSH;
                } else {
                    return UNSET_BRUSH;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class HeaderIsSelectedToColorConverter : IValueConverter {
        public static readonly HeaderIsSelectedToColorConverter Instance = new HeaderIsSelectedToColorConverter();

        private static readonly SolidColorBrush SELECTED_BRUSH = new(Color.FromRgb(30, 144, 255));
        private static readonly SolidColorBrush DESELECTED_BRUSH = new(Color.FromRgb(255, 255, 255));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var isSelected = value as bool?;
            if (isSelected is null) {
                return Binding.DoNothing;
            } else {
                if (isSelected.Value) {
                    return SELECTED_BRUSH;
                } else {
                    return DESELECTED_BRUSH;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

}
