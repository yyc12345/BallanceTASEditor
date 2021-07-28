using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BallanceTASEditor.UI {

    public class AddItemConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            try {
                var textCount = values[0] as string;
                var textFps = values[1] as string;

                var count = int.Parse(textCount);
                var fps = float.Parse(textFps);

                if (count <= 0 || fps <= 0) return false;
                return true;
            } catch  {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class FPS2DeltaTimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var text = value as string;
            if (text == null) return "0";

            float data;
            if (!float.TryParse(text, out data)) return "0";
            return (1000f / data).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
