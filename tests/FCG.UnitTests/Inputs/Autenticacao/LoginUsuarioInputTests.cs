using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Autenticacao
{
    public class LoginUsuarioInputTests
    {
        private string _emailValido;
        private string _senhaValida;

        public LoginUsuarioInputTests()
        {
            var faker = new Faker("pt_BR");
            _emailValido = faker.Internet.Email();
            _senhaValida = "Senha@123"; // Estática para validar regra de segurança
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var input = new LoginUsuarioInput(_emailValido, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_QuandoEmailVazio(string email)
        {
            // Arrange
            var input = new LoginUsuarioInput(email, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email é um campo obrigatório.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_QuandoSenhaVazia(string senha)
        {
            // Arrange
            var input = new LoginUsuarioInput(_emailValido, senha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Senha é um campo obrigatório.");
        }
    }
}