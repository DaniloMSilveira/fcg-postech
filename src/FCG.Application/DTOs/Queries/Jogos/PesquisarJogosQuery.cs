using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Queries.Jogos
{
    public class PesquisarJogosQuery
    {
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public string? Filtro { get; set; }
    }
}