using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Common;
using System.Data;

namespace SGMCJ.Persistence.Ado.Users
{
    public class UserAdoRepository : IUserAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<UserAdoRepository> _logger;

        public UserAdoRepository(StoredProcedureExecutor sp, ILogger<UserAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<UserDto>> ListActiveAsync()
        {
            var users = new List<UserDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("users.usp_User_ListActive");
                while (await r.ReadAsync())
                {
                    users.Add(new UserDto
                    {
                        UserId = r.GetInt32("UserID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = r.IsDBNull("DateOfBirth") ? null : DateOnly.FromDateTime(r.GetDateTime("DateOfBirth")),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        RoleId = r.IsDBNull("RoleID") ? null : r.GetInt32("RoleID"),
                        RoleName = r.IsDBNull("RoleName") ? string.Empty : r.GetString("RoleName"),
                        IsActive = r.GetBoolean("IsActive"),
                        CreatedAt = r.GetDateTime("CreatedAt")
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing active users");
                return new List<UserDto>();
            }
        }

        public async Task<List<UserDto>> ListByRoleAsync(string roleName)
        {
            var users = new List<UserDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_ListByRole",
                    ("@RoleName", roleName)
                );
                while (await r.ReadAsync())
                {
                    users.Add(new UserDto
                    {
                        UserId = r.GetInt32("UserID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = r.IsDBNull("DateOfBirth") ? null : DateOnly.FromDateTime(r.GetDateTime("DateOfBirth")),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        RoleId = r.IsDBNull("RoleID") ? null : r.GetInt32("RoleID"),
                        RoleName = r.IsDBNull("RoleName") ? string.Empty : r.GetString("RoleName"),
                        IsActive = r.GetBoolean("IsActive"),
                        CreatedAt = r.GetDateTime("CreatedAt")
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users by role: {RoleName}", roleName);
                return new List<UserDto>();
            }
        }

        public async Task<bool> UpdateLastAccessAsync(int userId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_User_UpdateLastAccess",
                    ("@UserID", userId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last access for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ActivateAsync(int userId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_User_Activate",
                    ("@UserID", userId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> DeactivateAsync(int userId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_User_Deactivate",
                    ("@UserID", userId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return false;
            }
        }

        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_GetById",
                    ("@UserID", userId)
                );
                if (await r.ReadAsync())
                {
                    return new UserDto
                    {
                        UserId = r.GetInt32("UserID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = r.IsDBNull("DateOfBirth") ? null : DateOnly.FromDateTime(r.GetDateTime("DateOfBirth")),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        RoleId = r.IsDBNull("RoleID") ? null : r.GetInt32("RoleID"),
                        RoleName = r.IsDBNull("RoleName") ? string.Empty : r.GetString("RoleName"),
                        IsActive = r.GetBoolean("IsActive"),
                        CreatedAt = r.GetDateTime("CreatedAt")
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
                return null;
            }
        }
    }
}