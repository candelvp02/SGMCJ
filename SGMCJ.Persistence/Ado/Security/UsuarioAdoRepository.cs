using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Entities.Security;
using SGMCJ.Domain.Repositories.Security;
using SGMCJ.Persistence.Common;

namespace SGMCJ.Persistence.Ado.Security
{
    public class UsuarioAdoRepository : IUsuarioAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<UsuarioAdoRepository> _logger;

        public UsuarioAdoRepository(StoredProcedureExecutor sp, ILogger<UsuarioAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<Usuario>> ListarActivosAsync()
        {
            _logger.LogInformation("Listando usuarios activos");

            try
            {
                var usuarios = new List<Usuario>();
                using var r = await _sp.ExecuteReaderAsync("dbo.usp_Usuario_GetActivos");

                while (await r.ReadAsync())
                {
                    var usuario = MapToUsuarioEntity(r);
                    if (usuario != null)
                        usuarios.Add(usuario);
                }

                _logger.LogInformation("Se encontraron {Count} usuarios activos", usuarios.Count);
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar usuarios activos");
                return new List<Usuario>();
            }
        }

        public async Task<List<Usuario>> ListarPorRolAsync(string rol)
        {
            _logger.LogInformation("Listando usuarios por rol: {Rol}", rol);

            try
            {
                var usuarios = new List<Usuario>();
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Usuario_GetByRol",
                    ("@NombreRol", rol)
                );

                while (await r.ReadAsync())
                {
                    var usuario = MapToUsuarioEntity(r);
                    if (usuario != null)
                        usuarios.Add(usuario);
                }

                _logger.LogInformation("Se encontraron {Count} usuarios para el rol {Rol}", usuarios.Count, rol);
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar usuarios por rol: {Rol}", rol);
                return new List<Usuario>();
            }
        }

        public async Task<bool> ActualizarUltimoAccesoAsync(int usuarioId)
        {
            _logger.LogInformation("Actualizando último acceso para usuario ID: {UsuarioId}", usuarioId);

            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "dbo.usp_Usuario_ActualizarUltimoAcceso",
                    ("@UsuarioId", usuarioId)
                );

                var success = result > 0;
                _logger.LogInformation("Actualización de último acceso {Estado} para usuario ID: {UsuarioId}",
                    success ? "exitosa" : "fallida", usuarioId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar último acceso para usuario ID: {UsuarioId}", usuarioId);
                return false;
            }
        }

        public async Task<bool> ActivarUsuarioAsync(int usuarioId)
        {
            _logger.LogInformation("Activando usuario ID: {UsuarioId}", usuarioId);

            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "dbo.usp_Usuario_Activar",
                    ("@UsuarioId", usuarioId)
                );

                var success = result > 0;
                _logger.LogInformation("Activación de usuario {Estado} para ID: {UsuarioId}",
                    success ? "exitosa" : "fallida", usuarioId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar usuario ID: {UsuarioId}", usuarioId);
                return false;
            }
        }

        public async Task<bool> DesactivarUsuarioAsync(int usuarioId)
        {
            _logger.LogInformation("Desactivando usuario ID: {UsuarioId}", usuarioId);

            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "dbo.usp_Usuario_Desactivar",
                    ("@UsuarioId", usuarioId)
                );

                var success = result > 0;
                _logger.LogInformation("Desactivación de usuario {Estado} para ID: {UsuarioId}",
                    success ? "exitosa" : "fallida", usuarioId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuario ID: {UsuarioId}", usuarioId);
                return false;
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo usuario por ID: {Id}", id);

            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Usuario_GetById",
                    ("@Id", id)
                );

                if (await r.ReadAsync())
                {
                    var usuario = MapToUsuarioEntity(r);
                    _logger.LogInformation("Usuario encontrado: {NombreUsuario} (ID: {Id})", usuario?.NombreUsuario, id);
                    return usuario;
                }

                _logger.LogWarning("Usuario no encontrado para ID: {Id}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID: {Id}", id);
                return null;
            }
        }

        public async Task<Usuario> AddAsync(Usuario entity)
        {
            _logger.LogInformation("Agregando nuevo usuario: {NombreUsuario}", entity.NombreUsuario);

            try
            {
                var result = await _sp.ExecuteScalarAsync<int>(
                    "dbo.usp_Usuario_Insert",
                    ("@NombreUsuario", entity.NombreUsuario),
                    ("@Email", entity.Email),
                    ("@PasswordHash", entity.PasswordHash),
                    ("@EsActivo", entity.EsActivo),
                    ("@UltimoAcceso", entity.UltimoAcceso)
                );

                entity.Id = result;
                _logger.LogInformation("Usuario agregado exitosamente: {NombreUsuario} (ID: {Id})", entity.NombreUsuario, entity.Id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario: {NombreUsuario}", entity.NombreUsuario);
                throw;
            }
        }

        private static Usuario? MapToUsuarioEntity(SqlDataReader r)
        {
            try
            {
                var usuario = new Usuario
                {
                    Id = r.GetInt32(r.GetOrdinal("Id")),
                    NombreUsuario = r.GetString(r.GetOrdinal("NombreUsuario")),
                    Email = r.GetString(r.GetOrdinal("Email")),
                    PasswordHash = r.GetString(r.GetOrdinal("PasswordHash")),
                    EsActivo = r.GetBoolean(r.GetOrdinal("EsActivo")),
                    FechaCreacion = r.GetDateTime(r.GetOrdinal("FechaCreacion"))
                };

                if (HasColumn(r, "UltimoAcceso") && !r.IsDBNull(r.GetOrdinal("UltimoAcceso")))
                    usuario.UltimoAcceso = r.GetDateTime(r.GetOrdinal("UltimoAcceso"));

                if (HasColumn(r, "FechaModificacion") && !r.IsDBNull(r.GetOrdinal("FechaModificacion")))
                    usuario.FechaModificacion = r.GetDateTime(r.GetOrdinal("FechaModificacion"));

                usuario.UsuarioRoles = new List<UsuarioRol>();

                return usuario;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}