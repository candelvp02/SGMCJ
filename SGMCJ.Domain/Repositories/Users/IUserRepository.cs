﻿using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailAndPasswordAsync(string email, string password);
        Task<bool> ExistsAsync(int userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
        Task<User?> GetByIdWithDetailsAsync(int userId);
        Task<User?> GetByEmailWithDetailsAsync(string email);
    }
}