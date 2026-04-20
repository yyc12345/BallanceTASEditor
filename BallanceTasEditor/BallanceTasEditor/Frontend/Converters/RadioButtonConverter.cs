using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BallanceTasEditor.Frontend.Converters {

    internal class GenericEnumRadioButtonConverter<T> where T: struct {
        public object Convert(object value, object parameter) {
            var susValue = value as T?;
            var susParam = parameter as T?;

            if (susValue is null || susParam is null) {
                return Binding.DoNothing;
            } else {
                return susValue.Value.Equals(susParam.Value);
            }
        }

        public object ConvertBack(object value, object parameter) {
            var susValue = value as bool?;
            var susParam = parameter as T?;

            if (susValue is null || susParam is null) {
                return Binding.DoNothing;
            } else {
                return susValue.Value ? susParam.Value : Binding.DoNothing;
            }
        }
    }

    [ValueConversion(typeof(Shared.SequenceKind), typeof(bool))]
    public class SequenceKindRadioButtonConverter : IValueConverter {
        public static readonly SequenceKindRadioButtonConverter Instance = new();

        private readonly GenericEnumRadioButtonConverter<Shared.SequenceKind> m_Inner = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.ConvertBack(value, parameter);
        }
    }

    [ValueConversion(typeof(Shared.EditorLayoutKind), typeof(bool))]
    public class EditorLayoutKindRadioButtonConverter : IValueConverter {
        public static readonly EditorLayoutKindRadioButtonConverter Instance = new();

        private readonly GenericEnumRadioButtonConverter<Shared.EditorLayoutKind> m_Inner = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.ConvertBack(value, parameter);
        }
    }

    [ValueConversion(typeof(Shared.EditorPasteBehavior), typeof(bool))]
    public class EditorPasteBehaviorRadioButtonConverter : IValueConverter {
        public static readonly EditorPasteBehaviorRadioButtonConverter Instance = new();

        private readonly GenericEnumRadioButtonConverter<Shared.EditorPasteBehavior> m_Inner = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return m_Inner.ConvertBack(value, parameter);
        }
    }

}
