using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Jogos
{
    public class CriarJogoInput : BaseInput<CriarJogoInput>
    {
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public string? Desenvolvedora { get; private set; }
        public DateTime? DataLancamento { get; private set; }
        public decimal Preco { get; private set; }
        public bool Ativo { get; private set; }

        public CriarJogoInput(string nome,
            string? descricao,
            string? desenvolvedora,
            DateTime? dataLancamento,
            decimal preco,
            bool? ativo)
        {
            Nome = nome;
            Descricao = descricao;
            Desenvolvedora = desenvolvedora;
            DataLancamento = dataLancamento;
            Preco = preco;
            Ativo = ativo ?? true;
        }

        protected override IValidator<CriarJogoInput> GetValidator()
        {
            return new CriarJogoInputValidator();
        }
    }

    public class CriarJogoInputValidator : AbstractValidator<CriarJogoInput>
    {
        public CriarJogoInputValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Nome é um campo obrigatório.");

            RuleFor(p => p.Nome)
                .MaximumLength(256)
                .WithMessage("Nome deve ter até 256 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Nome));

            RuleFor(p => p.Descricao)
                .MaximumLength(1024)
                .WithMessage("Descricao deve ter até 1024 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Descricao));

            RuleFor(p => p.Desenvolvedora)
                .MaximumLength(256)
                .WithMessage("Desenvolvedora deve ter até 256 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Desenvolvedora));

            RuleFor(p => p.Preco)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Preco precisa ser maior ou igual a zero.");
        }
    }
}