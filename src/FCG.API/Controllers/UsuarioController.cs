using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.Services;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Authorize(Roles = Roles.USUARIO)]
    [Route("usuario")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioAppService _service;

        public UsuarioController(ILogger<UsuarioController> logger,
            IUsuarioAppService service)
        {
            _logger = logger;
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("cadastrar", Name = "CadastrarUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioInput input)
        {
            var resultado = await _service.Cadastrar(input);

            return !resultado.Success ? BadRequest(resultado) : Created();
        }

        [AllowAnonymous]
        [HttpPost("login", Name = "LoginUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> LoginUsuario([FromBody] LoginUsuarioInput input)
        {
            var resultado = await _service.Login(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        [HttpGet("perfil", Name = "ObterPerfilUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPerfilUsuario()
        {
            var perfil = _service.ObterPerfil();
            return Ok(perfil);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpGet("{id}", Name = "ObterUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterUsuario([FromRoute] Guid id)
        {
            var usuario = await _service.ObterPorId(id);

            return usuario is null ? NotFound() : Ok(usuario);
        }
    }
}