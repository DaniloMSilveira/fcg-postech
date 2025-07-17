using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Helpers;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs
{
    public class LoginUsuarioInput : BaseInput<LoginUsuarioInput>
    {
        public string? Email { get; private set; }
        public string? Senha { get; private set; }

        public LoginUsuarioInput(string email, string senha)
        {
            Email = email;
            Senha = senha;
        }

        protected override IValidator<LoginUsuarioInput> GetValidator()
        {
            return new LoginUsuarioInputValidator();
        }
    }

    public class LoginUsuarioInputValidator : AbstractValidator<LoginUsuarioInput>
    {
        public LoginUsuarioInputValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Email é um campo obrigatório.");

            RuleFor(p => p.Senha)
                .NotEmpty()
                .WithMessage("Senha é um campo obrigatório.");
        }
    }
}