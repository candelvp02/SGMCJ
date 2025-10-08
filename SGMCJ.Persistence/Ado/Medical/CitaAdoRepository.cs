using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Common;
using SGMCJ.Domain.Dto;

namespace SGMCJ.Persistence.Ado.Medical
{
    public class CitaAdoRepository : ICitaAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<CitaAdoRepository> _logger;

        public CitaAdoRepository(StoredProcedureExecutor sp, ILogger<CitaAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<CitaDto>> ListarConDetallesAsync()
        {
            var citas = new List<CitaDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("dbo.usp_Cita_ListarConDetalles");

                while (await r.ReadAsync())
                {
                    var cita = new CitaDto
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        PacienteId = r.GetInt32(r.GetOrdinal("PacienteId")),
                        MedicoId = r.GetInt32(r.GetOrdinal("MedicoId")),
                        FechaHora = r.GetDateTime(r.GetOrdinal("FechaHora")),
                        Motivo = r.IsDBNull(r.GetOrdinal("Motivo")) ? string.Empty : r.GetString(r.GetOrdinal("Motivo")),
                        Observaciones = r.IsDBNull(r.GetOrdinal("Observaciones")) ? string.Empty : r.GetString(r.GetOrdinal("Observaciones"))
                    };
                    citas.Add(cita);
                }

                return citas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar citas con detalles");
                return new List<CitaDto>();
            }
        }

        public async Task<List<CitaDto>> ListarPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var citas = new List<CitaDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "dbo.usp_Cita_ListarPorRangoFechas",
                    ("@FechaInicio", fechaInicio),
                    ("@FechaFin", fechaFin)
                );

                while (await r.ReadAsync())
                {
                    var cita = new CitaDto
                    {
                        Id = r.GetInt32(r.GetOrdinal("Id")),
                        PacienteId = r.GetInt32(r.GetOrdinal("PacienteId")),
                        MedicoId = r.GetInt32(r.GetOrdinal("MedicoId")),
                        FechaHora = r.GetDateTime(r.GetOrdinal("FechaHora")),
                        Motivo = r.IsDBNull(r.GetOrdinal("Motivo")) ? string.Empty : r.GetString(r.GetOrdinal("Motivo")),
                        Observaciones = r.IsDBNull(r.GetOrdinal("Observaciones")) ? string.Empty : r.GetString(r.GetOrdinal("Observaciones"))
                    };
                    citas.Add(cita);
                }

                return citas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar citas por rango de fechas");
                return new List<CitaDto>();
            }
        }

        public async Task<bool> CancelarCitaAsync(int citaId, string motivo)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "dbo.usp_Cita_Cancelar",
                    ("@CitaId", citaId),
                    ("@Motivo", motivo)
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {CitaId}", citaId);
                return false;
            }
        }

        public async Task<bool> ConfirmarCitaAsync(int citaId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "dbo.usp_Cita_Confirmar",
                    ("@CitaId", citaId)
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar cita {CitaId}", citaId);
                return false;
            }
        }

        public async Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "dbo.usp_Cita_ExisteEnHorario",
                    ("@MedicoId", medicoId),
                    ("@FechaHora", fechaHora)
                );

                return result.HasValue && result.Value > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar cita en horario");
                return false;
            }
        }
    }
}