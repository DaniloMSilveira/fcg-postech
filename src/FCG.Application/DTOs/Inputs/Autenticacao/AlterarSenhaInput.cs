using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Helpers;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Autenticacao
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
                .WithMessage("SenhaAtual é um campo obrigatório.");

            RuleFor(p => p.NovaSenha)
                .NotEmpty()
                .WithMessage("NovaSenha é um campo obrigatório.");

            RuleFor(p => p.NovaSenha)
                .NotEqual(c => c.SenhaAtual)
                .WithMessage("NovaSenha deve ser diferente da senha atual.")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));

            RuleFor(p => p.NovaSenha)
                .MaximumLength(40)
                .WithMessage("NovaSenha deve ter até 40 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));

            RuleFor(p => p.NovaSenha)
                .Must(ValidatorHelper.ValidStrongPassword)
                .WithMessage("NovaSenha deve se enquadrar nos requisitos de segurança (mínimo 8 caracteres, uma letra maiúscula, " +
                        "uma letra minúscula, um número e um caracter especial).")
                .When(p => !string.IsNullOrWhiteSpace(p.NovaSenha));
        }
    }
}