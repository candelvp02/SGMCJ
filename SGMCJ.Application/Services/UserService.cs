using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Dto.System;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Repositories.Users;
using System.Text.RegularExpressions;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Services
{
    public class UserService(
            IUserRepository repository,
            ILogger<UserService> logger
        ) : IUserService
    {
        private readonly IUserRepository _repository = repository;
        private readonly ILogger<UserService> _logger = logger;

        //autenticacion rf3.1.1
        public async Task<OperationResult<UserDto>> AuthenticateAsync(LoginDto dto)
        {
            var result = new OperationResult<UserDto>();

            try
            {
                // validar datos basicos
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Email y password son requeridos";
                    return result;
                }

                // buscar usuario por email
                var user = await _repository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Credenciales invalidas";
                    return result;
                }

                // verificar password usando hashing
                //if (user.PasswordHash != dto.Password)
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Credenciales invalidas";
                    return result;
                }

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Autenticacion exitosa";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en autenticacion para {Email}", dto.Email);
                result.Exitoso = false;
                result.Mensaje = "Error al autenticart usuario";
            }

            return result;
        }

        // registro rf3.1.1.
        public async Task<OperationResult<UserDto>> RegisterAsync(RegisterUserDto dto)
        {
            var result = new OperationResult<UserDto>();

            try
            {
                //validar datos requeridos
                if (!ValidateRegistrationData(dto, result))
                    return result;

                //validar formato email
                if (!IsValidEmail(dto.Email))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Formato de email invalido";
                    return result;
                }

                //validar unicidad de email
                var existingUser = await _repository.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    result.Exitoso = true;
                    result.Mensaje = "El email ya esta registrado";
                    return result;
                }

                //validar password min 8 caracteres req. de seg.
                if (!IsValidPassword(dto.Password))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La contrasena debe tener al menos 8 caracteres, una mayuscula, una minuscula y un numero";
                    return result;
                }
                // crear usuario
                var user = new User
                {
                    Email = dto.Email.ToLower(),
                    //PasswordHash = dto.Password,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleId = dto.RoleId,
                    IsActive = false, //requiere confirmacion por email
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(user);

                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Usuario registrado. Verifique su email para activar cuenta.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario {Email}", dto.Email);
                result.Exitoso = false;
                result.Mensaje = "Error al registrar usuario";
            }

            return result;
        }

        // recuperacion password rf3.1.1
        public async Task<OperationResult> RequestPasswordResetAsync(string email)
        {
            var result = new OperationResult();

            try
            {
                if (!IsValidEmail(email))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Email invalido";
                    return result;
                }

                var user = await _repository.GetByEmailAsync(email);
                if (user == null)
                {
                    // por seguridad no se revela el email si existe
                    result.Exitoso = true;
                    result.Mensaje = "Si el email existe, recibe instrucciones para restablecer password";
                    return result;
                }

                result.Exitoso = true;
                result.Mensaje = "Si el email existe, recibe instrucciones para restablecer password";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al solicitar password reset para {Email}", email);
                result.Exitoso = false;
                result.Mensaje = "Error al procesar solicitud";
            }
            return result;
        }

        //activar cuenta con confirmacion por email
        public async Task<OperationResult> ActivateAccountAsync(int userId)
        {
            var result = new OperationResult();

            try
            {
                var user = await _repository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    return result;
                }

                if (user.IsActive)
                {
                    result.Exitoso = false;
                    result.Mensaje = "la cuenta ya esta activada";
                    return result;
                }

                user.IsActive = true;
                user.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Cuenta activada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar cuenta {UserId}", userId);
                result.Exitoso = false;
                result.Mensaje = "Error al activar cuenta";
            }

            return result;
        }

        // actualizar perfil
        public async Task<OperationResult<UserDto>> UpdateProfileAsync(UpdateUserDto dto)
        {
            var result = new OperationResult<UserDto>();

            try
            {
                var user = await _repository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    return result;
                }

                // validar email si cambio
                if (dto.Email != user.Email)
                {
                    if (!IsValidEmail(dto.Email))
                    {
                        result.Exitoso = false;
                        result.Mensaje = "Formato de email invalido";
                        return result;
                    }

                    var emailExists = await _repository.GetByEmailAsync(dto.Email);
                    if (emailExists != null)
                    {
                        result.Exitoso = false;
                        result.Mensaje = "El email ya esta en uso";
                        return result;
                    }
                }

                user.Email = dto.Email.ToLower();
                user.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(user);

                result.Datos = MapToDto(user);
                result.Exitoso = true;
                result.Mensaje = "Perfil actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil {UserId}", dto.UserId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar perfil";
            }
            return result;
        }

        // cambiar password
        public async Task<OperationResult> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var result = new OperationResult();

            try
            {
                var user = await _repository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    return result;
                }

                // verify current password
                if (user.PasswordHash != dto.CurrentPassword)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Password actual incorrecta";
                    return result;
                }

                // validar new password
                if (!IsValidPassword(dto.NewPassword))
                {
                    result.Exitoso = false;
                    result.Mensaje = "new password debe tener al menos 8 caracteres, una mayuscula, una minuscula y un numero";
                    return result;
                }

                user.PasswordHash = dto.NewPassword;
                user.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "password cambiada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar password {UserId}", dto.UserId);
                result.Exitoso = false;
                result.Mensaje = "Error al cambiar password";
            }
            return result;
        }

        //consultas
        public async Task<OperationResult<UserDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                var user = await _repository.GetByIdAsync(id);
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
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario";
            }

            return result;
        }
        public async Task<OperationResult<UserDto>> GetByEmailAsync(string email)
        {
            var result = new OperationResult<UserDto>();
            try
            {
                var user = await _repository.GetByEmailAsync(email);
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
                _logger.LogError(ex, "Error al obtener usuario por email {Email}", email);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuario por email";
            }
            return result;
        }

        public async Task<OperationResult<List<UserDto>>> GetByRoleAsync(short roleId)
        {
            var result = new OperationResult<List<UserDto>>();
            try
            {
                var user = await _repository.GetByRoleIdAsync(roleId);
                result.Datos = user.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Usuarios obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por rol {RoleId}", roleId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuarios por rol";
            }
            return result;
        }
        public async Task<OperationResult<List<UserDto>>> GetActiveUsersAsync()
        {
            var result = new OperationResult<List<UserDto>>();

            try
            {
                var users = await _repository.GetActiveUsersAsync();
                result.Datos = users.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Usuarios activos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios activos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener usuarios activos";
            }

            return result;
        }
        public async Task<OperationResult<List<UserDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<UserDto>>();
            try
            {
                var users = await _repository.GetAllAsync();
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
        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();

            try
            {
                var user = await _repository.GetByIdAsync(id);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    return result;
                }

                await _repository.DeleteAsync(id);
                result.Exitoso = true;
                result.Mensaje = "Usuario eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar usuario";
            }

            return result;
        }

        public async Task<OperationResult> DeactivateAsync(int id)
        {
            var result = new OperationResult();

            try
            {
                var user = await _repository.GetByIdAsync(id);
                if (user == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Usuario no encontrado";
                    return result;
                }

                if (!user.IsActive)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cuenta ya está desactivada";
                    return result;
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(user);

                result.Exitoso = true;
                result.Mensaje = "Cuenta desactivada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar cuenta {UserId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al desactivar cuenta";
            }

            return result;
        }

        // metodos privados de validacion
        private static bool ValidateRegistrationData(RegisterUserDto dto, OperationResult result)
        {
            if (dto == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Datos de registro requeridos";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                result.Exitoso = false;
                result.Mensaje = "Email es requerido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                result.Exitoso = false;
                result.Mensaje = "Password es requerido";
                return false;
            }

            if (dto.RoleId <= 0)
            {
                result.Exitoso = false;
                result.Mensaje = "Rol es requerido";
                return false;
            }

            return true;
        }
        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        private static bool IsValidPassword(string password)
        {
            // min 8 caracteres 1 mayuscula 1 minuscula 1 numero
            return !string.IsNullOrWhiteSpace(password) &&
                    password.Length >= 8 &&
                    Regex.IsMatch(password, @"[A-Z]") &&
                    Regex.IsMatch(password, @"[a-z]") &&
                    Regex.IsMatch(password, @"\d");
        }

        //mapper
        private static UserDto MapToDto(User u) => new()
        {
            UserId = u.UserId,
            Email = u.Email,
            RoleId = u.RoleId,
            RoleName = u.Role?.RoleName ?? string.Empty,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };
    }
}