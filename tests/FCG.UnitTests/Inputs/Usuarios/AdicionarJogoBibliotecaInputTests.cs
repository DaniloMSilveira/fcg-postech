using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Inputs.Usuarios;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Usuarios
{
    public class AdicionarJogoBibliotecaInputValidatorTests
    {
        [Fact]
        public void IsValid_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var jogoId = Guid.NewGuid();
            var input = new AdicionarJogoBibliotecaInput(jogoId);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoJogoIdVazio()
        {
            // Arrange
            var jogoId = Guid.Empty;
            var input = new AdicionarJogoBibliotecaInput(jogoId);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "JogoId é um campo obrigatório.");
        }
    }
}