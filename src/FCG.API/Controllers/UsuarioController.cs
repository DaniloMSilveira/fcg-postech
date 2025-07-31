using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Queries.Usuarios;
using FCG.Application.Security;
using FCG.Application.Services;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Authorize]
    [Route("usuarios")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuarioController(ILogger<UsuarioController> logger,
            IUsuarioAppService usuarioAppService)
        {
            _logger = logger;
            _usuarioAppService = usuarioAppService;
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpGet("pesquisar", Name = "PesquisarUsuarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PesquisarUsuarios([FromQuery] PesquisarUsuariosQuery query)
        {
            if (query.Pagina <= 0 || query.TamanhoPagina <= 0)
                return BadRequest(new { error = "Parâmetros inválidos." });

            var resultado = await _usuarioAppService.PesquisarUsuarios(query);

            return Ok(resultado);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpGet("{id}", Name = "ObterUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterUsuario([FromRoute] Guid id)
        {
            var usuario = await _usuarioAppService.ObterPorId(id);

            return usuario is null ? NotFound() : Ok(usuario);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPost(Name = "CriarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioInput input)
        {
            var resultado = await _usuarioAppService.Criar(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoverUsuario([FromRoute] Guid id)
        {
            var resultado = await _usuarioAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        #region Biblioteca

        [Authorize(Roles = Roles.USUARIO)]
        [HttpGet("biblioteca", Name = "ObterBibliotecaUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterBibliotecaUsuario()
        {
            var resultado = await _usuarioAppService.ObterBibliotecaUsuario();

            return Ok(resultado);
        }

        [Authorize(Roles = Roles.USUARIO)]
        [HttpPost("biblioteca", Name = "AdicionarJogoBibliotecaUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AdicionarJogoBibliotecaUsuario([FromBody] AdicionarJogoBibliotecaInput input)
        {
            var resultado = await _usuarioAppService.AdicionarJogoBibliotecaUsuario(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        #endregion
    }
}