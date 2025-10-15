using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Insurance;
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
            _dbSet = _context.Set<T>();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync() // para obtener todas las entidades
        {
            return await _dbSet.ToListAsync();
        }
        public virtual async Task<T> GetByIdAsync(int id) // para obtener una entidad por su id
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} with id {id} was not found.");
            return entity;
        }
        public virtual async Task<T> AddAsync(T entity) // para agregar una nueva entidad
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task UpdateAsync(T entity) // para actualizar una entidad existente
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        public virtual async Task<bool> DeleteAsync(int id) // para eliminar una entidad por su id
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) // para buscar entidades que cumplan con una condicion (where)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) // para verificar si existe alguna entidad que cumpla con una condicion
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}