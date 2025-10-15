using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public sealed class SpecialtyRepository : BaseRepository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<Specialty>> GetActiveSpecialtiesAsync()
            => await _dbSet.Where(s => s.IsActive).ToListAsync();

        public async Task<Specialty?> GetNameAsync(string specialtyName)
            => await _dbSet.FirstOrDefaultAsync(s => s.SpecialtyName == specialtyName);

        public async Task<bool> ExistsAsync(short specialtyId)
            => await _dbSet.AnyAsync(s => s.SpecialtyId == specialtyId);

        public async Task<bool> ExistsByNameAsync(string specialtyName)
            => await _dbSet.AnyAsync(s => s.SpecialtyName == specialtyName);

        public Task<Specialty?> GetByIdAsync(short specialtyId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(short specialtyId)
        {
            throw new NotImplementedException();
        }
    }
}