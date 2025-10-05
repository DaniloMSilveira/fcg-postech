using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces.Repositories
{
    public interface IPromocaoRepository : IRepository<Promocao>
    {
        Task<(IEnumerable<Promocao>, int)> Consultar(int pagina, int tamanhoPagina, decimal? precoMinimo, decimal? precoMaximo, bool? ativo);
        Task<bool> ExistePromocao(Guid jogoId, DateTime dataInicio, DateTime dataFim);
    }
}