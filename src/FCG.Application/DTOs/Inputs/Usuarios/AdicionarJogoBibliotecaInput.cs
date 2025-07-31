using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Usuarios
{
    public class AdicionarJogoBibliotecaInput : BaseInput<AdicionarJogoBibliotecaInput>
    {
        public Guid JogoId { get; private set; }

        public AdicionarJogoBibliotecaInput(Guid jogoId)
        {
            JogoId = jogoId;
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
            RuleFor(p => p.JogoId)
                .NotEmpty()
                .WithMessage("JogoId é um campo obrigatório.");

            RuleFor(p => p.JogoId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("JogoId é um campo obrigatório.");
        }
    }
}