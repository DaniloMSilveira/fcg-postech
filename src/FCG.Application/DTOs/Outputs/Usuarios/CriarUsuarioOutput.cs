using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace FCG.Application.DTOs.Outputs.Usuarios
{
    public class CriarUsuarioOutput
    {
        public Guid Id { get; set; }

        public CriarUsuarioOutput(Guid id)
        {
            Id = id;
        }
    }
}