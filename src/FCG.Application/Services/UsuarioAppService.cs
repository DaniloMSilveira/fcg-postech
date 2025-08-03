using System.Transactions;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Queries.Usuarios;
using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Services;

using CriarUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Usuarios.UsuarioOutput>;
using AdicionarJogoBibliotecaUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<FCG.Application.DTOs.Outputs.Usuarios.UsuarioJogoOutput>;
using RemoverUsuarioResult = FCG.Application.DTOs.Outputs.BaseOutput<bool>;

namespace FCG.Application.Services
{
    public class UsuarioAppService : IUsuarioAppService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IUserContext _userContext;

        public UsuarioAppService(IUsuarioRepository repository,
            IJogoRepository jogoRepository,
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IUserContext userContext)
        {
            _repository = repository;
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _userContext = userContext;
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

        public async Task<BaseOutput<UsuarioOutput>> Criar(CriarUsuarioInput input)
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
                return CriarUsuarioResult.Ok(new UsuarioOutput(usuario.Id, usuario.Nome, usuario.Email));
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

        #region Biblioteca

        public async Task<IEnumerable<UsuarioJogoOutput>> ObterBibliotecaUsuario()
        {
            var usuario = await _repository.ObterUsuarioPorEmail(_userContext.Email);
            if (usuario is null)
                throw new Exception("Não foi possível obter o usuário no domínio.");

            return usuario.Jogos.Select(item => new UsuarioJogoOutput
            {
                Id = item.Id,
                JogoId = item.Jogo.Id,
                JogoNome = item.Jogo.Nome
            });
        }

        public async Task<BaseOutput<UsuarioJogoOutput>> AdicionarJogoBibliotecaUsuario(AdicionarJogoBibliotecaInput input)
        {
            if (!input.IsValid())
                return AdicionarJogoBibliotecaUsuarioResult.Fail(input.ValidationResult);

            var usuario = await _repository.ObterUsuarioPorEmail(_userContext.Email);
            if (usuario is null)
                throw new Exception("Não foi possível obter o usuário no domínio.");

            var jogo = await _jogoRepository.ObterJogoPromocoes(input.JogoId);
            if (jogo is null)
                return AdicionarJogoBibliotecaUsuarioResult.Fail("Jogo não encontrado.");

            var usuarioPossuiJogo = usuario.Jogos.Any(p => p.JogoId == jogo.Id);
            if (usuarioPossuiJogo)
                return AdicionarJogoBibliotecaUsuarioResult.Fail("Usuário já possui este jogo.");

            var dataAtual = DateTime.Now;
            var promocaoAtiva = jogo.Promocoes.FirstOrDefault(p => p.DataInicio <= dataAtual && p.DataFim >= dataAtual);

            var usuarioJogo = new UsuarioJogo(
                usuario.Id,
                jogo.Id,
                promocaoAtiva is not null ? promocaoAtiva.Preco : jogo.Preco,
                promocaoAtiva is not null ? promocaoAtiva.Id : null);
            await _unitOfWork.UsuarioRepository.AdicionarJogoBiblioteca(usuarioJogo);

            var success = await _unitOfWork.Commit();
            if (!success)
                throw new Exception("Erro ao adicionar jogo para o usuário no domínio.");

            return AdicionarJogoBibliotecaUsuarioResult.Ok(new UsuarioJogoOutput
            {
                Id = usuarioJogo.Id,
                JogoId = usuarioJogo.JogoId,
                JogoNome = jogo.Nome
            });
        }

        #endregion
    }
}