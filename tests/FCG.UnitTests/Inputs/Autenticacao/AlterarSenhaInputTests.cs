using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Autenticacao
{
    public class AlterarSenhaInputTests
    {
        private string _senhaAtualValida = "Senha@123";
        private string _novaSenhaValida = "Senha!123";

        [Fact]
        public void IsValid_DeveRetornarSucesso_DadosValidos()
        {
            // Arrange
            var input = new AlterarSenhaInput(_senhaAtualValida, _novaSenhaValida);

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
        public void IsValid_DeveRetornarErro_SenhaAtualObrigatoria(string senhaAtual)
        {
            // Arrange
            var input = new AlterarSenhaInput(senhaAtual, _novaSenhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "SenhaAtual é um campo obrigatório.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_NovaSenhaObrigatoria(string novaSenha)
        {
            // Arrange
            var input = new AlterarSenhaInput(_senhaAtualValida, novaSenha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "NovaSenha é um campo obrigatório.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_NovaSenhaIgualSenhaAtual()
        {
            // Arrange
            var novaSenha = _senhaAtualValida;
            var input = new AlterarSenhaInput(_senhaAtualValida, novaSenha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "NovaSenha deve ser diferente da senha atual.");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345678")]
        [InlineData("senha@123")]
        [InlineData("Senha123")]
        public void IsValid_DeveRetornarErro_NovaSenhaInvalida(string novaSenha)
        {
            // Arrange
            var input = new AlterarSenhaInput(_senhaAtualValida, novaSenha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(
                e => e.ErrorMessage == "NovaSenha deve se enquadrar nos requisitos de segurança (mínimo 8 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caracter especial).");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_NovaSenhaTamanhoMaximo()
        {
            // Arrange
            var novaSenha = _novaSenhaValida + new string('a', 40);
            var input = new AlterarSenhaInput(_senhaAtualValida, novaSenha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "NovaSenha deve ter até 40 caracteres.");
        }
    }
}