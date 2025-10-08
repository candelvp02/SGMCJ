using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Common;

namespace SGMCJ.Persistence.Ado.Medical
{
    public class MedicoAdoRepository : IMedicoAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<MedicoAdoRepository> _logger;

        public MedicoAdoRepository(StoredProcedureExecutor sp, ILogger<MedicoAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<Medico>> ListarActivosAsync()
        {
            var medicos = new List<Medico>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("dbo.usp_Medico_ListarActivos");

                while (await r.ReadAsync())
                {
                    var medico = new Medico
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        Nombre = r.GetString(r.GetOrdinal("Nombre")),
                        Apellido = r.GetString(r.GetOrdinal("Apellido")),
                        Cedula = r.GetString(r.GetOrdinal("Cedula")),
                        NumeroLicencia = r.GetString(r.GetOrdinal("NumeroLicencia")),
                        Especialidad = (Especialidad)Enum.Parse(typeof(Especialidad), r.GetString(r.GetOrdinal("Especialidad"))),
                        Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? string.Empty : r.GetString(r.GetOrdinal("Telefono")),
                        Email = r.IsDBNull(r.GetOrdinal("Email")) ? string.Empty : r.GetString(r.GetOrdinal("Email")),
                        EsActivo = true, 
                        FechaCreacion = DateTime.Now
                    };
                    medicos.Add(medico);
                }

                return medicos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar médicos activos");
                return new List<Medico>();
            }
        }

        public async Task<List<Medico>> BuscarPorNombreAsync(string nombre)
        {
            var medicos = new List<Medico>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Medico_BuscarPorNombre",
                    ("@Nombre", nombre)
                );

                while (await r.ReadAsync())
                {
                    var medico = new Medico
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        Nombre = r.GetString(r.GetOrdinal("Nombre")),
                        Apellido = r.GetString(r.GetOrdinal("Apellido")),
                        Cedula = r.GetString(r.GetOrdinal("Cedula")),
                        NumeroLicencia = r.GetString(r.GetOrdinal("NumeroLicencia")),
                        Especialidad = (Especialidad)Enum.Parse(typeof(Especialidad), r.GetString(r.GetOrdinal("Especialidad"))),
                        Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? string.Empty : r.GetString(r.GetOrdinal("Telefono")),
                        Email = r.IsDBNull(r.GetOrdinal("Email")) ? string.Empty : r.GetString(r.GetOrdinal("Email")),
                        EsActivo = true,
                        FechaCreacion = DateTime.Now
                    };
                    medicos.Add(medico);
                }

                return medicos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar médicos por nombre: {Nombre}", nombre);
                return new List<Medico>();
            }
        }

        public async Task<int> ObtenerTotalCitasAtendidasAsync(int medicoId)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "dbo.usp_Medico_ObtenerTotalCitasAtendidas",
                    ("@MedicoId", medicoId)
                );
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de citas atendidas para médico {MedicoId}", medicoId);
                return 0;
            }
        }

        public async Task<List<Medico>> ListarPorEspecialidadAsync(Especialidad especialidad)
        {
            var medicos = new List<Medico>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Medico_ListarPorEspecialidad",
                    ("@Especialidad", (int)especialidad) // ✅ Convertir enum a int
                );

                while (await r.ReadAsync())
                {
                    var medico = new Medico
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        Nombre = r.GetString(r.GetOrdinal("Nombre")),
                        Apellido = r.GetString(r.GetOrdinal("Apellido")),
                        Cedula = r.GetString(r.GetOrdinal("Cedula")),
                        NumeroLicencia = r.GetString(r.GetOrdinal("NumeroLicencia")),
                        Especialidad = (Especialidad)Enum.Parse(typeof(Especialidad), r.GetString(r.GetOrdinal("Especialidad"))),
                        Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? string.Empty : r.GetString(r.GetOrdinal("Telefono")),
                        Email = r.IsDBNull(r.GetOrdinal("Email")) ? string.Empty : r.GetString(r.GetOrdinal("Email"))
                    };
                    medicos.Add(medico);
                }

                return medicos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar médicos por especialidad: {Especialidad}", especialidad);
                return new List<Medico>();
            }
        }
    }
}