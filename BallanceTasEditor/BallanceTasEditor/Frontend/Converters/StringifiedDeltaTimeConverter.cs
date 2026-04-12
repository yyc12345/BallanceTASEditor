using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BallanceTasEditor.Frontend.Converters {

    [ValueConversion(typeof(string), typeof(string))]
    public class StringifiedDeltaTimeConverter : IValueConverter {
        public static readonly StringifiedDeltaTimeConverter Instance = new StringifiedDeltaTimeConverter();
        
        public StringifiedDeltaTimeConverter() {
            m_Validator = new Validators.FpsValidator();
        }

        private Validators.FpsValidator m_Validator;
        private static readonly string INVALID_DELTA_TIME = "N/A";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var stringifiedFps = value as string;
            if (stringifiedFps is null) {
                return INVALID_DELTA_TIME;
            }

            return m_Validator.Validate(stringifiedFps).Match(
                v => Backend.FpsConverter.ToDelta(v).ToString(),
                _ => INVALID_DELTA_TIME
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }
}
