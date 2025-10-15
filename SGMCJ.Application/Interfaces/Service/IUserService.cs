using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IUserService
    {
            Task<OperationResult<List<UserDto>>> GetAllAsync();
            Task<OperationResult<UserDto>> GetByIdAsync(int id);
            Task<OperationResult<UserDto>> CreateAsync(RegisterUserDto userDto);
            Task<OperationResult<UserDto>> UpdateAsync(UpdateUserDto userDto);
            Task<OperationResult> DeleteAsync(int id);
            Task<OperationResult<List<UserDto>>> GetActiveAsync();
            Task<OperationResult<List<UserDto>>> GetByRoleAsync(int roleId);
            Task<OperationResult<UserDto>> GetByEmailAsync(string email);
            Task<OperationResult<UserDto>> GetByUsernameAsync(string username);
            Task<OperationResult<bool>> ValidateCredentialsAsync(string username, string password);
            Task<OperationResult> UpdateLastLoginAsync(int userId);
            Task<OperationResult> ActivateAsync(int userId);
            Task<OperationResult> DeactivateAsync(int userId);
        }
    }