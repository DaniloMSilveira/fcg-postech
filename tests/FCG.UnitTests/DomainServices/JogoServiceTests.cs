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
    public class JogoServiceTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly JogoService _service;

        public JogoServiceTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _service = new JogoService(_jogoRepositoryMock.Object);
        }

        [Fact]
        public async Task JogoDuplicado_DeveRetornarSucesso_QuandoJogoExistir()
        {
            // Arrange
            var jogo = CriarJogoFake();
            _jogoRepositoryMock
                .Setup(r => r.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.JogoDuplicado(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento);

            // Assert
            resultado.Should().BeTrue();
            _jogoRepositoryMock.Verify(r => r.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento), Times.Once);
        }

        [Fact]
        public async Task JogoDuplicado_DeveRetornarFalha_QuandoJogoNaoExistir()
        {
            // Arrange
            var jogo = CriarJogoFake();
            _jogoRepositoryMock
                .Setup(r => r.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento))
                .ReturnsAsync(false);

            // Act
            var resultado = await _service.JogoDuplicado(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento);

            // Assert
            resultado.Should().BeFalse();
            _jogoRepositoryMock.Verify(r => r.ExisteJogo(jogo.Nome, jogo.Desenvolvedora, jogo.DataLancamento), Times.Once);
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

        #endregion
    }
}