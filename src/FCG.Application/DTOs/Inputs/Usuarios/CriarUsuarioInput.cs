using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Helpers;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Usuarios
{
    public class CriarUsuarioInput : BaseInput<CriarUsuarioInput>
    {
        public string? Nome { get; private set; }
        public string? Email { get; private set; }
        public string? Senha { get; private set; }

        public CriarUsuarioInput(string nome, string email, string senha)
        {
            Nome = nome;
            Email = email;
            Senha = senha;
        }

        protected override IValidator<CriarUsuarioInput> GetValidator()
        {
            return new CriarUsuarioInputValidator();
        }
    }

    public class CriarUsuarioInputValidator : AbstractValidator<CriarUsuarioInput>
    {
        public CriarUsuarioInputValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Nome é um campo obrigatório.");

            RuleFor(p => p.Nome)
                .MaximumLength(256)
                .WithMessage("Nome deve ter até 256 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Nome));

            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Email é um campo obrigatório.");

            RuleFor(p => p.Email)
                .MaximumLength(100)
                .WithMessage("Email deve ter até 100 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Email));

            RuleFor(p => p.Email)
                .Must(ValidatorHelper.ValidEmail)
                .WithMessage("Email inválido.")
                .When(p => !string.IsNullOrWhiteSpace(p.Email));

            RuleFor(p => p.Senha)
                .NotEmpty()
                .WithMessage("Senha é um campo obrigatório.");

            RuleFor(p => p.Senha)
                .MaximumLength(40)
                .WithMessage("Senha deve ter até 40 caracteres.")
                .When(p => !string.IsNullOrWhiteSpace(p.Senha));

            RuleFor(p => p.Senha)
                .Must(ValidatorHelper.ValidStrongPassword)
                .WithMessage("Senha deve se enquadrar nos requisitos de segurança (mínimo 8 caracteres, uma letra maiúscula, " +
                        "uma letra minúscula, um número e um caracter especial).")
                .When(p => !string.IsNullOrWhiteSpace(p.Senha));
        }
    }
}