using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Usuarios
{
    public class AdicionarJogoBibliotecaInput : BaseInput<AdicionarJogoBibliotecaInput>
    {
        public Guid UsuarioId { get; private set; }
        public Guid JogoId { get; private set; }
        public decimal PrecoCompra { get; private set; }
        public Guid? PromocaoId { get; private set; }

        public AdicionarJogoBibliotecaInput(Guid usuarioId, Guid jogoId, decimal precoCompra, Guid? promocaoId)
        {
            UsuarioId = usuarioId;
            JogoId = jogoId;
            PrecoCompra = precoCompra;
            PromocaoId = promocaoId;
        }

        protected override IValidator<AdicionarJogoBibliotecaInput> GetValidator()
        {
            return new AdicionarJogoBibliotecaInputValidator();
        }
    }

    public class AdicionarJogoBibliotecaInputValidator : AbstractValidator<AdicionarJogoBibliotecaInput>
    {
        public AdicionarJogoBibliotecaInputValidator()
        {
            RuleFor(p => p.UsuarioId)
                .NotEmpty()
                .WithMessage("UsuarioId é um campo obrigatório.");

            RuleFor(p => p.UsuarioId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("UsuarioId é um campo obrigatório.");

            RuleFor(p => p.JogoId)
                .NotEmpty()
                .WithMessage("JogoId é um campo obrigatório.");

            RuleFor(p => p.JogoId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("JogoId é um campo obrigatório.");

            RuleFor(p => p.PrecoCompra)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PrecoCompra deve ser maior ou igual a zero.");

            RuleFor(p => p.PromocaoId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("PromocaoId inválido.")
                .When(p => p.PromocaoId != null);
        }
    }
}