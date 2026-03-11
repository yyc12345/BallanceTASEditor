using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {
    public static class NullableExtensions {
        public static T Unwrap<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class {
            ArgumentNullException.ThrowIfNull(value, paramName);
            return value;
        }
    }
}
