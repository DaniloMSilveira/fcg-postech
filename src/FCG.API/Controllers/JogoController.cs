using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Inputs.Jogos;
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

        [HttpGet("pesquisar", Name = "PesquisarJogos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarJogos([FromQuery] PesquisarJogosQuery query)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _jogoAppService.PesquisarJogos(query);

            return Ok(resultado);
        }

        [HttpGet("{id}", Name = "ObterJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterJogo([FromRoute] Guid id)
        {
            var jogo = await _jogoAppService.ObterPorId(id);

            return jogo is null ? NotFound() : Ok(jogo);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPost(Name = "CriarJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CriarJogo([FromBody] CriarJogoInput input)
        {
            var resultado = await _jogoAppService.Criar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPut("{id}", Name = "AlterarJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarJogo([FromRoute] Guid id, [FromBody] AlterarJogoInput input)
        {
            input.PreencherId(id);
            var resultado = await _jogoAppService.Alterar(input);

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/ativar", Name = "AtivarJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AtivarJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Ativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("{id}/inativar", Name = "InativarJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InativarJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Inativar(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverJogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoverJogo([FromRoute] Guid id)
        {
            var resultado = await _jogoAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}