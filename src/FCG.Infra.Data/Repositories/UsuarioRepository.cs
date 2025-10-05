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
            var query = _context.Usuarios
                .AsNoTracking()
                .Where(p =>
                    string.IsNullOrEmpty(filtro)
                    || (!string.IsNullOrEmpty(filtro) && (p.Nome.ToLower().Contains(filtro) || p.Email.ToLower().Contains(filtro)))
                );

            var total = await query.CountAsync();
            var usuarios = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (usuarios, total);
        }

        public async Task<Usuario?> ObterUsuarioPorEmail(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Include(p => p.Jogos)
                .ThenInclude(p => p.Jogo)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<bool> ExisteUsuario(string email)
        {
            return await _context.Usuarios.AnyAsync(e => e.Email == email);
        }

        #region Biblioteca jogos

        public async Task AdicionarJogoBiblioteca(UsuarioJogo jogo)
        {
            await _context.UsuarioJogos.AddAsync(jogo);
        }

        #endregion
    }
}