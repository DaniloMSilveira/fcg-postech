using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace FCG.Application.DTOs.Outputs
{
    public class CadastrarUsuarioOutput
    {
        public Guid Id { get; set; }

        public CadastrarUsuarioOutput(Guid id)
        {
            Id = id;
        }
    }
}