using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Validator {

    public sealed class ValidatorAdapter<TIn, TOut, V> where V: IValidator<TIn, TOut> {
        public ValidatorAdapter(IValidator<TIn, TOut> validator) {
            m_Validator = validator;
        }

        private readonly IValidator<TIn, TOut> m_Validator;

        public ValidationResult? Validate(TIn value, ValidationContext validationContext) {
            // YYC MARK:
            // Due to the shitty behavior of LanguageExt
            // which do not allow I return nullable class from Match,
            // I was forcely use MatchUnsafe.
            return m_Validator.Validate(value).MatchUnsafe(
                Left: v => ValidationResult.Success,
                Right: err => new ValidationResult(err)
            );
        }

        public TOut Conclude(TIn value) {
            return m_Validator.Validate(value).Match(
                Left: v => v,
                Right: _ => throw new InvalidOperationException("Can not unwrap an error casting.")
            );
        }

    }

}
