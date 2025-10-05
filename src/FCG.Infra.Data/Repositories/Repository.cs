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
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly FCGDataContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FCGDataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> ObterPorId(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ObterTodos()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task Adicionar(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Atualizar(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remover(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}