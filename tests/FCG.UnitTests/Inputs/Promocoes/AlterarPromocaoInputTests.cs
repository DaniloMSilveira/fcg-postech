using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Promocoes;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Promocoes
{
    public class AlterarPromocaoInputTests
    {
        private Guid _idValido;
        private decimal _precoValido;
        private DateTime _dataInicioValida;
        private DateTime _dataFimValida;

        public AlterarPromocaoInputTests()
        {
            var faker = new Faker("pt_BR");
            _idValido = Guid.NewGuid();
            _precoValido = faker.Random.Number(0, 100);
            _dataInicioValida = DateTime.Now;
            _dataFimValida = DateTime.Now.AddHours(4);
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var input = new AlterarPromocaoInput(
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoIdNaoPreenchido()
        {
            // Arrange
            var input = new AlterarPromocaoInput(
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Id é um campo obrigatório.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoIdVazio()
        {
            // Arrange
            var input = new AlterarPromocaoInput(
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );
            var id = Guid.Empty;
            input.PreencherId(id);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Id é um campo obrigatório.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void IsValid_DeveRetornarErro_QuandoPrecoNegativo(decimal preco)
        {
            // Arrange
            var input = new AlterarPromocaoInput(
                preco: preco,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Preco deve ser maior ou igual a zero.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoDataFimMenorDataInicio()
        {
            // Arrange
            var input = new AlterarPromocaoInput(
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataInicioValida.AddHours(-2)
            );

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "DataFim deve ser maior que a data de início.");
        }
    }
}