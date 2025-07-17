using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Application.DTOs.Outputs
{
    public class UsuarioOutput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public UsuarioOutput(Guid id, string nome, string email)
        {
            Id = id;
            Nome = nome;
            Email = email;
        }
    }
}