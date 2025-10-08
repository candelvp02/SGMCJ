using SGMCJ.Domain.Base;
using System.Linq.Expressions;

namespace SGMCJ.Domain.Repositories
{
    public interface IBaseRepository<T> where T : AuditEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(int id);
    }
}