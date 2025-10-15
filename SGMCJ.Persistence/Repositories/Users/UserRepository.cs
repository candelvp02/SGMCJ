using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(HealtSyncContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
        {
            return await _dbSet.Where(u => u.RoleId == roleId).ToListAsync();
        }

        public async Task<User?> GetByIdWithDetailsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.UserNavigation)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByEmailWithDetailsAsync(string email)
        {
            return await _dbSet
                .Include(u => u.UserNavigation)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        Task IUserRepository.DeleteAsync(int userId)
        {
            return DeleteAsync(userId);
        }

        public Task<bool> ExistsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}