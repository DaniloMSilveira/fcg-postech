using System.Transactions;
using FCG.Application.DTOs.Outputs;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Services;
using FCG.Application.DTOs.Outputs.Autenticacao;
using FCG.Application.DTOs.Inputs.Autenticacao;

using RegistrarUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Autenticacao.RegistrarUsuarioOutput>;
using LoginUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Autenticacao.LoginUsuarioOutput>;

namespace FCG.Application.Services
{
    public class AutenticacaoAppService : IAutenticacaoAppService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IUserContext _userContext;

        public AutenticacaoAppService(IUsuarioRepository repository,
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IUserContext userContext)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _userContext = userContext;
        }

        public async Task<BaseOutput<RegistrarUsuarioOutput>> Registrar(RegistrarUsuarioInput input)
        {
            if (!input.IsValid())
                return RegistrarUsuarioResult.Fail(input.ValidationResult);

            var existeUsuario = await _repository.ExisteUsuario(input.Email);
            if (existeUsuario)
                return RegistrarUsuarioResult.Fail("Já existe um usuário cadastrado com este e-mail.");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var identityResponse = await _identityService.CriarUsuario(input.Nome, input.Email, input.Senha);
                if (!identityResponse.Success)
                    throw new Exception("Erro ao criar usuário no identity.");

                var usuario = new Usuario(input.Nome, input.Email);
                await _unitOfWork.UsuarioRepository.Adicionar(usuario);

                var success = await _unitOfWork.Commit();
                if (!success)
                    throw new Exception("Erro ao criar usuário no domínio.");

                scope.Complete();
                return RegistrarUsuarioResult.Ok(new RegistrarUsuarioOutput(usuario.Id, usuario.Nome, usuario.Email));
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

        public async Task<BaseOutput> AlterarSenha(AlterarSenhaInput input)
        {
            if (!input.IsValid())
                return BaseOutput.Fail(input.ValidationResult);

            var identityResponse = await _identityService.AlterarSenha(_userContext.Email, input.SenhaAtual, input.NovaSenha);
            if (!identityResponse.Success)
                return BaseOutput.Fail(identityResponse.Errors);

            return BaseOutput.Ok();
        }
    }
}