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
    public class JogoRepository : Repository<Jogo>, IJogoRepository
    {
        public JogoRepository(FCGDataContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Jogo>, int)> Consultar(int pagina, int tamanhoPagina, string? filtro)
        {
            filtro = filtro?.ToLower();
            var query = _context.Jogos
                .AsNoTracking()
                .Include(p => p.Promocoes)
                .Where(p =>
                    string.IsNullOrEmpty(filtro)
                    || (!string.IsNullOrEmpty(filtro) && p.Nome.ToLower().Contains(filtro))
                );

            var total = await query.CountAsync();
            var jogos = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (jogos, total);
        }

        public async Task<bool> ExisteJogo(string nome, string? desenvolvedora, DateTime? dataLancamento)
        {
            return await _context.Jogos.AnyAsync(e =>
                e.Nome.ToLower() == nome.ToLower()
                && e.Desenvolvedora == desenvolvedora
                && e.DataLancamento == dataLancamento);
        }

        public async Task<Jogo> ObterJogoPromocoes(Guid id)
        {
            return await _context.Jogos
                .AsNoTracking()
                .Include(p => p.Promocoes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}