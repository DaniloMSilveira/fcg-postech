using FCG.Application.DTOs.Inputs.Promocoes;
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

        [HttpGet("pesquisar", Name = "PesquisarPromocoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarPromocoes([FromQuery] PesquisarPromocoesQuery query)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _promocaoAppService.PesquisarPromocoes(query);

            return Ok(resultado);
        }

        [HttpGet("{id}", Name = "ObterPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPromocao([FromRoute] Guid id)
        {
            var promocao = await _promocaoAppService.ObterPorId(id);

            return promocao is null ? NotFound() : Ok(promocao);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPost(Name = "CriarPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CriarPromocao([FromBody] CriarPromocaoInput input)
        {
            var resultado = await _promocaoAppService.Criar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPut("{id}", Name = "AlterarPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarPromocao([FromRoute] Guid id, [FromBody] AlterarPromocaoInput input)
        {
            input.PreencherId(id);
            var resultado = await _promocaoAppService.Alterar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/ativar", Name = "AtivarPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AtivarPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Ativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/inativar", Name = "InativarPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InativarPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Inativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverPromocao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoverPromocao([FromRoute] Guid id)
        {
            var resultado = await _promocaoAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}