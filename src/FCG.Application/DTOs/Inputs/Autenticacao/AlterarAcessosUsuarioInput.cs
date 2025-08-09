using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Inputs.Autenticacao
{
    public class AlterarAcessosUsuarioInput : BaseInput<AlterarAcessosUsuarioInput>
    {
        public Guid UsuarioId { get; private set; }
        public List<string> Roles { get; private set; }

        public AlterarAcessosUsuarioInput(Guid usuarioId, List<string> roles)
        {
            UsuarioId = usuarioId;
            Roles = roles;
        }

        protected override IValidator<AlterarAcessosUsuarioInput> GetValidator()
        {
            return new AlterarAcessosUsuarioInputValidator();
        }
    }

    public class AlterarAcessosUsuarioInputValidator : AbstractValidator<AlterarAcessosUsuarioInput>
    {
        public AlterarAcessosUsuarioInputValidator()
        {
            RuleFor(p => p.UsuarioId)
                .NotEmpty()
                .WithMessage("UsuarioId é um campo obrigatório.");

            RuleFor(p => p.UsuarioId)
                .NotEqual(p => Guid.Empty)
                .WithMessage("UsuarioId é um campo obrigatório.");

            RuleFor(p => p.Roles)
                .NotEmpty()
                .WithMessage("É necessário informar ao menos uma role de acesso.");
        }
    }
}