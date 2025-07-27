using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Outputs.Jogos;

namespace FCG.Application.DTOs.Outputs.Promocoes
{
    public class PromocaoOutput
    {
        public Guid Id { get; set; }
        public Guid JogoId { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}