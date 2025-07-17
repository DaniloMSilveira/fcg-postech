using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FCG.Application.DTOs.Inputs
{
    public abstract class BaseInput<TInput> where TInput : BaseInput<TInput>
    {
        private ValidationResult _validationResult;
        public ValidationResult ValidationResult => _validationResult;

        public bool IsValid()
        {
            var validator = GetValidator();
            if (validator == null)
                throw new InvalidOperationException("Validator not provided.");

            _validationResult = validator.Validate((TInput)this);
            return _validationResult.IsValid;
        }

        protected abstract IValidator<TInput> GetValidator();
    }
}