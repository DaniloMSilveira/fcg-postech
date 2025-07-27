using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Repositories
{
    public class PromocaoRepository : Repository<Promocao>, IPromocaoRepository
    {
        public PromocaoRepository(FCGDataContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Promocao>, int)> Consultar(int pagina, int tamanhoPagina, decimal? precoMinimo, decimal? precoMaximo)
        {
            var query = _context.Promocoes
                .AsNoTracking()
                .Include(p => p.Jogo)
                .Where(p => p.Ativo == true
                    && (p.Preco >= precoMinimo || precoMinimo == null)
                    && (p.Preco <= precoMaximo || precoMaximo == null));

            var total = await query.CountAsync();
            var promocoes = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (promocoes, total);
        }

        public async Task<bool> ExistePromocao(Guid jogoId, DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Promocoes.AnyAsync(e =>
                e.JogoId == jogoId
                && e.DataInicio >= dataInicio
                && e.DataFim <= dataFim
                && e.Ativo == true);
        }
    }
}