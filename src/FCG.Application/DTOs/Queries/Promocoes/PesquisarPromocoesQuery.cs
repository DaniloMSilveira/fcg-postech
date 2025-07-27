using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Queries.Promocoes
{
    public class PesquisarPromocoesQuery
    {
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
    }
}