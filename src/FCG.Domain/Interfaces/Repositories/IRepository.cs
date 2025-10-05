using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T?> ObterPorId(Guid id);
        Task<IEnumerable<T>> ObterTodos();
        Task Adicionar(T entity);
        void Atualizar(T entity);
        void Remover(T entity);
    }
}