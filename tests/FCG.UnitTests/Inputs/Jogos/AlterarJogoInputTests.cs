using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Jogos;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Jogos
{
    public class AlterarJogoInputTests
    {
        private Guid _idValido;
        private string _nomeValido;
        private string _descricaoValida;
        private string _desenvolvedoraValida;
        private DateTime _dataLancamentoValida;
        private decimal _precoValido;

        public AlterarJogoInputTests()
        {
            var faker = new Faker("pt_BR");
            _idValido = Guid.NewGuid();
            _nomeValido = faker.Lorem.Sentence(3);
            _descricaoValida = faker.Lorem.Sentence(10, 2);
            _desenvolvedoraValida = faker.Lorem.Sentence(3);
            _dataLancamentoValida = DateTime.Now.Date;
            _precoValido = faker.Random.Number(0, 100);
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
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
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
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
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_QuandoNomeVazio(string nome)
        {
            // Arrange
            var input = new AlterarJogoInput(
                nome: nome,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome é um campo obrigatório.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoNomeAtingirTamanhoMaximo()
        {
            // Arrange
            var nome = _nomeValido + new string('a', 256);
            var input = new AlterarJogoInput(
                nome: nome,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome deve ter até 256 caracteres.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoDescricaoAtingirTamanhoMaximo()
        {
            // Arrange
            var descricao = _descricaoValida + new string('a', 1024);
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: descricao,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Descricao deve ter até 1024 caracteres.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_QuandoDesenvolvedoraAtingirTamanhoMaximo()
        {
            // Arrange
            var desenvolvedora = _desenvolvedoraValida + new string('a', 256);
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: desenvolvedora,
                dataLancamento: _dataLancamentoValida,
                preco: _precoValido
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Desenvolvedora deve ter até 256 caracteres.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void IsValid_DeveRetornarErro_QuandoPrecoNegativo(decimal preco)
        {
            // Arrange
            var input = new AlterarJogoInput(
                nome: _nomeValido,
                descricao: _descricaoValida,
                desenvolvedora: _desenvolvedoraValida,
                dataLancamento: _dataLancamentoValida,
                preco: preco
            );
            input.PreencherId(_idValido);

            // Act
            var result = input.IsValid();

            // Assert
            result.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Preco deve ser maior ou igual a zero.");
        }
    }
}