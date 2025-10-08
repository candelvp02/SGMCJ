using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Common;

namespace SGMCJ.Persistence.Ado.Medical
{
    public class PacienteAdoRepository : IPacienteAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<PacienteAdoRepository> _logger;

        public PacienteAdoRepository(StoredProcedureExecutor sp, ILogger<PacienteAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<Paciente>> ListarActivosAsync()
        {
            var pacientes = new List<Paciente>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("dbo.usp_Paciente_ListarActivos");

                while (await r.ReadAsync())
                {
                    var paciente = new Paciente
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        Nombre = r.GetString(r.GetOrdinal("Nombre")),
                        Apellido = r.GetString(r.GetOrdinal("Apellido")),
                        Cedula = r.GetString(r.GetOrdinal("Cedula")),
                        FechaNacimiento = r.GetDateTime(r.GetOrdinal("FechaNacimiento")),
                        Sexo = r.GetString(r.GetOrdinal("Sexo")),
                        Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? string.Empty : r.GetString(r.GetOrdinal("Telefono")),
                        Email = r.IsDBNull(r.GetOrdinal("Email")) ? string.Empty : r.GetString(r.GetOrdinal("Email")),
                        Direccion = r.IsDBNull(r.GetOrdinal("Direccion")) ? string.Empty : r.GetString(r.GetOrdinal("Direccion"))
                    };
                    pacientes.Add(paciente);
                }

                return pacientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar pacientes activos");
                return new List<Paciente>();
            }
        }

        public async Task<List<Paciente>> BuscarPorNombreAsync(string nombre)
        {
            var pacientes = new List<Paciente>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Paciente_BuscarPorNombre",
                    ("@Nombre", nombre)
                );

                while (await r.ReadAsync())
                {
                    var paciente = new Paciente
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        Nombre = r.GetString(r.GetOrdinal("Nombre")),
                        Apellido = r.GetString(r.GetOrdinal("Apellido")),
                        Cedula = r.GetString(r.GetOrdinal("Cedula")),
                        FechaNacimiento = r.GetDateTime(r.GetOrdinal("FechaNacimiento")),
                        Sexo = r.GetString(r.GetOrdinal("Sexo")),
                        Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? string.Empty : r.GetString(r.GetOrdinal("Telefono")),
                        Email = r.IsDBNull(r.GetOrdinal("Email")) ? string.Empty : r.GetString(r.GetOrdinal("Email")),
                        Direccion = r.IsDBNull(r.GetOrdinal("Direccion")) ? string.Empty : r.GetString(r.GetOrdinal("Direccion"))
                    };
                    pacientes.Add(paciente);
                }

                return pacientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar pacientes por nombre: {Nombre}", nombre);
                return new List<Paciente>();
            }
        }

        public async Task<int> ObtenerTotalCitasAsync(int pacienteId)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "dbo.usp_Paciente_ObtenerTotalCitas",
                    ("@PacienteId", pacienteId)
                );
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de citas para paciente {PacienteId}", pacienteId);
                return 0;
            }
        }
    }
}