using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Autenticacao
{
    public class AlterarAcessosUsuarioInputTests
    {
        private Guid _usuarioId = Guid.NewGuid();
        private List<string> _roles = [ "USUARIO" ];

        [Fact]
        public void IsValid_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var input = new AlterarAcessosUsuarioInput(_usuarioId, _roles);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoUsuarioIdVazio()
        {
            // Arrange
            var input = new AlterarAcessosUsuarioInput(Guid.Empty, _roles);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "UsuarioId é um campo obrigatório.");
        }
        
        [Fact]
        public void IsValid_DeveRetornarErro_QuandoRolesVazia()
        {
            // Arrange
            var input = new AlterarAcessosUsuarioInput(_usuarioId, []);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "É necessário informar ao menos uma role de acesso.");
        }
    }
}