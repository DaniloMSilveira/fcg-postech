using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Interfaces.Services;

namespace FCG.Domain.Services
{
    public class JogoService : IJogoService
    {
        private readonly IJogoRepository _repository;

        public JogoService(IJogoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> JogoDuplicado(string nome, string desenvolvedora, DateTime? dataLancamento)
        {
            return await _repository.ExisteJogo(nome, desenvolvedora, dataLancamento);
        }
    }
}