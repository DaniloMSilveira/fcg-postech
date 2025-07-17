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

        public async Task<bool> ExisteUsuario(string email)
        {
            return await _context.Usuarios.AnyAsync(e => e.Email == email);
        }
    }
}