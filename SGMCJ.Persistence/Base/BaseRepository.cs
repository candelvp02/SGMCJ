using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Base;
using SGMCJ.Persistence.Context;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SGMCJ.Persistence.Base
{
    public abstract class BaseRepository<T> where T : AuditEntity
    {
        protected readonly SGMCJDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(SGMCJDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.EstaEliminado).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.EstaEliminado);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.FechaCreacion = DateTime.Now;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            entity.FechaModificacion = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            entity.EstaEliminado = true;
            entity.FechaModificacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(e => !e.EstaEliminado).ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id && !e.EstaEliminado);
        }
    }
}