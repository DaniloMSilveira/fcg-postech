using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Data.Contexts;

namespace FCG.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FCGDataContext _context;
        public IUsuarioRepository UsuarioRepository { get; }

        public UnitOfWork(FCGDataContext context, IUsuarioRepository usuarioRepository)
        {
            _context = context;
            UsuarioRepository = usuarioRepository;
        }

        public async Task<bool> Commit()
        {
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}