using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Promocoes;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Promocoes
{
    public class CriarPromocaoInputTests
    {
        private Guid _jogoIdValido;
        private decimal _precoValido;
        private DateTime _dataInicioValida;
        private DateTime _dataFimValida;

        public CriarPromocaoInputTests()
        {
            var faker = new Faker("pt_BR");
            _jogoIdValido = Guid.NewGuid();
            _precoValido = faker.Random.Number(0, 100);
            _dataInicioValida = DateTime.Now;
            _dataFimValida = DateTime.Now.AddHours(4);
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_DadosValidos()
        {
            // Arrange
            var input = new CriarPromocaoInput(
                jogoId: _jogoIdValido,
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_DeveRetornarErro_JogoIdObrigatorio()
        {
            // Arrange
            var jogoId = Guid.Empty;
            var input = new CriarPromocaoInput(
                jogoId: jogoId,
                preco: _precoValido,
                dataInicio: _dataInicioValida,
                dataFim: _dataFimValida
            );

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "JogoId é um campo obrigatório.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void IsValid_DeveRetornarErro_PrecoNegativo(decimal preco)
        {
            // Arrange
            var input = new CriarPromocaoInput(
                jogoId: _jogoIdValido,
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
        public void IsValid_DeveRetornarErro_DataFimMenorDataInicio()
        {
            // Arrange
            var input = new CriarPromocaoInput(
                jogoId: _jogoIdValido,
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