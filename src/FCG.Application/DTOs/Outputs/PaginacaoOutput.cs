using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs
{
    public class PaginacaoOutput<T>
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanhoPagina { get; set; }
        public List<T> Dados { get; set; } = new();
    }
}