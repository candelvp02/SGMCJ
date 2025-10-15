using SGMCJ.Domain.Entities.Insurance;

namespace SGMCJ.Domain.Repositories.Insurance
{
    public interface IInsuranceProviderRepository
    {
        Task<InsuranceProvider?> GetByIdAsync(int insuranceProviderId);
        Task<IEnumerable<InsuranceProvider>> GetAllAsync();
        Task<InsuranceProvider> AddAsync(InsuranceProvider insuranceProvider);
        Task UpdateAsync(InsuranceProvider insuranceProvider);
        Task DeleteAsync(int insuranceProviderId);
        Task<IEnumerable<InsuranceProvider>> GetActiveProviderAsync();
        Task<IEnumerable<InsuranceProvider>> GetPreferredProvidersAsync();
        Task<IEnumerable<InsuranceProvider>> GetByNetworkTypeIdAsync(int networkTypeId);
        Task<InsuranceProvider?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(int insuranceProviderId);
    }
}