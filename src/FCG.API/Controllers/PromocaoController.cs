using FCG.Application.DTOs.Inputs.Promocoes;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Outputs.Promocoes;
using FCG.Application.DTOs.Queries.Promocoes;
using FCG.Application.Services;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Authorize(Roles = Roles.USUARIO)]
    [Route("promocoes")]
    public class PromocaoController : Controller
    {
        private readonly ILogger<PromocaoController> _logger;
        private readonly IPromocaoAppService _promocaoAppService;

        public PromocaoController(ILogger<PromocaoController> logger,
            IPromocaoAppService promocaoAppService)
        {
            _logger = logger;
            _promocaoAppService = promocaoAppService;
        }

        /// <summary>
        /// Pesquisa promoções de jogos ativas.
        /// </summary>
        /// <remarks>
        /// Permite que usuários consultem a lista de jogos que estão em promoção, com suporte à paginação.
        /// </remarks>
        /// <param name="query">
        /// Parâmetros da pesquisa, incluindo número da página e tamanho da página.
        /// </param>
        /// <response code="200">Retorna a lista paginada das promoções encontradas.</response>
        [HttpGet("pesquisar", Name = "PesquisarPromocoes")]
        [ProducesResponseType(typeof(PaginacaoOutput<PromocaoItemListaOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarPromocoes([FromQuery] PesquisarPromocoesQuery query)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _promocaoAppService.PesquisarPromocoes(query, true);

            return Ok(resultado);
        }

        /// <summary>
        /// Pesquisa promoções de jogos com perfil de administrador.
        /// </summary>
        /// <remarks>
        /// Permite que usuários administradores consultem a lista de todas promoções, mesmo que inativas, com suporte à paginação.
        /// </remarks>
        /// <param name="query">
        /// Parâmetros da pesquisa, incluindo número da página e tamanho da página.
        /// </param>
        /// <response code="200">Retorna a lista paginada das promoções encontradas.</response>
        [HttpGet("pesquisar-admin", Name = "PesquisarPromocoesAdmin")]
        [ProducesResponseType(typeof(PaginacaoOutput<PromocaoItemListaOutput>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarPromocoesAdmin([FromQuery] PesquisarPromocoesQuery query, bool? ativo)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _promocaoAppService.PesquisarPromocoes(query, ativo);

            return Ok(resultado);
        }

        /// <summary>
        /// Obtém os dados da promoção pelo seu identificador.
        /// </summary>
        /// <remarks>
        /// Retorna os dados da promoção.
        /// </remarks>
        /// <param name="id">Identificador da promoção.</param>
        /// <response code="200">Promoção encontrada com sucesso. Retorna os dados da promoção.</response>
        /// <response code="404">Promoção não encontrada.</response>
        [HttpGet("{id}", Name = "ObterPromocao")]
        [ProducesResponseType(typeof(PromocaoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPromocao([FromRoute] Guid id)
        {
            var promocao = await _promocaoAppService.ObterPorId(id);

            return promocao is null ? NotFound() : Ok(promocao);
        }

        /// <summary>
        /// Cria uma nova promoção de um jogo.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o preço menor que o preço atual do jogo e um intervalo de data válido
        /// </remarks>
        /// <param name="input">Dados necessários para o registro da promoção.</param>
        /// <response code="201">Promoção criada com sucesso. Retorna os dados da promoção.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPost(Name = "CriarPromocao")]
        [ProducesResponseType(typeof(PromocaoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPromocao([FromBody] CriarPromocaoInput input)
        {
            var resultado = await _promocaoAppService.Criar(input);

            return !resultado.Success
                ? BadRequest(resultado)
                : CreatedAtRoute("CriarPromocao", new { id = resultado.Data.Id }, resultado.Data);
        }

        /// <summary>
        /// Altera as informações de uma promoção.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o preço menor que o preço atual do jogo e um intervalo de data válido
        /// </remarks>
        /// <param name="input">Dados necessários para alterar a promoção.</param>
        /// <response code="200">Promoção alterada com sucesso. Retorna os dados da promoção.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPut("{id}", Name = "AlterarPromocao")]
        [ProducesResponseType(typeof(PromocaoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarPromocao([FromRoute] Guid id, [FromBody] AlterarPromocaoInput input)
        {
            input.PreencherId(id);
            var resultado = await _promocaoAppService.Alterar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        /// <summary>
        /// Ativa uma promoção.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o id da promoção que será ativada
        /// </remarks>
        /// <param name="id">Identificador da promoção.</param>
        /// <response code="200">Promoção ativada com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/ativar", Name = "AtivarPromocao")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtivarPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Ativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        /// <summary>
        /// Inativar uma promoção.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o id da promoção que será inativada
        /// </remarks>
        /// <param name="id">Identificador da promoção.</param>
        /// <response code="200">Promoção inativada com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/inativar", Name = "InativarPromocao")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InativarPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Inativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        /// <summary>
        /// Remove uma promoção do sistema.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// Essa operação é irreversível e remove permanentemente a promoção da base de dados.
        /// </remarks>
        /// <param name="id">Identificador da promoção.</param>
        /// <response code="204">Promoção removida com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverPromocao")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}