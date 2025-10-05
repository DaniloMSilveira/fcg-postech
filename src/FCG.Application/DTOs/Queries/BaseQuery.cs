using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FCG.Application.DTOs.Queries
{
    public abstract class BaseQuery<TQuery> where TQuery : BaseQuery<TQuery>
    {
        private ValidationResult _validationResult;
        public ValidationResult ValidationResult => _validationResult;

        public bool IsValid()
        {
            var validator = GetValidator();
            if (validator == null)
                throw new InvalidOperationException("Validator not provided.");

            _validationResult = validator.Validate((TQuery)this);
            return _validationResult.IsValid;
        }

        protected abstract IValidator<TQuery> GetValidator();
    }
}