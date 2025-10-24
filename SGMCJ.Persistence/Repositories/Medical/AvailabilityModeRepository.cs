﻿using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public sealed class AvailabilityModeRepository : BaseRepository<AvailabilityMode>, IAvailabilityModeRepository
    {
        public AvailabilityModeRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<AvailabilityMode>> GetActiveModesAsync()
            => await _dbSet.Where(a => a.IsActive).ToListAsync();

        public async Task<AvailabilityMode?> GetByNameAsync(string name)
            => await _dbSet.FirstOrDefaultAsync(a => a.AvailabilityMode1 == name);

        public async Task<bool> ExistsAsync(short availabilityModeId)
            => await _dbSet.AnyAsync(a => a.AvailabilityModeId == availabilityModeId);

        public async Task<AvailabilityMode?> GetByIdAsync(short availabilityModeId)
    => await _dbSet.FirstOrDefaultAsync(a => a.AvailabilityModeId == availabilityModeId);

        public async Task DeleteAsync(short availabilityModeId)
        {
            var entity = await GetByIdAsync(availabilityModeId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}