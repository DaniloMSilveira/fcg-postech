using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs.Jogos
{
    public class JogoItemListaOutput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        public JogoItemListaOutput(Guid id, string nome, decimal preco)
        {
            Id = id;
            Nome = nome;
            Preco = preco;
        }
    }
}