using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace BallanceTASEditor.UI {

    [ValueConversion(typeof(bool), typeof(Color))]
    public class BackgroundConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                bool bl = System.Convert.ToBoolean(value);
                if (bl) return Color.FromRgb(30, 144, 255);
                else return Color.FromArgb(0, 255, 255, 255);
            } catch {
                return Color.FromArgb(0, 255, 255, 255);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    [ValueConversion(typeof(float), typeof(string))]
    public class FloatConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                float bl = System.Convert.ToSingle(value);
                if (bl < 0) return "";
                else return bl.ToString();
            } catch {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }    
    
    [ValueConversion(typeof(long), typeof(string))]
    public class LongConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                float bl = System.Convert.ToInt64(value);
                if (bl < 0) return "";
                else return bl.ToString();
            } catch {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
