using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Queries;
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
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuarioController(ILogger<UsuarioController> logger,
            IUsuarioAppService usuarioAppService)
        {
            _logger = logger;
            _usuarioAppService = usuarioAppService;
        }

        #region Autenticação

        [AllowAnonymous]
        [HttpPost("autenticacao/registrar", Name = "RegistrarUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistrarUsuarioInput input)
        {
            var resultado = await _usuarioAppService.Registrar(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        [AllowAnonymous]
        [HttpPost("autenticacao/login", Name = "LoginUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> LoginUsuario([FromBody] LoginUsuarioInput input)
        {
            var resultado = await _usuarioAppService.Login(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        [HttpGet("autenticacao/perfil", Name = "ObterPerfilUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPerfilUsuario()
        {
            var perfil = _usuarioAppService.ObterPerfil();
            return Ok(perfil);
        }

        [HttpPatch("autenticacao/alterar-senha", Name = "AlterarSenhaUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarSenhaUsuario([FromBody] AlterarSenhaInput input)
        {
            var resultado = await _usuarioAppService.AlterarSenha(input);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        #endregion

        #region Gerenciamento Usuarios

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpGet("pesquisa", Name = "PesquisarUsuarios")]
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

            return !resultado.Success ? BadRequest(resultado)  : Ok(resultado.Data);
        }

        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpDelete("{id}", Name = "RemoverUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoverUsuario([FromRoute] Guid id)
        {
            var resultado = await _usuarioAppService.Remover(id);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }

        #endregion
    }
}