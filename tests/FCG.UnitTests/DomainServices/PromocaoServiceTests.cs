using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Services;
using FluentAssertions;
using Moq;

namespace FCG.UnitTests.DomainServices
{
    public class PromocaoServiceTests
    {
        private readonly Mock<IPromocaoRepository> _promocaoRepositoryMock;
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly PromocaoService _service;

        public PromocaoServiceTests()
        {
            _promocaoRepositoryMock = new Mock<IPromocaoRepository>();
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _service = new PromocaoService(_promocaoRepositoryMock.Object, _jogoRepositoryMock.Object);
        }

        [Fact]
        public async Task ValidarNovaPromocao_DeveRetornarErro_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);
            _jogoRepositoryMock
                .Setup(r => r.ObterPorId(jogo.Id))
                .ReturnsAsync((Jogo?)null);

            // Act
            var (sucesso, erro) = await _service.ValidarNovaPromocao(
                promocao.JogoId,
                promocao.Preco,
                promocao.DataInicio,
                promocao.DataFim
            );

            // Assert
            sucesso.Should().BeFalse();
            erro.Should().Be("Jogo não encontrado.");
        }

        [Fact]
        public async Task ValidarNovaPromocao_DeveRetornarErro_QuandoPrecoMaiorOuIgualAoJogo()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);
            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);

            // Act
            var (sucesso, erro) = await _service.ValidarNovaPromocao(
                promocao.JogoId,
                jogo.Preco + 5,
                promocao.DataInicio,
                promocao.DataFim
            );

            // Assert
            sucesso.Should().BeFalse();
            erro.Should().Be("Preço da promoção deve ser menor que o preço do jogo.");
        }

        [Fact]
        public async Task ValidarNovaPromocao_DeveRetornarErro_QuandoPromocaoJaExiste()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);
            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
            _promocaoRepositoryMock.Setup(r => r.ExistePromocao(promocao.JogoId, promocao.DataInicio, promocao.DataFim)).ReturnsAsync(true);

            // Act
            var (sucesso, erro) = await _service.ValidarNovaPromocao(
                promocao.JogoId,
                promocao.Preco,
                promocao.DataInicio,
                promocao.DataFim
            );

            // Assert
            sucesso.Should().BeFalse();
            erro.Should().Be("Já existe uma promoção ativa para este jogo.");
        }

        [Fact]
        public async Task ValidarNovaPromocao_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);
            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);
            _promocaoRepositoryMock.Setup(r => r.ExistePromocao(promocao.JogoId, promocao.DataInicio, promocao.DataFim)).ReturnsAsync(false);

            // Act
            var (sucesso, erro) = await _service.ValidarNovaPromocao(
                promocao.JogoId,
                promocao.Preco,
                promocao.DataInicio,
                promocao.DataFim
            );

            // Assert
            sucesso.Should().BeTrue();
            erro.Should().BeNull();
        }

        [Fact]
        public async Task ValidarAlteracaoPromocao_DeveRetornarErro_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var promocao = CriarPromocaoFake(Guid.NewGuid());

            _jogoRepositoryMock.Setup(r => r.ObterPorId(promocao.JogoId)).ReturnsAsync((Jogo?)null);

            // Act
            var (sucesso, erro) = await _service.ValidarAlteracaoPromocao(promocao, promocao.Preco + 5);

            // Assert
            sucesso.Should().BeFalse();
            erro.Should().Be("Jogo não encontrado.");
        }

        [Fact]
        public async Task ValidarAlteracaoPromocao_DeveRetornarErro_QuandoNovoPrecoMaiorOuIgual()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);

            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);

            // Act
            var (sucesso, erro) = await _service.ValidarAlteracaoPromocao(promocao, jogo.Preco + 5);

            // Assert
            sucesso.Should().BeFalse();
            erro.Should().Be("Preço da promoção deve ser menor que o preço do jogo.");
        }

        [Fact]
        public async Task ValidarAlteracaoPromocao_DeveRetornarSucesso_QuandoDadosValidos()
        {
            // Arrange
            var jogo = CriarJogoFake();
            var promocao = CriarPromocaoFake(jogo.Id);

            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);

            // Act
            var (sucesso, erro) = await _service.ValidarAlteracaoPromocao(promocao, promocao.Preco + 5);

            // Assert
            sucesso.Should().BeTrue();
            erro.Should().BeNull();
        }

        #region PRIVATE

        private Jogo CriarJogoFake()
        {
            var faker = new Faker("pt_BR");
            return new Jogo(
                faker.Lorem.Sentence(2),
                faker.Lorem.Sentence(5),
                faker.Company.CompanyName(),
                DateTime.Today,
                faker.Random.Decimal(60, 150)
            );
        }

        private Promocao CriarPromocaoFake(Guid jogoId)
        {
            var faker = new Faker("pt_BR");
            return new Promocao(
                jogoId,
                faker.Random.Number(0, 50),
                DateTime.Today,
                DateTime.Today.AddDays(5)
            );
        }

        #endregion
    }
}