using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Utils {

    // YYC MARK:
    // 这些验证器尽管服务于UI，但是并不遵循WPF或者CommunityToolkit.Mvvm的Validator模型，
    // 所以我把他们放在这里。

    /// <summary>
    /// 验证器接口。
    /// </summary>
    /// <typeparam name="TIn">验证器接受的待验证数据的类型。</typeparam>
    /// <typeparam name="TOut">验证器验证完毕后，会输出的类型。</typeparam>
    public interface IValidator<TIn, TOut> {
        /// <summary>
        /// 验证给定数据是否正确。
        /// </summary>
        /// <param name="data">要验证的数据。</param>
        /// <returns>数据正确，或对应的错误信息。</returns>
        ValidationResult Validate(TIn data);

        /// <summary>
        /// 获取验证无误数据转换后的数据。
        /// </summary>
        /// <param name="data">验证无误，用于获取输出的数据。</param>
        /// <returns>输出的数据。</returns>
        /// <exception cref="ArgumentException">给定数据验证时出现错误。</exception>
        TOut Fetch(TIn data);
    }

    /// <summary>
    /// 以字符串呈现的数据的通用验证器
    /// </summary>
    public abstract class StringifiedValueValidator<T> : IValidator<string, T> where T : notnull {
        /// <summary>
        /// 用户需要实现的验证函数。
        /// </summary>
        /// <param name="stringifiedValue">要进行验证的数据。</param>
        /// <returns>验证完毕用于输出的数值，或者验证失败时的错误消息。</returns>
        protected abstract Either<T, string> ValidateValue(string stringifiedValue);

        public ValidationResult Validate(string data) {
            return ValidateValue(data).Match(
                Left: (_) => ValidationResult.Success,
                Right: (v) => new ValidationResult(v)
            );
        }

        public T Fetch(string data) {
            return ValidateValue(data).Match(
                Left: (v) => v,
                Right: (msg) => throw new ArgumentException($"Given value can not pass Validator due to {msg}.")
            );
        }
    }

    public abstract class IntegerValidator : StringifiedValueValidator<int> {
        protected override Either<int, string> ValidateValue(string stringifiedValue) {
            if (int.TryParse(stringifiedValue, out int val)) {
                return Left(val);
            } else {
                return Right("Given string do not represent any valid number.");
            }
        }
    }

    public class FpsValidator : IntegerValidator {
        public static FpsValidator Instance = new FpsValidator();

        protected override Either<int, string> ValidateValue(string stringifiedValue) {
            return base.ValidateValue(stringifiedValue).BindLeft<int>((v) => {
                if (v <= 0) return Right("Fps must be greater than zero.");
                else return Left(v);
            });
        }
    }

    public class CountValidator : IntegerValidator {
        public static FpsValidator Instance = new FpsValidator();

        protected override Either<int, string> ValidateValue(string stringifiedValue) {
            return base.ValidateValue(stringifiedValue).BindLeft<int>((v) => {
                if (v < 0) return Right("Count can not lower than zero.");
                else return Left(v);
            });
        }
    }


}
