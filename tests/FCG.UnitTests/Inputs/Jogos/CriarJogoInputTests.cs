using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Jogos;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Jogos
{
    public class CriarJogoInputTests
    {
        private string _nomeValido;
        private string _descricaoValida;
        private string _desenvolvedoraValida;
        private DateTime _dataLancamentoValida;
        private decimal _precoValido;

        public CriarJogoInputTests()
        {
            var faker = new Faker("pt_BR");
            _nomeValido = faker.Lorem.Sentence(3);
            _descricaoValida = faker.Lorem.Sentence(10, 2);
            _desenvolvedoraValida = faker.Lorem.Sentence(3);
            _dataLancamentoValida = DateTime.Now.Date;
            _precoValido = faker.Random.Number(0, 100);
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_DadosValidos()
        {
            // Arrange
            var input = new CriarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeTrue();
            input.ValidationResult.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_NomeObrigatorio(string nome)
        {
            // Arrange
            var input = new CriarJogoInput(
                nome: nome,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome é um campo obrigatório.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_NomeTamanhoMaximo()
        {
            // Arrange
            var nome = _nomeValido + new string('a', 256);
            var input = new CriarJogoInput(
                nome: nome,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome deve ter até 256 caracteres.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_DescricaoTamanhoMaximo()
        {
            // Arrange
            var descricao = _descricaoValida + new string('a', 1024);
            var input = new CriarJogoInput(
                nome: _nomeValido,
                descricao: descricao,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Descricao deve ter até 1024 caracteres.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_DesenvolvedoraTamanhoMaximo()
        {
            // Arrange
            var desenvolvedora = _desenvolvedoraValida + new string('a', 256);
            var input = new CriarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: desenvolvedora,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Desenvolvedora deve ter até 256 caracteres.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void IsValid_DeveRetornarErro_PrecoNegativo(decimal preco)
        {
            // Arrange
            var input = new CriarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: preco
            );

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Preco deve ser maior ou igual a zero.");
        }
    }
}