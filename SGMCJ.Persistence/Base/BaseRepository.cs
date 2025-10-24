using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Repositories;
using SGMCJ.Persistence.Context;
using System.Linq.Expressions;

namespace SGMCJ.Persistence.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly HealtSyncContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(HealtSyncContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        //Obtiene una entidad por su ID. Devuelve null si no se encuentra.
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Prepara una nueva entidad para ser agregada a la base de datos.
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        // Marca una entidad existente como modificada.
        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        // Marca una entidad para ser eliminada por su ID.
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null) // Se comprueba si la entidad existe antes de intentar borrarla.
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}