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
        /// É necessário informar nome, e-mail válido e uma senha que atenda aos critérios de segurança definidos: 
        /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
        /// </remarks>
        /// <param name="input">Dados necessários para o registro do usuário.</param>
        /// <response code="201">Usuário registrado com sucesso. Retorna os dados do usuário.</response>
        /// <response code="400">Requisição inválida.</response>
        [HttpPost("registrar", Name = "Registrar")]
        [ProducesResponseType(typeof(RegistrarUsuarioOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioInput input)
        {
            var resultado = await _autenticacaoAppService.Registrar(input);

            return !resultado.Success
                ? BadRequest(resultado)
                : CreatedAtRoute("Registrar", new { id = resultado.Data.Id }, resultado.Data);
        }

        /// <summary>
        /// Realiza o login de um usuário no sistema.
        /// </summary>
        /// <remarks>
        /// É necessário informar o e-mail e senha válidos do usuário.
        /// </remarks>
        /// <param name="input">Credenciais do usuário para autenticação.</param>
        /// <response code="200">Usuário autenticado com sucesso. Retorna os dados de acesso.</response>
        /// <response code="400">Requisição inválida ou credenciais incorretas.</response>
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(typeof(LoginUsuarioOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioInput input)
        {
            var resultado = await _autenticacaoAppService.Login(input);

            return !resultado.Success ? BadRequest(resultado) : Ok(resultado.Data);
        }

        /// <summary>
        /// Obtém o perfil do usuário autenticado no sistema.
        /// </summary>
        /// <remarks>
        /// O usuário deve estar autenticado para realizar esta operação.
        /// </remarks>
        /// <response code="200">Retorna os dados do usuário autenticado e suas roles de acesso.</response>
        [Authorize]
        [HttpGet("perfil", Name = "ObterPerfil")]
        [ProducesResponseType(typeof(PerfilUsuarioOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPerfil()
        {
            var perfil = _autenticacaoAppService.ObterPerfil();
            return Ok(perfil);
        }

        /// <summary>
        /// Altera a senha do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// O usuário deve estar autenticado para realizar esta operação.  
        /// É necessário informar a senha atual e a nova senha, que deve atender aos critérios de segurança definidos: 
        /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
        /// </remarks>
        /// <param name="input">Dados necessários para alteração da senha.</param>
        /// <response code="204">Senha alterada com sucesso.</response>
        /// <response code="400">Requisição inválida ou senha incorreta.</response>
        [Authorize]
        [HttpPatch("alterar-senha", Name = "AlterarSenha")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaInput input)
        {
            var resultado = await _autenticacaoAppService.AlterarSenha(input);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
        
        /// <summary>
        /// Alterar acessos de um usuário no sistema.
        /// </summary>
        /// <remarks>
        /// Requer acesso de administrador. 
        /// É necessário informar o id do usuário e as roles de acesso
        /// </remarks>
        /// <param name="input">Dados necessários para alteração de acessos do usuário.</param>
        /// <response code="201">Acessos alterados com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [Authorize(Roles = Roles.ADMINISTRADOR)]
        [HttpPatch("alterar-acessos", Name = "AlterarAcessosUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseOutput), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AlterarAcessosUsuario([FromBody] AlterarAcessosUsuarioInput input)
        {
            var resultado = await _autenticacaoAppService.AlterarAcessos(input);

            return !resultado.Success ? BadRequest(resultado) : NoContent();
        }
    }
}