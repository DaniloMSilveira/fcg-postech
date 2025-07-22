using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace FCG.Application.DTOs.Queries
{
    public class PesquisarUsuariosQuery
    {
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public string? Filtro { get; set; }
    }
}