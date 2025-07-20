using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Helpers;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Usuarios
{
    public class AlterarSenhaInput : BaseInput<AlterarSenhaInput>
    {
        public string SenhaAtual { get; private set; }
        public string NovaSenha { get; private set; }

        public AlterarSenhaInput(string senhaAtual, string novaSenha)
        {
            SenhaAtual = senhaAtual;
            NovaSenha = novaSenha;
        }

        protected override IValidator<AlterarSenhaInput> GetValidator()
        {
            return new AlterarSenhaInputValidator();
        }
    }

    public class AlterarSenhaInputValidator : AbstractValidator<AlterarSenhaInput>
    {
        public AlterarSenhaInputValidator()
        {
            RuleFor(p => p.SenhaAtual)
                .NotEmpty()
                .WithMessage("Senha atual é um campo obrigatório.");

            RuleFor(p => p.NovaSenha)
                .NotEmpty()
                .WithMessage("Nova senha é um campo obrigatório.");

            RuleFor(p => p.NovaSenha)
                .NotEqual(c => c.SenhaAtual)
                .WithMessage("Nova senha deve ser diferente da senha atual.")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));

            RuleFor(p => p.NovaSenha)
                .MaximumLength(40)
                .WithMessage("Nova senha deve ter até 40 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));

            RuleFor(p => p.NovaSenha)
                .Must(ValidatorHelper.ValidStrongPassword)
                .WithMessage("Nova senha deve se enquadrar nos requisitos de segurança (mínimo 8 caracteres, uma letra maiúscula, " +
                        "uma letra minúscula, um número e um caracter especial).")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));
        }
    }
}