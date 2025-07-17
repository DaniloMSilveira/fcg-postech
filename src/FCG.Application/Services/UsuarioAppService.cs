using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Outputs;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Services;

using CadastrarUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.CadastrarUsuarioOutput>;
using LoginUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.LoginUsuarioOutput>;

namespace FCG.Application.Services
{
    public class UsuarioAppService : IUsuarioAppService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IUserContext _userContext;

        public UsuarioAppService(IUsuarioRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IUserContext userContext)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _userContext = userContext;
        }

        public async Task<BaseOutput<CadastrarUsuarioOutput>> Cadastrar(CadastrarUsuarioInput input)
        {
            if (!input.IsValid())
                return CadastrarUsuarioResult.Fail(input.ValidationResult);

            var existeUsuario = await _repository.ExisteUsuario(input.Email);
            if (existeUsuario)
                return CadastrarUsuarioResult.Fail("Já existe um usuário cadastrado com este e-mail.");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var identityResponse = await _identityService.CadastrarUsuario(input.Email, input.Senha);
                if (!identityResponse.Success)
                    throw new Exception("Erro ao cadastrar usuário no identity.");

                var usuario = new Usuario(input.Nome, input.Email);
                await _unitOfWork.UsuarioRepository.Adicionar(usuario);

                var success = await _unitOfWork.Commit();
                if (!success)
                    throw new Exception("Erro ao persistir usuário de domínio.");

                scope.Complete();
                return CadastrarUsuarioResult.Ok(new CadastrarUsuarioOutput(usuario.Id));
            }
        }

        public async Task<BaseOutput<LoginUsuarioOutput>> Login(LoginUsuarioInput input)
        {
            if (!input.IsValid())
                return LoginUsuarioResult.Fail(input.ValidationResult);

            var identityResponse = await _identityService.Login(input.Email, input.Senha);
            if (!identityResponse.Success)
                return LoginUsuarioResult.Fail(identityResponse.Error);

            return LoginUsuarioResult.Ok(new LoginUsuarioOutput(identityResponse.AccessToken, identityResponse.RefreshToken));
        }

        public PerfilUsuarioOutput ObterPerfil()
        {
            return new PerfilUsuarioOutput
            {
                Id = _userContext.Id,
                Nome = _userContext.Nome,
                Email = _userContext.Email,
                Roles = _userContext.Roles
            };
        }

        public async Task<UsuarioOutput?> ObterPorId(Guid id)
        {
            var usuario = await _repository.ObterPorId(id);
            if (usuario is null)
                return null;

            return new UsuarioOutput(usuario.Id, usuario.Nome, usuario.Email);
        }
    }
}