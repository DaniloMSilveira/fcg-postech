using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Promocoes;
using FCG.Application.Services;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace FCG.UnitTests.ApplicationServices
{
    public class PromocaoAppServiceTests
    {
        private readonly Mock<IPromocaoRepository> _repositoryMock;
        private readonly Mock<IPromocaoService> _serviceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PromocaoAppService _appService;
        private readonly Faker _faker;

        public PromocaoAppServiceTests()
        {
            _repositoryMock = new Mock<IPromocaoRepository>();
            _serviceMock = new Mock<IPromocaoService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _faker = new Faker("pt_BR");

            _appService = new PromocaoAppService(
                _repositoryMock.Object,
                _serviceMock.Object,
                _unitOfWorkMock.Object);

            _unitOfWorkMock.SetupGet(u => u.PromocaoRepository).Returns(_repositoryMock.Object);
        }
        
        [Fact]
        public async Task Criar_DeveRetornarSucesso_QuandoInputValido()
        {
            // Arrange
            var input = new CriarPromocaoInput(Guid.NewGuid(), 10.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _serviceMock.Setup(s => s.ValidarNovaPromocao(
                input.JogoId, 
                input.Preco, 
                input.DataInicio, 
                input.DataFim))
                .ReturnsAsync((true, null));
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        
            // Act
            var result = await _appService.Criar(input);
        
            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            _repositoryMock.Verify(r => r.Adicionar(It.IsAny<Promocao>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Criar_DeveRetornarErro_QuandoInputInvalido()
        {
            // Arrange
            var input = new CriarPromocaoInput(Guid.Empty, -10.0m, DateTime.Now, DateTime.Now.AddDays(-1));
        
            // Act
            var result = await _appService.Criar(input);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _repositoryMock.Verify(r => r.Adicionar(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Criar_DeveRetornarErro_QuandoValidacaoDoDominioFalha()
        {
            // Arrange
            var input = new CriarPromocaoInput(Guid.NewGuid(), 10.0m, DateTime.Now, DateTime.Now.AddDays(10));
            var erroValidacao = "Erro de validação do domínio.";
            _serviceMock.Setup(s => s.ValidarNovaPromocao(
                It.IsAny<Guid>(), 
                It.IsAny<decimal>(), 
                It.IsAny<DateTime>(), 
                It.IsAny<DateTime>()))
                .ReturnsAsync((false, erroValidacao));
        
            // Act
            var result = await _appService.Criar(input);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(erroValidacao);
            _repositoryMock.Verify(r => r.Adicionar(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Criar_DeveLancarExcecao_QuandoUnitOfWorkCommitFalha()
        {
            // Arrange
            var input = new CriarPromocaoInput(Guid.NewGuid(), 10.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _serviceMock.Setup(s => s.ValidarNovaPromocao(
                It.IsAny<Guid>(), 
                It.IsAny<decimal>(), 
                It.IsAny<DateTime>(), 
                It.IsAny<DateTime>()))
                .ReturnsAsync((true, null));
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        
            // Act
            Func<Task> act = async () => await _appService.Criar(input);
        
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao criar promoção no domínio.");
            _repositoryMock.Verify(r => r.Adicionar(It.IsAny<Promocao>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Alterar_DeveRetornarSucesso_QuandoInputEPromocaoValidos()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocaoExistente = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(5));
            var input = new AlterarPromocaoInput(15.0m, promocaoExistente.DataInicio, promocaoExistente.DataFim);
            input.PreencherId(promocaoId);

            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocaoExistente);
            _serviceMock.Setup(s => s.ValidarAlteracaoPromocao(promocaoExistente, input.Preco)).ReturnsAsync((true, null));
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        
            // Act
            var result = await _appService.Alterar(input);
        
            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Preco.Should().Be(input.Preco);
            _repositoryMock.Verify(r => r.ObterPorId(promocaoId), Times.Once);
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Promocao>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Alterar_DeveRetornarErro_QuandoInputInvalido()
        {
            // Arrange
            var input = new AlterarPromocaoInput(-10.0m, DateTime.Now, DateTime.Now.AddDays(-1));
            input.PreencherId(Guid.Empty);
        
            // Act
            var result = await _appService.Alterar(input);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _repositoryMock.Verify(r => r.ObterPorId(It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Alterar_DeveRetornarErro_QuandoPromocaoNaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var input = new AlterarPromocaoInput(15.0m, DateTime.Now, DateTime.Now.AddDays(10));
            input.PreencherId(promocaoId);
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync((Promocao)null);
        
            // Act
            var result = await _appService.Alterar(input);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Promoção não encontrada.");
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Alterar_DeveRetornarErro_QuandoValidacaoDoDominioFalha()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocaoExistente = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            var input = new AlterarPromocaoInput(15.0m, promocaoExistente.DataInicio, promocaoExistente.DataFim);
            input.PreencherId(promocaoId);
            var erroValidacao = "Erro de validação de alteração do domínio.";

            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocaoExistente);
            _serviceMock.Setup(s => s.ValidarAlteracaoPromocao(promocaoExistente, input.Preco)).ReturnsAsync((false, erroValidacao));
        
            // Act
            var result = await _appService.Alterar(input);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(erroValidacao);
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Alterar_DeveLancarExcecao_QuandoUnitOfWorkCommitFalha()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocaoExistente = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            var input = new AlterarPromocaoInput(15.0m, promocaoExistente.DataInicio, promocaoExistente.DataFim);
            input.PreencherId(promocaoId);
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocaoExistente);
            _serviceMock.Setup(s => s.ValidarAlteracaoPromocao(promocaoExistente, input.Preco)).ReturnsAsync((true, null));
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        
            // Act
            Func<Task> act = async () => await _appService.Alterar(input);
        
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao atualizar promoção no domínio.");
            _repositoryMock.Verify(r => r.Atualizar(It.IsAny<Promocao>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Ativar_DeveRetornarSucesso_QuandoPromocaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            promocao.Inativar();
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        
            // Act
            var result = await _appService.Ativar(promocaoId);
        
            // Assert
            result.Success.Should().BeTrue();
            promocao.Ativo.Should().BeTrue();
            _repositoryMock.Verify(r => r.Atualizar(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Ativar_DeveRetornarErro_QuandoPromocaoNaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync((Promocao)null);
        
            // Act
            var result = await _appService.Ativar(promocaoId);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Promoção não encontrada.");
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Ativar_DeveLancarExcecao_QuandoUnitOfWorkCommitFalha()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            promocao.Inativar();
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        
            // Act
            Func<Task> act = async () => await _appService.Ativar(promocaoId);
        
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao ativar promoção no domínio.");
            _repositoryMock.Verify(r => r.Atualizar(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Inativar_DeveRetornarSucesso_QuandoPromocaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        
            // Act
            var result = await _appService.Inativar(promocaoId);
        
            // Assert
            result.Success.Should().BeTrue();
            promocao.Ativo.Should().BeFalse();
            _repositoryMock.Verify(r => r.Atualizar(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Inativar_DeveRetornarErro_QuandoPromocaoNaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync((Promocao)null);
        
            // Act
            var result = await _appService.Inativar(promocaoId);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Promoção não encontrado.");
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Inativar_DeveLancarExcecao_QuandoUnitOfWorkCommitFalha()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        
            // Act
            Func<Task> act = async () => await _appService.Inativar(promocaoId);
        
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao inativar promoção no domínio.");
            _repositoryMock.Verify(r => r.Atualizar(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Remover_DeveRetornarSucesso_QuandoPromocaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        
            // Act
            var result = await _appService.Remover(promocaoId);
        
            // Assert
            result.Success.Should().BeTrue();
            _repositoryMock.Verify(r => r.Remover(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
        
        [Fact]
        public async Task Remover_DeveRetornarErro_QuandoPromocaoNaoEncontrada()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync((Promocao)null);
        
            // Act
            var result = await _appService.Remover(promocaoId);
        
            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Promoção não encontrado.");
            _repositoryMock.Verify(r => r.Remover(It.IsAny<Promocao>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
        
        [Fact]
        public async Task Remover_DeveLancarExcecao_QuandoUnitOfWorkCommitFalha()
        {
            // Arrange
            var promocaoId = Guid.NewGuid();
            var promocao = new Promocao(Guid.NewGuid(), 20.0m, DateTime.Now, DateTime.Now.AddDays(10));
            _repositoryMock.Setup(r => r.ObterPorId(promocaoId)).ReturnsAsync(promocao);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);
        
            // Act
            Func<Task> act = async () => await _appService.Remover(promocaoId);
        
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao remover promoção no domínio.");
            _repositoryMock.Verify(r => r.Remover(promocao), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
    }
}