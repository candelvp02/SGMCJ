using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Dto;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Domain.Interfaces.Repositories;

namespace SGMCJ.Application.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly IPacienteRepository _pacienteRepo;
        private readonly ILogger<CitaService> _logger;

        public CitaService(
            ICitaRepository citaRepo,
            IMedicoRepository medicoRepo,
            IPacienteRepository pacienteRepo,
            ILogger<CitaService> logger)
        {
            _citaRepo = citaRepo;
            _medicoRepo = medicoRepo;
            _pacienteRepo = pacienteRepo;
            _logger = logger;
        }

        // === MÉTODOS CON DTOs (para API) ===
        public async Task<OperationResult<List<CitaDto>>> GetAllAsync()
        {
            return await ObtenerTodasAsync();
        }

        public async Task<OperationResult<CitaDto>> GetByIdAsync(int id)
        {
            return await ObtenerPorIdAsync(id);
        }

        public async Task<OperationResult<CitaDto>> CreateAsync(CitaDto citaDto)
        {
            return await CrearCitaAsync(citaDto);
        }

        public async Task<OperationResult<CitaDto>> UpdateAsync(CitaDto citaDto)
        {
            var result = new OperationResult<CitaDto>();
            try
            {
                var cita = await _citaRepo.GetByIdAsync(citaDto.Id);
                if (cita == null)
                    return Fallo(result, "Cita no encontrada");

                cita.FechaHora = citaDto.FechaHora;
                cita.Motivo = citaDto.Motivo;
                cita.Observaciones = citaDto.Observaciones;

                await _citaRepo.UpdateAsync(cita);
                result.Exitoso = true;
                result.Mensaje = "Cita actualizada correctamente";
                result.Datos = MapToDto(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando cita {CitaId}", citaDto.Id);
                return Fallo(result, "Error al actualizar cita");
            }
            return result;
        }

        public async Task<OperationResult<List<CitaDto>>> GetByPacienteIdAsync(int pacienteId)
        {
            var result = NewResult<List<CitaDto>>();
            try
            {
                var citas = await _citaRepo.GetByPacienteIdAsync(pacienteId);
                result.Datos = citas.Select(MapToDto).ToList();
                result.Exitoso = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo citas del paciente {PacienteId}", pacienteId);
                return Fallo(result, "Error al obtener citas del paciente");
            }
            return result;
        }

        public async Task<OperationResult<List<CitaDto>>> GetByMedicoIdAsync(int medicoId)
        {
            var result = NewResult<List<CitaDto>>();
            try
            {
                var citas = await _citaRepo.GetByMedicoIdAsync(medicoId);
                result.Datos = citas.Select(MapToDto).ToList();
                result.Exitoso = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo citas del médico {MedicoId}", medicoId);
                return Fallo(result, "Error al obtener citas del médico");
            }
            return result;
        }

        public async Task<OperationResult<List<CitaDto>>> GetByFechaAsync(DateTime fecha)
        {
            var result = NewResult<List<CitaDto>>();
            try
            {
                var citas = await _citaRepo.GetByFechaAsync(fecha);
                result.Datos = citas.Select(MapToDto).ToList();
                result.Exitoso = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo citas para la fecha {Fecha}", fecha);
                return Fallo(result, "Error al obtener citas por fecha");
            }
            return result;
        }

        public async Task<OperationResult> CancelarCitaAsync(int citaId, string? motivo)
        {
            var result = NewResult();
            _logger.LogInformation("Cancelando cita {CitaId}. Motivo: {Motivo}", citaId, motivo ?? "(no especificado)");

            try
            {
                var cita = await _citaRepo.GetByIdAsync(citaId);
                if (cita is null)
                    return Fallo(result, "La cita no existe.");

                if (cita.Estado == EstadoCita.Completada)
                    return Fallo(result, "No se puede cancelar una cita completada.");

                if (cita.Estado == EstadoCita.Cancelada)
                    return Fallo(result, "La cita ya fue cancelada.");

                cita.Estado = EstadoCita.Cancelada;
                cita.Observaciones = string.IsNullOrWhiteSpace(motivo)
                    ? cita.Observaciones
                    : $"{cita.Observaciones}\nCancelacion: {motivo}";
                cita.FechaModificacion = DateTime.Now;

                await _citaRepo.UpdateAsync(cita);

                result.Exitoso = true;
                result.Mensaje = "Cita cancelada correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar la cita {CitaId}", citaId);
                return Fallo(result, "Ocurrio un error interno al cancelar la cita.");
            }
        }

        public async Task<OperationResult> ConfirmarCitaAsync(int citaId)
        {
            var result = NewResult();
            _logger.LogInformation("Confirmando cita {CitaId}", citaId);

            try
            {
                var cita = await _citaRepo.GetByIdAsync(citaId);
                if (cita is null)
                    return Fallo(result, "La cita no existe.");

                if (cita.Estado != EstadoCita.Programada)
                    return Fallo(result, "Solo se pueden confirmar citas en estado 'Programada'.");

                cita.Estado = EstadoCita.Confirmada;
                cita.FechaModificacion = DateTime.Now;

                await _citaRepo.UpdateAsync(cita);

                result.Exitoso = true;
                result.Mensaje = "Cita confirmada correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar la cita {CitaId}", citaId);
                return Fallo(result, "Ocurrio un error interno al confirmar la cita.");
            }
        }
        public async Task<OperationResult> ReprogramarCitaAsync(int citaId, DateTime nuevaFechaHora)
        {
            var result = new OperationResult();
            _logger.LogInformation("Reprogramando cita {CitaId} a {Fecha}", citaId, nuevaFechaHora);
            try
            {
                var cita = await _citaRepo.GetByIdAsync(citaId);
                if (cita is null)
                    return Fallo(result, "La cita no existe.");

                if (cita.Estado == EstadoCita.Completada || cita.Estado == EstadoCita.Cancelada)
                    return Fallo(result, "No se puede reprogramar una cita completada o cancelada.");

                if (nuevaFechaHora <= DateTime.Now.AddMinutes(10))
                    return Fallo(result, "La nueva fecha/hora debe ser a futuro.");

                var medico = await _medicoRepo.GetByIdAsync(cita.MedicoId);
                var hora = nuevaFechaHora.TimeOfDay;
                var dia = nuevaFechaHora.DayOfWeek;

                var enVentana = medico?.Disponibilidades?.Any(d =>
                    d.EsActivo &&
                    d.DiaSemana == dia &&
                    d.HoraInicio <= hora &&
                    hora < d.HoraFin) ?? false;

                if (!enVentana)
                    return Fallo(result, "La nueva hora está fuera de la disponibilidad del medico.");

                var ocupado = await _citaRepo.ExisteCitaEnHorarioAsync(cita.MedicoId, nuevaFechaHora);
                if (ocupado)
                    return Fallo(result, "El medico ya tiene una cita en ese horario.");

                cita.FechaHora = nuevaFechaHora;
                cita.Estado = EstadoCita.Programada;
                cita.FechaModificacion = DateTime.Now;

                await _citaRepo.UpdateAsync(cita);

                result.Exitoso = true;
                result.Mensaje = "Cita reprogramada correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar la cita {CitaId}", citaId);
                return Fallo(result, "Ocurrio un error interno al reprogramar la cita.");
            }
        }

        // === MÉTODOS CON ENTIDADES (para uso interno EF) ===
        public async Task<OperationResult<Cita>> CreateEntityAsync(Cita cita)
        {
            var result = new OperationResult<Cita>();
            try
            {
                var citaCreada = await _citaRepo.AddAsync(cita);
                result.Exitoso = true;
                result.Mensaje = "Cita creada correctamente";
                result.Datos = citaCreada;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando entidad Cita");
                result.Exitoso = false;
                result.Mensaje = "Error al crear entidad Cita";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<Cita>> UpdateEntityAsync(Cita cita)
        {
            var result = new OperationResult<Cita>();
            try
            {
                await _citaRepo.UpdateAsync(cita);
                result.Exitoso = true;
                result.Mensaje = "Cita actualizada correctamente";
                result.Datos = cita;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando entidad Cita {Id}", cita.Id);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar entidad Cita";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        // === MÉTODOS PRIVADOS ===
        private async Task<OperationResult<List<CitaDto>>> ObtenerTodasAsync()
        {
            var result = NewResult<List<CitaDto>>();
            _logger.LogInformation("Listando todas las citas (rango amplio).");

            try
            {
                var desde = DateTime.Now.AddYears(-5);
                var hasta = DateTime.Now.AddYears(5);

                var citas = await _citaRepo.GetCitasProximasAsync(desde, hasta);
                var dto = citas.Select(MapToDto).ToList();

                result.Exitoso = true;
                result.Mensaje = "Listado generado correctamente.";
                result.Datos = dto;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar todas las citas.");
                return Fallo(result, "Ocurrio un error interno al listar las citas.");
            }
        }

        private async Task<OperationResult<CitaDto>> ObtenerPorIdAsync(int id)
        {
            var result = NewResult<CitaDto>();
            _logger.LogInformation("Obteniendo cita por Id {CitaId}", id);

            try
            {
                var cita = await _citaRepo.GetByIdAsync(id);
                if (cita is null)
                    return Fallo(result, "La cita no existe.");

                result.Exitoso = true;
                result.Mensaje = "Cita obtenida correctamente.";
                result.Datos = MapToDto(cita);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cita {CitaId}", id);
                return Fallo(result, "Ocurrio un error interno al obtener la cita.");
            }
        }

        private async Task<OperationResult<CitaDto>> CrearCitaAsync(CitaDto dto)
        {
            var result = NewResult<CitaDto>();
            _logger.LogInformation("Iniciando creacion de cita: PacienteId={PacienteId}, MedicoId={MedicoId}, Fecha={Fecha}", dto?.PacienteId, dto?.MedicoId, dto?.FechaHora);

            try
            {
                if (dto is null)
                    return Fallo(result, "El cuerpo de la solicitud (CitaDto) es requerido.");

                if (dto.PacienteId <= 0 || dto.MedicoId <= 0)
                    return Fallo(result, "PacienteId y MedicoId son obligatorios.");

                if (dto.FechaHora == default)
                    return Fallo(result, "La FechaHora de la cita es obligatoria.");

                if (dto.FechaHora <= DateTime.Now.AddMinutes(10))
                    return Fallo(result, "La cita debe programarse con al menos 10 minutos de antelacion.");

                var paciente = await _pacienteRepo.GetByIdAsync(dto.PacienteId);
                if (paciente is null)
                    return Fallo(result, "El paciente no existe.");

                var medico = await _medicoRepo.GetByIdAsync(dto.MedicoId);
                if (medico is null)
                    return Fallo(result, "El medico no existe.");

                var hora = dto.FechaHora.TimeOfDay;
                var dia = dto.FechaHora.DayOfWeek;

                var dentroDeDisponibilidad = medico?.Disponibilidades?.Any(d =>
                    d.EsActivo &&
                    d.DiaSemana == dia &&
                    d.HoraInicio <= hora &&
                    hora < d.HoraFin) ?? false;

                if (!dentroDeDisponibilidad)
                    return Fallo(result, "La hora solicitada está fuera de la disponibilidad del medico.");

                var ocupado = await _citaRepo.ExisteCitaEnHorarioAsync(dto.MedicoId, dto.FechaHora);
                if (ocupado)
                    return Fallo(result, "El medico ya tiene una cita en ese horario.");

                var citasPacienteMismaHora = (await _citaRepo.GetCitasByPacienteAsync(dto.PacienteId))
                    .Any(c => c.FechaHora == dto.FechaHora && c.Estado != EstadoCita.Cancelada);

                if (citasPacienteMismaHora)
                    return Fallo(result, "El paciente ya tiene una cita en ese horario.");

                var cita = new Cita
                {
                    FechaHora = dto.FechaHora,
                    Estado = EstadoCita.Programada,
                    Motivo = dto.Motivo ?? string.Empty,
                    Observaciones = dto.Observaciones ?? string.Empty,
                    PacienteId = dto.PacienteId,
                    MedicoId = dto.MedicoId,
                    FechaCreacion = DateTime.Now,
                    EstaEliminado = false
                };

                cita = await _citaRepo.AddAsync(cita);

                var salida = MapToDto(cita);
                result.Exitoso = true;
                result.Mensaje = "Cita creada correctamente.";
                result.Datos = salida;

                _logger.LogInformation("Cita {CitaId} creada: MedicoId={MedicoId}, PacienteId={PacienteId}", cita.Id, cita.MedicoId, cita.PacienteId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear la cita. MedicoId={MedicoId}, PacienteId={PacienteId}", dto?.MedicoId, dto?.PacienteId);
                return Fallo(result, "Ocurrio un error interno al crear la cita.");
            }
        }

        // === HELPERS ===
        private static OperationResult NewResult() => new() { Mensaje = string.Empty };
        private static OperationResult<T> NewResult<T>() => new() { Mensaje = string.Empty };

        private static OperationResult Fallo(OperationResult r, string mensaje)
        {
            r.Exitoso = false;
            r.Mensaje = mensaje;
            r.Errores.Add(mensaje);
            return r;
        }

        private static OperationResult<T> Fallo<T>(OperationResult<T> r, string mensaje)
        {
            r.Exitoso = false;
            r.Mensaje = mensaje;
            r.Errores.Add(mensaje);
            return r;
        }

        private static CitaDto MapToDto(Cita c) => new()
        {
            Id = c.Id,
            FechaHora = c.FechaHora,
            Motivo = c.Motivo,
            Observaciones = c.Observaciones,
            PacienteId = c.PacienteId,
            MedicoId = c.MedicoId
        };
    }
}