using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using System.Text.RegularExpressions;

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
            _logger.LogInformation("Buscando usuario con ID: {UserId}", id);
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de usuario inválido: {UserId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "ID de usuario inválido.";
                    result.Errores.Add("El ID debe ser un número positivo.");
                    return result;
                }

                var user = await _userRepo.GetByIdWithDetailsAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado con ID: {UserId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado.";
                    result.Errores.Add($"No existe un usuario con el ID {id}.");
                    return result;
                }

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario obtenido correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> CreateAsync(RegisterUserDto dto)
        {
            var result = new OperationResult<UserDto>();
            _logger.LogInformation("Iniciando creación de nuevo usuario para el email: {Email}", dto?.Email);
            try
            {
                var validationResult = await ValidateCreateUserDto(dto);
                if (!validationResult.Exitoso)
                {
                    _logger.LogWarning("Validación fallida para la creación de usuario con email: {Email}", dto?.Email);
                    result.Exitoso = false;
                    result.Mensaje = validationResult.Mensaje;
                    result.Errores.AddRange(validationResult.Errores);
                    return result;
                }

                _logger.LogInformation("Validaciones completadas exitosamente.");

                var person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    IdentificationNumber = dto.IdentificationNumber,
                    Gender = dto.Gender
                };

                var user = new User
                {
                    Email = dto.Email,
                    Password = dto.Password, // Recordar hacer hash a esto en un proyecto real
                    RoleId = dto.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UserNavigation = person
                };

                var createdUser = await _userRepo.AddAsync(user);
                result.Datos = MapToDto(createdUser);
                result.Exitoso = true;
                result.Mensaje = "Usuario creado correctamente.";
                _logger.LogInformation("Usuario creado con ID: {UserId}", createdUser.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario");
                result.Exitoso = false;
                result.Mensaje = "Error al crear usuario.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<UserDto>> UpdateAsync(UpdateUserDto dto)
        {
            var result = new OperationResult<UserDto>();
            _logger.LogInformation("Iniciando actualización de usuario ID: {UserId}", dto?.UserId);
            try
            {
                if (dto == null || dto.UserId <= 0)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos del usuario inválidos.";
                    result.Errores.Add("Se requiere un DTO válido con un ID de usuario positivo.");
                    return result;
                }

                var user = await _userRepo.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Usuario a actualizar no encontrado: {UserId}", dto.UserId);
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado.";
                    result.Errores.Add($"No existe un usuario con el ID {dto.UserId}.");
                    return result;
                }

                if (user.Email != dto.Email)
                {
                    var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
                    if (existingUser != null && existingUser.UserId != dto.UserId)
                    {
                        _logger.LogWarning("Intento de actualizar a un email ya existente: {Email}", dto.Email);
                        result.Exitoso = false;
                        result.Mensaje = "El email ya está en uso.";
                        result.Errores.Add($"Ya existe otro usuario con el email '{dto.Email}'.");
                        return result;
                    }
                }

                user.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    user.Password = dto.Password;
                }
                user.RoleId = dto.RoleId;
                user.IsActive = dto.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _userRepo.UpdateAsync(user);

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando usuario {UserId}", dto?.UserId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar usuario.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();
            _logger.LogInformation("Iniciando desactivación de usuario ID: {UserId}", id);
            try
            {
                if (id <= 0)
                {
                    result.Exitoso = false;
                    result.Mensaje = "ID de usuario inválido.";
                    result.Errores.Add("El ID debe ser un número positivo.");
                    return result;
                }

                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado.";
                    result.Errores.Add($"No existe un usuario con el ID {id}.");
                    return result;
                }

                if (!user.IsActive)
                {
                    _logger.LogInformation("El usuario {UserId} ya estaba inactivo.", id);
                    result.Exitoso = true;
                    result.Mensaje = "El usuario ya se encontraba inactivo.";
                    return result;
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                await _userRepo.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Usuario desactivado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error desactivando usuario {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al desactivar el usuario.";
                result.Errores.Add(ex.Message);
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
            // Asumimos que el "username" es el email, como en tu código original
            return await GetByEmailAsync(username);
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
                    return result;
                }

                user.UpdatedAt = DateTime.Now; // Puedes usar un campo dedicado como `LastLoginAt` si lo tienes
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
                    return result;
                }

                if (user.IsActive)
                {
                    result.Exitoso = true;
                    result.Mensaje = "El usuario ya estaba activo.";
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
            // Este método es funcionalmente idéntico a DeleteAsync (soft delete)
            return await DeleteAsync(userId);
        }

        private async Task<OperationResult> ValidateCreateUserDto(RegisterUserDto dto)
        {
            var result = new OperationResult { Exitoso = true };

            if (dto == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Los datos del usuario son requeridos.";
                result.Errores.Add("El cuerpo de la solicitud no puede ser nulo.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || !Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                result.Exitoso = false;
                result.Errores.Add("El formato del email es inválido.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            {
                result.Exitoso = false;
                result.Errores.Add("La contraseña es obligatoria y debe tener al menos 6 caracteres.");
            }

            if (dto.RoleId <= 0)
            {
                result.Exitoso = false;
                result.Errores.Add("Se debe especificar un rol válido.");
            }

            if (!result.Exitoso)
            {
                result.Mensaje = "Datos de entrada inválidos.";
                return result;
            }

            var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                result.Exitoso = false;
                result.Mensaje = "El email ya está en uso.";
                result.Errores.Add($"Ya existe un usuario con el email '{dto.Email}'.");
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