using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Inputs.Jogos;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Outputs.Jogos;
using FCG.Application.DTOs.Queries;
using FCG.Application.DTOs.Queries.Jogos;
using FCG.Application.Services;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Authorize(Roles = Roles.USUARIO)]
    [Route("jogos")]
    public class JogoController : Controller
    {
        private readonly ILogger<JogoController> _logger;
        private readonly IJogoAppService _jogoAppService;

        public JogoController(ILogger<JogoController> logger,
            IJogoAppService jogoAppService)
        {
            _logger = logger;
            _jogoAppService = jogoAppService;
        }


        /// <summary>
        /// Pesquisa jogos no sistema.
        /// </summary>
        /// <remarks>
        /// Permite que usuários consultem a lista de jogos disponíveis na plataforma, com suporte à paginação.
        /// </remarks>
        /// <param name="query">
        /// Parâmetros da pesquisa, incluindo número da página e tamanho da página.
        /// </param>
        /// <response code="200">Retorna a lista paginada dos jogos encontrados.</response>
        [HttpGet("pesquisar", Name = "PesquisarJogos")]
        [ProducesResponseType(typeof(PaginacaoOutput<JogoItemListaOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarJogos([FromQuery] PesquisarJogosQuery query)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _jogoAppService.PesquisarJogos(query, true);

            return Ok(resultado);
        }

        /// <summary>
        /// Pesquisa jogos no sistema com perfil de administrador.
        /// </summary>
        /// <remarks>
        /// Permite que usuários administradores consultem a lista de todos os jogos disponíveis na plataforma, mesmo que inativos, com suporte à paginação.
        /// </remarks>
        /// <param name="query">
        /// Parâmetros da pesquisa, incluindo número da página e tamanho da página.
        /// </param>
        /// <response code="200">Retorna a lista paginada dos jogos encontrados.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpGet("pesquisar-admin", Name = "PesquisarJogosAdmin")]
        [ProducesResponseType(typeof(PaginacaoOutput<JogoItemListaOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarJogosAdmin([FromQuery] PesquisarJogosQuery query, [FromQuery] bool? ativo)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _jogoAppService.PesquisarJogos(query, ativo);

            return Ok(resultado);
        }

        /// <summary>
        /// Obtém os dados do jogo pelo seu identificador.
        /// </summary>
        /// <remarks>
        /// Retorna os dados do jogo.
        /// </remarks>
        /// <param name="id">Identificador do jogo.</param>
        /// <response code="200">Jogo encontrado com sucesso. Retorna os dados do jogo.</response>
        /// <response code="404">Jogo não encontrado.</response>
        [HttpGet("{id}", Name = "ObterJogo")]
        [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterJogo([FromRoute] Guid id)
        {
            var jogo = await _jogoAppService.ObterPorId(id);

            return jogo is null ? NotFound() : Ok(jogo);
        }


        /// <summary>
        /// Cria um novo jogo no sistema.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar um nome válido e um preço para o jogo (caso seja zerado o jogo será gratuito)
        /// </remarks>
        /// <param name="input">Dados necessários para o registro do jogo.</param>
        /// <response code="201">Jogo criado com sucesso. Retorna os dados do jogo.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPost(Name = "CriarJogo")]
        [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarJogo([FromBody] CriarJogoInput input)
        {
            var resultado = await _jogoAppService.Criar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        /// <summary>
        /// Altera as informações de um jogo.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar um nome válido e um preço para o jogo (caso seja zerado o jogo será gratuito)
        /// </remarks>
        /// <param name="input">Dados necessários para alterar o jogo.</param>
        /// <response code="200">Jogo alterado com sucesso. Retorna os dados do jogo.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPut("{id}", Name = "AlterarJogo")]
        [ProducesResponseType(typeof(JogoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarJogo([FromRoute] Guid id, [FromBody] AlterarJogoInput input)
        {
            input.PreencherId(id);
            var resultado = await _jogoAppService.Alterar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        /// <summary>
        /// Ativa um jogo.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o id do jogo que será ativado
        /// </remarks>
        /// <param name="id">Identificador do jogo.</param>
        /// <response code="200">Jogo ativado com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/ativar", Name = "AtivarJogo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtivarJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Ativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        /// <summary>
        /// Inativa um jogo.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o id do jogo que será inativado
        /// </remarks>
        /// <param name="id">Identificador do jogo.</param>
        /// <response code="200">Jogo inativado com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/inativar", Name = "InativarJogo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InativarJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Inativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        /// <summary>
        /// Remove um jogo do sistema.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// Essa operação é irreversível e remove permanentemente o jogo da base de dados.
        /// </remarks>
        /// <param name="id">Identificador do jogo.</param>
        /// <response code="204">Jogo removido com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverJogo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}