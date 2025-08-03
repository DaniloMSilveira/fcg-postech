using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Application.DTOs.Outputs.Jogos
{
    public class JogoOutput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string? Desenvolvedora { get; set; }
        public DateTime? DataLancamento { get; set; }
        public decimal Preco { get; set; }

        public JogoOutput(
            Guid id,
            string nome,
            string? descricao,
            string? desenvolvedora,
            DateTime? dataLancamento,
            decimal preco)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Desenvolvedora = desenvolvedora;
            DataLancamento = dataLancamento;
            Preco = preco;
        }

        public static JogoOutput FromEntity(Jogo jogo)
        {
            return new JogoOutput(
                jogo.Id,
                jogo.Nome,
                jogo.Descricao,
                jogo.Desenvolvedora,
                jogo.DataLancamento,
                jogo.Preco);
        }
    }
}