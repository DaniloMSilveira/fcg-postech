using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Outputs.Jogos;

namespace FCG.Application.DTOs.Outputs.Promocoes
{
    public class PromocaoItemListaOutput
    {
        public Guid Id { get; set; }
        public Guid JogoId { get; set; }
        public string JogoNome { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Ativo { get; set; }
    }
}