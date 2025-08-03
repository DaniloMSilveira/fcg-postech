using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Domain.Interfaces.Services
{
    public interface IJogoService
    {
        Task<bool> JogoDuplicado(string nome, string desenvolvedora, DateTime? dataLancamento);
    }
}