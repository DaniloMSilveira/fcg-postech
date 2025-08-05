using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FCG.Application.DTOs.Inputs.Autenticacao;
using FluentAssertions;

namespace FCG.UnitTests.Inputs.Autenticacao
{
    public class RegistrarUsuarioInputTests
    {
        private string _nomeValido;
        private string _emailValido;
        private string _senhaValida;

        public RegistrarUsuarioInputTests()
        {
            var faker = new Faker("pt_BR");
            _nomeValido = faker.Person.FullName;
            _emailValido = faker.Person.Email;
            _senhaValida = "Senha@123"; // Estática para validar regra de segurança
        }

        [Fact]
        public void IsValid_DeveRetornarSucesso_DadosValidos()
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nomeValido, _emailValido, _senhaValida);

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
        public void IsValid_DeveRetornarErro_NomeObrigatorio(string nome)
        {
            // Arrange
            var input = new RegistrarUsuarioInput(nome, _emailValido, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome é um campo obrigatório.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_NomeTamanhoMaximo()
        {
            // Arrange
            var nome = _nomeValido + new string('a', 256);
            var input = new RegistrarUsuarioInput(nome, _emailValido, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Nome deve ter até 256 caracteres.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_EmailObrigatorio(string email)
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nomeValido, email, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email é um campo obrigatório.");
        }

        [Theory]
        [InlineData("teste")]
        [InlineData("teste.com")]
        [InlineData("teste@")]
        [InlineData("teste@teste")]
        [InlineData("@teste")]
        [InlineData("@teste.com")]
        public void IsValid_DeveRetornarErro_EmailInvalido(string email)
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nomeValido, email, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email inválido.");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_EmailTamanhoMaximo()
        {
            // Arrange
            var email = _emailValido + new string('a', 100);
            var input = new RegistrarUsuarioInput(_nomeValido, email, _senhaValida);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email deve ter até 100 caracteres.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_DeveRetornarErro_SenhaObrigatorio(string senha)
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nomeValido, _emailValido, senha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Senha é um campo obrigatório.");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345678")]
        [InlineData("senha@123")]
        [InlineData("Senha123")]
        public void IsValid_DeveRetornarErro_SenhaInvalida(string senha)
        {
            // Arrange
            var input = new RegistrarUsuarioInput(_nomeValido, _emailValido, senha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(
                e => e.ErrorMessage == "Senha deve se enquadrar nos requisitos de segurança (mínimo 8 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caracter especial).");
        }

        [Fact]
        public void IsValid_DeveRetornarErro_SenhaTamanhoMaximo()
        {
            // Arrange
            var senha = _senhaValida + new string('a', 40);
            var input = new RegistrarUsuarioInput(_nomeValido, _emailValido, senha);

            // Act
            var resultado = input.IsValid();

            // Assert
            resultado.Should().BeFalse();
            input.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == "Senha deve ter até 40 caracteres.");
        }
    }
}