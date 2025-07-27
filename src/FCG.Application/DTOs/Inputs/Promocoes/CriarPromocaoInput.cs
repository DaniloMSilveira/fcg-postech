using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Promocoes
{
    public class CriarPromocaoInput : BaseInput<CriarPromocaoInput>
    {
        public Guid JogoId { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public bool Ativo { get; private set; }

        public CriarPromocaoInput(Guid jogoId,
            decimal preco,
            DateTime dataInicio,
            DateTime dataFim,
            bool ativo)
        {
            JogoId = jogoId;
            Preco = preco;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Ativo = ativo;
        }

        protected override IValidator<CriarPromocaoInput> GetValidator()
        {
            return new CriarPromocaoInputValidator();
        }
    }

    public class CriarPromocaoInputValidator : AbstractValidator<CriarPromocaoInput>
    {
        public CriarPromocaoInputValidator()
        {
            RuleFor(p => p.JogoId)
                .NotEmpty()
                .WithMessage("JogoId é um campo obrigatório.");

            RuleFor(p => p.JogoId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("JogoId é um campo obrigatório.");

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