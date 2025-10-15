using SGMCJ.Domain.Entities.System;

namespace SGMCJ.Domain.Repositories.System
{
    public interface IStatusRepository
    {
        Task<Status?> GetByIdAsync(int statusId);
        Task<IEnumerable<Status>> GetAllAsync();
        Task<Status> AddAsync(Status status);
        Task UpdateAsync(Status status);
        Task DeleteAsync(int statusId);
        Task<Status?> GetByNameAsync(string statusName);
        Task<bool> ExistsAsync(int statusId);
        Task<bool> ExistsByNameAsync(string statusName);
    }
}