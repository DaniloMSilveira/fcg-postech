using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FCG.Application.DTOs.Inputs.Usuarios;
using FCG.Application.DTOs.Outputs;
using FCG.Application.DTOs.Outputs.Autenticacao;
using FCG.Application.Security;
using FCG.Application.Services;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Security.Models;
using FCG.Infra.Security.Services;
using FluentAssertions;
using Moq;

namespace FCG.UnitTests.ApplicationServices
{
    public class AutenticacaoAppServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly AutenticacaoAppService _appService;
        public string _nome;
        public string _email;
        public string _senha;

        public AutenticacaoAppServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _identityServiceMock = new Mock<IIdentityService>();
            _userContextMock = new Mock<IUserContext>();

            _appService = new AutenticacaoAppService(
                _usuarioRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _identityServiceMock.Object,
                _userContextMock.Object);

            var faker = new Faker("pt_BR");
            _nome = faker.Person.FullName;
            _email = faker.Person.Email;
            _senha = "Senha@123"; // Estática para validar regra de segurança
        }

        [Fact]
        public async Task Registrar_DeveRetornarSucesso_QuandoDadosValidos()
        {
            var input = new RegistrarUsuarioInput(_nome, _email, _senha);

            _usuarioRepositoryMock.Setup(r => r.ExisteUsuario(input.Email)).ReturnsAsync(false);

            _identityServiceMock.Setup(s => s.CriarUsuario(input.Nome, input.Email, input.Senha))
                .ReturnsAsync(new IdentityResponse(true));

            var usuarioRepoMock = new Mock<IUsuarioRepository>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var result = await _appService.Registrar(input);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Email.Should().Be(input.Email);
        }

        [Fact]
        public async Task Registrar_DeveRetornarErro_QuandoInputInvalido()
        {
            // Arrange
            var input = new RegistrarUsuarioInput("", "", "");

            // Act
            var result = await _appService.Registrar(input);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Registrar_DeveRetornarErro_QuandoEmailJaCadastrado()
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nome, _email, _senha);
            _usuarioRepositoryMock.Setup(r => r.ExisteUsuario(input.Email)).ReturnsAsync(true);

            // Act
            var result = await _appService.Registrar(input);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Já existe um usuário cadastrado com este e-mail.");
        }

        [Fact]
        public async Task Registrar_DeveRetornarErro_QuandoFalhaNoIdentity()
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nome, _email, _senha);

            _identityServiceMock.Setup(s => s.CriarUsuario(input.Nome, input.Email, input.Senha))
                .ReturnsAsync(new IdentityResponse("Erro"));

            // Act
            Func<Task> result = async () => await _appService.Registrar(input);

            // Assert
            await result.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao criar usuário no identity.");
        }

        [Fact]
        public async Task Registrar_DeveRetornarErro_QuandoFalhaNoUnitOfWork()
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nome, _email, _senha);

            _usuarioRepositoryMock.Setup(r => r.ExisteUsuario(input.Email)).ReturnsAsync(false);

            _identityServiceMock.Setup(s => s.CriarUsuario(input.Nome, input.Email, input.Senha))
                .ReturnsAsync(new IdentityResponse(true));

            var usuarioRepoMock = new Mock<IUsuarioRepository>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            // Act
            Func<Task> result = async () => await _appService.Registrar(input);

            // Assert
            await result.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao criar usuário no domínio.");
        }

        [Fact]
        public async Task Login_DeveRetornarErro_QuandoInputInvalido()
        {
            // Arrange
            var input = new LoginUsuarioInput("", "");

            // Act
            var result = await _appService.Login(input);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Login_DeveRetornarErro_QuandoCredenciaisInvalidas()
        {
            // Arrange
            var input = new LoginUsuarioInput(_email, _senha);

            _identityServiceMock.Setup(s => s.Login(input.Email, input.Senha))
                .ReturnsAsync(new IdentityTokenResponse("Credenciais inválidas"));

            // Act
            var result = await _appService.Login(input);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas");
        }

        [Fact]
        public async Task Login_DeveRetornarSucesso_QuandoCredenciaisValidas()
        {
            // Arrange
            var input = new LoginUsuarioInput(_email, _senha);

            var output = new LoginUsuarioOutput("token123", "refresh456");

            _identityServiceMock.Setup(s => s.Login(input.Email, input.Senha))
                .ReturnsAsync(new IdentityTokenResponse(output.AccessToken, output.RefreshToken));

            // Act
            var result = await _appService.Login(input);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.AccessToken.Should().Be(output.AccessToken);
            result.Data.RefreshToken.Should().Be(output.RefreshToken);
        }

        [Fact]
        public void ObterPerfil_DeveRetornarPerfilDoUsuario()
        {
            // Arrange
            _userContextMock.SetupGet(x => x.Id).Returns(Guid.NewGuid().ToString());
            _userContextMock.SetupGet(x => x.Nome).Returns(_nome);
            _userContextMock.SetupGet(x => x.Email).Returns(_email);
            _userContextMock.SetupGet(x => x.Roles).Returns(["Admin", "Usuario"]);

            // Act
            var result = _appService.ObterPerfil();

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(_nome);
            result.Email.Should().Be(_email);
            result.Roles.Should().Contain("Admin");
            result.Roles.Should().Contain("Usuario");
        }

        [Fact]
        public async Task AlterarSenha_DeveRetornarErro_QuandoInputInvalido()
        {
            // Arrange
            var input = new AlterarSenhaInput("", "");

            // Act
            var result = await _appService.AlterarSenha(input);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task AlterarSenha_DeveRetornarErro_QuandoFalhaNoIdentity()
        {
            var input = new AlterarSenhaInput(_senha, $"new{_senha}");
            _userContextMock.Setup(x => x.Email).Returns(_email);

            _identityServiceMock.Setup(s => s.AlterarSenha(_email, input.SenhaAtual, input.NovaSenha))
                .ReturnsAsync(new IdentityResponse("Erro ao alterar senha no identity."));

            var result = await _appService.AlterarSenha(input);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Erro ao alterar senha no identity.");
        }

        [Fact]
        public async Task AlterarSenha_DeveRetornarSucesso_QuandoDadosValidos()
        {
            var input = new AlterarSenhaInput(_senha, $"new{_senha}");
            _userContextMock.Setup(x => x.Email).Returns(_email);

            _identityServiceMock.Setup(s => s.AlterarSenha(_email, input.SenhaAtual, input.NovaSenha))
                .ReturnsAsync(new IdentityResponse(true));

            var result = await _appService.AlterarSenha(input);

            result.Success.Should().BeTrue();
        }
    }
}