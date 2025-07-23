using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries.Usuarios;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Services;

using RegistrarUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Usuarios.RegistrarUsuarioOutput>;
using LoginUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Usuarios.LoginUsuarioOutput>;
using CriarUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Usuarios.CriarUsuarioOutput>;
using RemoverUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using AlterarSenhaResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;
using FCG.Application.DTOs.Queries;

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

        public async Task<RemoverUsuarioResult> AlterarSenha(AlterarSenhaInput input)
        {
            if (!input.IsValid())
                return AlterarSenhaResult.Fail(input.ValidationResult);

            var identityResponse = await _identityService.AlterarSenha(_userContext.Email, input.SenhaAtual, input.NovaSenha);
            if (!identityResponse.Success)
                return AlterarSenhaResult.Fail(identityResponse.Errors);

            return AlterarSenhaResult.Ok();
        }

        public async Task<PaginacaoOutput<UsuarioItemListaOutput>> PesquisarUsuarios(PesquisarUsuariosQuery query)
        {
            var (usuarios, total) = await _repository.Consultar(query.Pagina, query.TamanhoPagina, query.Filtro);

            var dados = usuarios.Select(p => new UsuarioItemListaOutput(p.Id, p.Nome, p.Email));

            return new PaginacaoOutput<UsuarioItemListaOutput>
            {
                PaginaAtual = query.Pagina,
                TamanhoPagina = query.TamanhoPagina,
                TotalRegistros = total,
                TotalPaginas = (int)Math.Ceiling((double)total / query.TamanhoPagina),
                Dados = dados.ToList()
            };
        }

        public async Task<UsuarioOutput?> ObterPorId(Guid id)
        {
            var usuario = await _repository.ObterPorId(id);
            if (usuario is null)
                return null;

            return new UsuarioOutput(usuario.Id, usuario.Nome, usuario.Email);
        }

        public async Task<BaseOutput<CriarUsuarioOutput>> Criar(CriarUsuarioInput input)
        {
            if (!input.IsValid())
                return CriarUsuarioResult.Fail(input.ValidationResult);

            var existeUsuario = await _repository.ExisteUsuario(input.Email);
            if (existeUsuario)
                return CriarUsuarioResult.Fail("Já existe um usuário cadastrado com este e-mail.");

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
                return CriarUsuarioResult.Ok(new CriarUsuarioOutput(usuario.Id));
            }
        }

        public async Task<BaseOutput<bool>> Remover(Guid id)
        {
            var usuario = await _repository.ObterPorId(id);
            if (usuario is null)
                return RemoverUsuarioResult.Fail("Usuário não encontrado.");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var identityResponse = await _identityService.RemoverUsuario(usuario.Email);
                if (!identityResponse.Success)
                    throw new Exception("Erro ao remover usuário no identity.");

                _unitOfWork.UsuarioRepository.Remover(usuario);
                var success = await _unitOfWork.Commit();
                if (!success)
                    throw new Exception("Erro ao remover usuário no domínio.");

                scope.Complete();
                return RemoverUsuarioResult.Ok();
            }

        }
    }
}