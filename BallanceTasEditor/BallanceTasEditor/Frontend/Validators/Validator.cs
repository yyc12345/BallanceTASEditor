using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Validators {

    public interface IValidator<TIn, TOut> {
         OneOf<TOut, string> Validate(TIn value);
    }

    public sealed class FpsValidator : IValidator<string, uint> {
        public OneOf<uint, string> Validate(string value) {
            if (uint.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out uint fps)) {
                if (Backend.FpsConverter.IsValidFps(fps)) {
                    return fps;
                } else {
                    return "Given FPS is out of range.";
                }
            } else {
                return "Given string can not be parsed as unsigned integer.";
            }
        }
    }

    public sealed class CountValidator : IValidator<string, int> {
        public OneOf<int, string> Validate(string value) {
            if (int.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out int count)) {
                if (count > 0) {
                    return count;
                } else {
                    return "Given count must be greater than zero.";
                }
            } else {
                return "Given string can not be parsed as integer.";
            }
        }
    }

}
