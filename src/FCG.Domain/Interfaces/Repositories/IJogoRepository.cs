using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces.Repositories
{
    public interface IJogoRepository : IRepository<Jogo>
    {
        Task<(IEnumerable<Jogo>, int)> Consultar(int pagina, int tamanhoPagina, string? filtro);
        Task<bool> ExisteJogo(string nome, string? desenvolvedora, DateTime? dataLancamento);
        Task<Jogo> ObterJogoPromocoes(Guid id);
    }
}