using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Repositories.System;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.System
{
    public sealed class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
            => await _dbSet.Where(r => r.IsActive).ToListAsync();

        public async Task<Role?> GetByNameAsync(string roleName)
            => await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);

        public async Task<bool> ExistsAsync(int roleId)
            => await _dbSet.AnyAsync(r => r.RoleId == roleId);

        public async Task<bool> ExistsByNameAsync(string roleName)
            => await _dbSet.AnyAsync(r => r.RoleName == roleName);

        Task IRoleRepository.DeleteAsync(int roleId)
        {
            return DeleteAsync(roleId);
        }
    }
}