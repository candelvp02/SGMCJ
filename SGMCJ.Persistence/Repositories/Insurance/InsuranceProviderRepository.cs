using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Insurance;
using SGMCJ.Domain.Repositories.Insurance;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Insurance
{
    public sealed class InsuranceProviderRepository : BaseRepository<InsuranceProvider>, IInsuranceProviderRepository
    {
        public InsuranceProviderRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<InsuranceProvider>> GetActiveProviderAsync()
            => await _dbSet.Where(i => i.IsActive).ToListAsync();

        public async Task<IEnumerable<InsuranceProvider>> GetPreferredProvidersAsync()
            => await _dbSet.Where(i => i.IsPreferred).ToListAsync();

        public async Task<IEnumerable<InsuranceProvider>> GetByNetworkTypeIdAsync(int networkTypeId)
            => await _dbSet.Where(i => i.NetworkTypeId == networkTypeId).ToListAsync();

        public async Task<InsuranceProvider?> GetByNameAsync(string name)
            => await _dbSet.FirstOrDefaultAsync(i => i.Name == name);

        public async Task<bool> ExistsAsync(int insuranceProviderId)
            => await _dbSet.AnyAsync(i => i.InsuranceProviderId == insuranceProviderId);

        Task IInsuranceProviderRepository.DeleteAsync(int insuranceProviderId)
        {
            return DeleteAsync(insuranceProviderId);
        }
    }
}