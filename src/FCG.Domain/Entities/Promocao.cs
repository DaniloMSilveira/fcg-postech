using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Domain.Entities
{
    public class Promocao : IEntity
    {
        public Guid Id { get; private set; }
        public Guid JogoId { get; private set; }
        public virtual Jogo Jogo { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? ModificadoEm { get; private set; }

        protected Promocao() { }

        public Promocao(Guid jogoId,
            decimal preco,
            DateTime dataInicio,
            DateTime dataFim)
        {
            Id = Guid.NewGuid();
            JogoId = jogoId;
            Preco = preco;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Ativo = true;
            CriadoEm = DateTime.Now;
        }

        public void Alterar(decimal preco,
            DateTime dataInicio,
            DateTime dataFim)
        {
            Preco = preco;
            DataInicio = dataInicio;
            DataFim = dataFim;
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