using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Promocoes
{
    public class AlterarPromocaoInput : BaseInput<AlterarPromocaoInput>
    {
        public Guid Id { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }

        public AlterarPromocaoInput(Guid id,
            decimal preco,
            DateTime dataInicio,
            DateTime dataFim)
        {
            Id = id;
            Preco = preco;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public void PreencherId(Guid id)
        {
            Id = id;
        }

        protected override IValidator<AlterarPromocaoInput> GetValidator()
        {
            return new AlterarPromocaoInputValidator();
        }
    }

    public class AlterarPromocaoInputValidator : AbstractValidator<AlterarPromocaoInput>
    {
        public AlterarPromocaoInputValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .WithMessage("Id é um campo obrigatório.");

            RuleFor(p => p.Id)
                .NotEqual(p => Guid.Empty)
                .WithMessage("Id é um campo obrigatório.");

            RuleFor(p => p.Preco)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Preco deve ser maior ou igual a zero.");

            RuleFor(p => p.DataInicio)
                .GreaterThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("DataInicio deve ser maior ou igual a data atual.");

            RuleFor(p => p.DataFim)
                .GreaterThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("DataFim deve ser maior ou igual a data atual.");

            RuleFor(p => p.DataFim)
                .GreaterThan(p => p.DataInicio)
                .WithMessage("DataFim deve ser maior que a data de início.");
        }
    }
}