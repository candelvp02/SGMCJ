using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(HealtSyncContext context) : base(context) { }

        public Task<bool> ExistsAsync(int employeeId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _dbSet.Where(e => e.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByJobTitleAsync(string jobTitle)
        {
            return await _dbSet.Where(e => e.JobTitle == jobTitle && e.IsActive).ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            return await _dbSet.FindAsync(employeeId).ConfigureAwait(false);
        }

        Task IEmployeeRepository.DeleteAsync(int employeeId)
        {
            return DeleteAsync(employeeId);
        }
    }
}
