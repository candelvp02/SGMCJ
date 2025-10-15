using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepo,
            ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<OperationResult<List<UserDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<UserDto>>();
            try
            {
                var users = await _userRepo.GetAllAsync();
                result.Datos = users.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Usuarios obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuarios";
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                var user = await _userRepo.GetByIdWithDetailsAsync(id);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario";
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> CreateAsync(RegisterUserDto dto)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Los datos del usuario son requeridos";
                    result.Errores.Add("Los datos del usuario son requeridos");
                    return result;
                }

                // Verificar si el email ya existe
                var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe un usuario con ese email";
                    result.Errores.Add("Ya existe un usuario con ese email");
                    return result;
                }

                // Crear la persona primero
                var person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    IdentificationNumber = dto.IdentificationNumber,
                    Gender = dto.Gender
                };

                // Crear el usuario
                var user = new User
                {
                    Email = dto.Email,
                    Password = dto.Password,
                    RoleId = dto.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UserNavigation = person
                };

                var createdUser = await _userRepo.AddAsync(user);
                result.Datos = MapToDto(createdUser);
                result.Exitoso = true;
                result.Mensaje = "Usuario creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario");
                result.Exitoso = false;
                result.Mensaje = "Error al crear usuario";
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> UpdateAsync(UpdateUserDto dto)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Los datos del usuario son requeridos";
                    result.Errores.Add("Los datos del usuario son requeridos");
                    return result;
                }

                var user = await _userRepo.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                user.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Password))
                    user.Password = dto.Password;
                user.RoleId = dto.RoleId;
                user.IsActive = dto.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _userRepo.UpdateAsync(user);

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando usuario {UserId}", dto?.UserId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar usuario";
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                await _userRepo.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Usuario eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando usuario {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar usuario";
            }
            return result;
        }

        public async Task<OperationResult<List<UserDto>>> GetActiveAsync()
        {
            var result = new OperationResult<List<UserDto>>();
            try
            {
                var users = await _userRepo.GetActiveUsersAsync();
                result.Datos = users.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Usuarios activos obtenidos";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando usuarios activos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuarios activos";
            }
            return result;
        }

        public async Task<OperationResult<List<UserDto>>> GetByRoleAsync(int roleId)
        {
            var result = new OperationResult<List<UserDto>>();
            try
            {
                var users = await _userRepo.GetByRoleIdAsync(roleId);
                result.Datos = users.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Usuarios obtenidos por rol";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando usuarios por rol {RoleId}", roleId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuarios por rol";
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> GetByEmailAsync(string email)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                var user = await _userRepo.GetByEmailWithDetailsAsync(email);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario por email {Email}", email);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario por email";
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> GetByUsernameAsync(string username)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                var user = await _userRepo.GetByEmailAsync(username);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario por nombre de usuario {Username}", username);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario por nombre de usuario";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ValidateCredentialsAsync(string username, string password)
        {
            var result = new OperationResult<bool>();
            try
            {
                var user = await _userRepo.GetByEmailAndPasswordAsync(username, password);
                result.Datos = user != null;
                result.Exitoso = true;
                result.Mensaje = "Credenciales validadas";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando credenciales para usuario {Username}", username);
                result.Exitoso = false;
                result.Mensaje = "Error al validar credenciales";
            }
            return result;
        }

        public async Task<OperationResult> UpdateLastLoginAsync(int userId)
        {
            var result = new OperationResult();
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                user.UpdatedAt = DateTime.Now;
                await _userRepo.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Último acceso actualizado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando último acceso para usuario {UserId}", userId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar último acceso";
            }
            return result;
        }

        public async Task<OperationResult> ActivateAsync(int userId)
        {
            var result = new OperationResult();
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                user.IsActive = true;
                user.UpdatedAt = DateTime.Now;
                await _userRepo.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Usuario activado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activando usuario {UserId}", userId);
                result.Exitoso = false;
                result.Mensaje = "Error al activar usuario";
            }
            return result;
        }

        public async Task<OperationResult> DeactivateAsync(int userId)
        {
            var result = new OperationResult();
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    result.Errores.Add("Usuario no encontrado");
                    return result;
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                await _userRepo.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Usuario desactivado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error desactivando usuario {UserId}", userId);
                result.Exitoso = false;
                result.Mensaje = "Error al desactivar usuario";
            }
            return result;
        }

        private static UserDto MapToDto(User u) => new()
        {
            UserId = u.UserId,
            FirstName = u.UserNavigation?.FirstName ?? string.Empty,
            LastName = u.UserNavigation?.LastName ?? string.Empty,
            Email = u.Email,
            RoleId = u.RoleId,
            RoleName = u.Role?.RoleName ?? string.Empty,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };
    }
}