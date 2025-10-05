using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Domain.Entities
{
    public class Jogo : IEntity, IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public string? Desenvolvedora { get; private set; }
        public DateTime? DataLancamento { get; private set; }
        public decimal Preco { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? ModificadoEm { get; private set; }

        private readonly List<UsuarioJogo> _usuarios = new();
        public IReadOnlyCollection<UsuarioJogo> Usuarios => _usuarios.AsReadOnly();

        private readonly List<Promocao> _promocoes = new();
        public IReadOnlyCollection<Promocao> Promocoes => _promocoes.AsReadOnly();

        protected Jogo() { }

        public Jogo(string nome,
            string? descricao,
            string? desenvolvedora,
            DateTime? dataLancamento,
            decimal preco)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            Desenvolvedora = desenvolvedora;
            DataLancamento = dataLancamento;
            Preco = preco;
            Ativo = true;
            CriadoEm = DateTime.Now;
        }

        public void Alterar(string nome,
            string? descricao,
            string? desenvolvedora,
            DateTime? dataLancamento,
            decimal preco)
        {
            Nome = nome;
            Descricao = descricao;
            Desenvolvedora = desenvolvedora;
            DataLancamento = dataLancamento;
            Preco = preco;
            ModificadoEm = DateTime.Now;
        }

        public void Ativar()
        {
            Ativo = true;
            ModificadoEm = DateTime.Now;
        }

        public void Inativar()
        {
            Ativo = false;
            ModificadoEm = DateTime.Now;
        }
    }
}