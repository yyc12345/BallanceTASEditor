using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BallanceTasEditor.Converters {
    public class GenericConverter<TIn, TOut> : IValueConverter {

        private readonly Func<TIn, TOut> converter;

        public GenericConverter(Func<TIn, TOut> converter) { this.converter = converter; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is TIn t) {
                return converter(t);
            } else {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }
    }

    public static class ConverterWarehouse {
        public static readonly GenericConverter<int, string> FpsConverter =
            new GenericConverter<int, string>((v) => Utils.FpsConverter.ToDelta(v).ToString());
    }
}
