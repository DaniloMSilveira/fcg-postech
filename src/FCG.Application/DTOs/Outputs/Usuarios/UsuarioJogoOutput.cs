using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs.Usuarios
{
    public class UsuarioJogoOutput
    {
        public Guid Id { get; set; }
        public Guid JogoId { get; set; }
        public string JogoNome { get; set; }
    }
}