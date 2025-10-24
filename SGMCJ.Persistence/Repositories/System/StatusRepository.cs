using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Repositories.System;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.System
{
    public sealed class StatusRepository : BaseRepository<Status>, IStatusRepository
    {
        public StatusRepository(HealtSyncContext context) : base(context) { }

        public async Task<Status?> GetByNameAsync(string statusName)
            => await _dbSet.FirstOrDefaultAsync(s => s.StatusName == statusName);

        public async Task<bool> ExistsAsync(int statusId)
            => await _dbSet.AnyAsync(s => s.StatusId == statusId);

        public async Task<bool> ExistsByNameAsync(string statusName)
            => await _dbSet.AnyAsync(s => s.StatusName == statusName);

        Task IStatusRepository.DeleteAsync(int statusId)
        {
            return DeleteAsync(statusId);
        }

        public override async Task<Status?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}