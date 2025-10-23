﻿//using SGMCJ.Application.Dto.System;
//using SGMCJ.Application.Dto.Users;
//using SGMCJ.Domain.Base;

//namespace SGMCJ.Application.Interfaces.Service
//{
//    public interface IUserService
//    {
//        // auth and registro rf3.1.1
//        Task<OperationResult<UserDto>> AuthenticateAsync(LoginDto dto);
//        Task<OperationResult<UserDto>> RegisterAsync(RegisterUserDto dto);
//        Task<OperationResult> RequestPasswordResetAsync(string email);
//        Task<OperationResult> ActivateAccountAsync(int userId);

//        //gestion de perfil
//        Task<OperationResult<UserDto>> UpdateProfileAsync(UpdateUserDto dto);
//        Task<OperationResult> ChangePasswordAsync(ChangePasswordDto dto);

//        //consultas
//        Task<OperationResult<UserDto>> GetByIdAsync(int id);
//        Task<OperationResult<UserDto>> GetByEmailAsync(string email);
//        Task<OperationResult<List<UserDto>>> GetByRoleAsync(short roleId);
//        Task<OperationResult<UserDto>> GetAllAsync();
//        Task<OperationResult<UserDto?>> DeleteAsync(int id);
//        Task<OperationResult<UserDto>> GetActiveUserAsync();
//        Task<OperationResult<UserDto?>> DeactivateAsync(int id);
//    }
//}

using SGMCJ.Application.Dto.System;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IUserService
    {
        // Auth y registro (RF3.1.1)
        Task<OperationResult<UserDto>> AuthenticateAsync(LoginDto dto);
        Task<OperationResult<UserDto>> RegisterAsync(RegisterUserDto dto);
        Task<OperationResult> RequestPasswordResetAsync(string email);
        Task<OperationResult> ActivateAccountAsync(int userId);

        // Gestión de perfil
        Task<OperationResult<UserDto>> UpdateProfileAsync(UpdateUserDto dto);
        Task<OperationResult> ChangePasswordAsync(ChangePasswordDto dto);

        // Consultas
        Task<OperationResult<UserDto>> GetByIdAsync(int id);
        Task<OperationResult<UserDto>> GetByEmailAsync(string email);
        Task<OperationResult<List<UserDto>>> GetByRoleAsync(short roleId);
        Task<OperationResult<List<UserDto>>> GetActiveUsersAsync();
        Task<OperationResult<List<UserDto>>> GetAllAsync();
        //Task<OperationResult<List<UserDto>>> GetActiveUserAsync();

        // CRUD
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult> DeactivateAsync(int id);
    }
}