using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface IAvailabilityModeRepository
    {
        Task<AvailabilityMode?> GetByIdAsync(short availabilityModeId);
        Task<IEnumerable<AvailabilityMode>> GetAllAsync();
        Task<AvailabilityMode> AddAsync(AvailabilityMode availabilityMode);
        Task UpdateAsync(AvailabilityMode availabilityMode);
        Task DeleteAsync(short availabilityModeId);
        Task<IEnumerable<AvailabilityMode>> GetActiveModesAsync();
        Task<AvailabilityMode?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(short availabilityModeId);
    }
}