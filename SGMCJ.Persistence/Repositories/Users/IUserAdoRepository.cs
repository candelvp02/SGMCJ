using SGMCJ.Application.Dto.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IUserAdoRepository
    {
        Task<List<UserDto>> ListActiveAsync();
        Task<List<UserDto>> SearchByNameAsync(string name);
        Task<List<UserDto>> ListByRoleAsync(int roleId);
        Task<UserDto?> GetByIdWithDetailsAsync(int userId);
        Task<UserDto?> GetByEmailAsync(string email);
        //Task<UserDto?> GetByIdentificationNumberAsync(string identificationNumber);
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
    }
}