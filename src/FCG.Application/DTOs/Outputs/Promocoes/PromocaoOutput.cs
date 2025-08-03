using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Application.DTOs.Outputs.Jogos;
using FCG.Domain.Entities;

namespace FCG.Application.DTOs.Outputs.Promocoes
{
    public class PromocaoOutput
    {
        public Guid Id { get; set; }
        public Guid JogoId { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public static PromocaoOutput FromEntity(Promocao promocao)
        {
            return new PromocaoOutput
            {
                Id = promocao.Id,
                JogoId = promocao.JogoId,
                Preco = promocao.Preco,
                DataInicio = promocao.DataInicio,
                DataFim = promocao.DataFim
            };
        }
    }
}