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
        public decimal PrecoOriginal { get; set; }
        public decimal PrecoFinal { get; set; }
        public bool Ativo { get; set; }
    }
}