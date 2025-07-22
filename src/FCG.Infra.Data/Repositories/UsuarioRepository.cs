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
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(FCGDataContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Usuario>, int)> Consultar(int pagina, int tamanhoPagina, string? filtro)
        {
            filtro = filtro?.ToLower();
            var query = _context.Usuarios.AsNoTracking();

            var total = await query.CountAsync();
            var usuarios = await query
                .Where(p =>
                    string.IsNullOrEmpty(filtro)
                    || (!string.IsNullOrEmpty(filtro) && (p.Nome.ToLower().Contains(filtro) || p.Email.ToLower().Contains(filtro)))
                )
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (usuarios, total);
        }

        public async Task<bool> ExisteUsuario(string email)
        {
            return await _context.Usuarios.AnyAsync(e => e.Email == email);
        }
    }
}