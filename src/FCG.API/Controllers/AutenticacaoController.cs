using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FCG.Application.DTOs.Inputs;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Outputs.Autenticacao;
using FCG.Application.DTOs.Queries.Usuarios;
using FCG.Application.Services;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers
{
    [Route("autenticacao")]
    public class AutenticacaoController : Controller
    {
        private readonly ILogger<AutenticacaoController> _logger;
        private readonly IAutenticacaoAppService _autenticacaoAppService;

        public AutenticacaoController(ILogger<AutenticacaoController> logger,
            IAutenticacaoAppService autenticacaoAppService)
        {
            _logger = logger;
            _autenticacaoAppService = autenticacaoAppService;
        }

        
        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <remarks>
        /// Requer um nome, e-mail válido e senha com os critérios definidos pela validação (ex: mínimo de 6 caracteres).
        /// </remarks>
        /// <param name="input">Dados necessários para o registro do usuário.</param>
        /// <returns>Resultado do registro com os dados do novo usuário ou erros de validação.</returns>
        [HttpPost("registrar", Name = "Registrar")]
        [ProducesResponseType(typeof(RegistrarUsuarioOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseErrorOutput), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioInput input)
        {
            var resultado = await _autenticacaoAppService.Registrar(input);

            return !resultado.Success
                ? BadRequest(resultado)
                : CreatedAtRoute("Registrar", new { id = resultado.Data.Id }, resultado.Data);
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioInput input)
        {
            var resultado = await _autenticacaoAppService.Login(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        [Authorize]
        [HttpGet("perfil", Name = "ObterPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPerfil()
        {
            var perfil = _autenticacaoAppService.ObterPerfil();
            return Ok(perfil);
        }

        [Authorize]
        [HttpPatch("alterar-senha", Name = "AlterarSenha")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaInput input)
        {
            var resultado = await _autenticacaoAppService.AlterarSenha(input);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}