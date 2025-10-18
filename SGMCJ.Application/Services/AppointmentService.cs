using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;

namespace SGMCJ.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepo = appointmentRepo;
            _logger = logger;
        }
        public async Task<OperationResult<List<AppointmentDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<AppointmentDto>>();
            _logger.LogInformation("Iniciando obtencion de todas las citas.");

            try
            {
                var appointments = await _appointmentRepo.GetAllWithDetailsAsync();

                if (appointments == null || !appointments.Any())
                {
                    _logger.LogWarning("No se encontraron citas en la bd.");
                    result.Exitoso = true;
                    result.Mensaje = "No hay citas registradas.";
                    result.Datos = new List<AppointmentDto>();
                    return result;
                }

                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente.";
                _logger.LogInformation("Se obtuvieron {Count} citas exitosamente.", result.Datos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las citas.");
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error inesperado al obtener las citas.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<AppointmentDto>();
            _logger.LogInformation("Iniciando obtencion de cita con ID: {AppointmentId}", id);

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de cita invalido: {AppointmentId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "ID de cita inválido.";
                    result.Errores.Add("El ID debe ser un número positivo.");
                    return result;
                }

                var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(id);

                if (appointment == null)
                {
                    _logger.LogWarning("Cita no encontrada con ID: {AppointmentId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada.";
                    result.Errores.Add("No existe una cita con el ID especificado.");
                    return result;
                }

                result.Datos = MapToDto(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita obtenida correctamente.";
                _logger.LogInformation("Cita obtenida exitosamente: {AppointmentId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cita {AppointmentId}", id);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error inesperado al obtener la cita.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();
            _logger.LogInformation("Iniciando creacion de cita para paciente {PatientId} y doctor {DoctorId}", dto?.PatientId, dto?.DoctorId);

            try
            {
                var validationResult = await ValidateCreateAppointmentDto(dto);
                if (!validationResult.Exitoso)
                {
                    _logger.LogWarning("Validacion de datos fallida al crear cita.");
                    return validationResult;
                }

                _logger.LogInformation("Validaciones de entrada completadas exitosamente");

                var appointment = new Appointment
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    StatusId = AppointmentStatus.Scheduled,
                    CreatedAt = DateTime.Now
                };

                var created = await _appointmentRepo.AddAsync(appointment);

                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Cita creada correctamente";
                _logger.LogInformation("Cita creada exitosamente con ID: {AppointmentId}", created.AppointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear la cita. PatientId={PatientId}, DoctorId={DoctorId}", dto?.PatientId, dto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error interno al crear la cita";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();
            _logger.LogInformation("Iniciando actualización de cita con ID: {AppointmentId}", dto?.AppointmentId);

            try
            {
                if (dto == null || dto.AppointmentId <= 0)
                {
                    _logger.LogWarning("Intento de actualizar cita con datos nulos o ID inválido.");
                    result.Exitoso = false;
                    result.Mensaje = "Datos de entrada inválidos.";
                    result.Errores.Add("Se requiere un DTO válido con un ID de cita positivo.");
                    return result;
                }

                var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("Intento de actualizar cita inexistente: {AppointmentId}", dto.AppointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada.";
                    result.Errores.Add($"No existe una cita con ID {dto.AppointmentId}.");
                    return result;
                }

                if (appointment.AppointmentDate != dto.AppointmentDate)
                {
                    var validation = await ValidateAppointmentDateAsync(appointment.DoctorId, dto.AppointmentDate);
                    if (!validation.Exitoso)
                    {
                        _logger.LogWarning("Validación de nueva fecha fallida para cita: {AppointmentId}", dto.AppointmentId);
                        result.Exitoso = false;
                        result.Mensaje = "Validación de fecha fallida.";
                        result.Errores.AddRange(validation.Errores);
                        return result;
                    }
                }

                _logger.LogInformation("Validaciones completadas exitosamente.");

                appointment.AppointmentDate = dto.AppointmentDate;
                appointment.StatusId = dto.StatusId;
                appointment.UpdatedAt = DateTime.Now;

                await _appointmentRepo.UpdateAsync(appointment);

                result.Datos = MapToDto(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita actualizada correctamente.";
                _logger.LogInformation("Cita actualizada exitosamente: {AppointmentId}", dto.AppointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cita {AppointmentId}", dto?.AppointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar la cita.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult> CancelAsync(int appointmentId)
        {
            var result = new OperationResult();
            _logger.LogInformation("Iniciando cancelación de cita con ID: {AppointmentId}", appointmentId);

            try
            {
                if (appointmentId <= 0)
                {
                    _logger.LogWarning("ID de cita inválido: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "ID de cita inválido.";
                    result.Errores.Add("El ID debe ser mayor a 0.");
                    return result;
                }

                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("Intento de cancelar cita inexistente: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add($"No existe una cita con ID {appointmentId}.");
                    return result;
                }

                if (appointment.AppointmentDate < DateTime.Now)
                {
                    _logger.LogWarning("Intento de cancelar una cita pasada: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "No se puede cancelar una cita que ya ha pasado.";
                    result.Errores.Add("La fecha de la cita es anterior a la fecha actual.");
                    return result;
                }

                if (appointment.StatusId == AppointmentStatus.Cancelled)
                {
                    _logger.LogInformation("La cita ya estaba cancelada: {AppointmentId}", appointmentId);
                    result.Exitoso = true;
                    result.Mensaje = "La cita ya se encontraba cancelada.";
                    return result;
                }

                appointment.StatusId = AppointmentStatus.Cancelled;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita cancelada correctamente.";
                _logger.LogInformation("Cita cancelada exitosamente: {AppointmentId}", appointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al cancelar la cita.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult> ConfirmAsync(int appointmentId)
        {
            var result = new OperationResult();
            _logger.LogInformation("Iniciando confirmación de cita con ID: {AppointmentId}", appointmentId);

            try
            {
                if (appointmentId <= 0)
                {
                    _logger.LogWarning("ID de cita inválido: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "ID de cita inválido";
                    result.Errores.Add("El ID debe ser mayor a 0");
                    return result;
                }

                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("Intento de confirmar cita inexistente: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe";
                    result.Errores.Add($"No existe una cita con ID {appointmentId}");
                    return result;
                }

                if (appointment.StatusId == AppointmentStatus.Cancelled)
                {
                    _logger.LogWarning("Intento de confirmar cita cancelada: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "No se puede confirmar una cita cancelada";
                    result.Errores.Add("La cita está cancelada y no puede ser confirmada");
                    return result;
                }

                if (appointment.StatusId == AppointmentStatus.Confirmed)
                {
                    _logger.LogInformation("La cita ya estaba confirmada: {AppointmentId}", appointmentId);
                    result.Exitoso = true;
                    result.Mensaje = "La cita ya estaba confirmada";
                    return result;
                }

                appointment.StatusId = AppointmentStatus.Confirmed;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita confirmada correctamente";
                _logger.LogInformation("Cita confirmada exitosamente: {AppointmentId}", appointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al confirmar cita";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult> RescheduleAsync(int appointmentId, DateTime newDate)
        {
            var result = new OperationResult();
            _logger.LogInformation("Iniciando reprogramación de cita ID: {AppointmentId} para fecha: {NewDate}", appointmentId, newDate);

            try
            {
                if (appointmentId <= 0 || newDate == default)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos de entrada inválidos.";
                    if (appointmentId <= 0) result.Errores.Add("El ID de la cita debe ser positivo.");
                    if (newDate == default) result.Errores.Add("La nueva fecha es requerida.");
                    return result;
                }

                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("Intento de reprogramar cita inexistente: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add($"No existe una cita con ID {appointmentId}.");
                    return result;
                }

                if (appointment.StatusId == AppointmentStatus.Cancelled)
                {
                    _logger.LogWarning("Intento de reprogramar cita cancelada: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "No se puede reprogramar una cita cancelada.";
                    result.Errores.Add("La cita está cancelada.");
                    return result;
                }

                var validation = await ValidateAppointmentDateAsync(appointment.DoctorId, newDate);
                if (!validation.Exitoso)
                {
                    _logger.LogWarning("Validación de nueva fecha fallida para reprogramación: {AppointmentId}", appointmentId);
                    result.Exitoso = false;
                    result.Mensaje = "Validación de fecha fallida.";
                    result.Errores.AddRange(validation.Errores);
                    return result;
                }

                appointment.AppointmentDate = newDate;
                appointment.StatusId = AppointmentStatus.Scheduled;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita reprogramada correctamente.";
                _logger.LogInformation("Cita reprogramada exitosamente: {AppointmentId}", appointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al reprogramar la cita.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByPatientIdAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            _logger.LogInformation("Obteniendo citas del paciente: {PatientId}", patientId);

            try
            {
                if (patientId <= 0)
                {
                    _logger.LogWarning("PatientId inválido: {PatientId}", patientId);
                    result.Exitoso = false;
                    result.Mensaje = "ID de paciente inválido";
                    result.Errores.Add("El ID del paciente debe ser mayor a 0");
                    return result;
                }

                var appointments = await _appointmentRepo.GetByPatientIdAsync(patientId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
                _logger.LogInformation("Se obtuvieron {Count} citas para el paciente {PatientId}", result.Datos.Count, patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del paciente";
                result.Errores.Add("Ocurrió un error al obtener las citas");
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDoctorIdAsync(int doctorId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            _logger.LogInformation("Obteniendo citas del doctor: {DoctorId}", doctorId);

            try
            {
                if (doctorId <= 0)
                {
                    _logger.LogWarning("DoctorId inválido: {DoctorId}", doctorId);
                    result.Exitoso = false;
                    result.Mensaje = "ID de doctor inválido";
                    result.Errores.Add("El ID del doctor debe ser mayor a 0");
                    return result;
                }

                var appointments = await _appointmentRepo.GetByDoctorIdAsync(doctorId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
                _logger.LogInformation("Se obtuvieron {Count} citas para el doctor {DoctorId}", result.Datos.Count, doctorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del doctor";
                result.Errores.Add("Ocurrió un error al obtener las citas");
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            _logger.LogInformation("Obteniendo citas entre {StartDate} y {EndDate}", startDate, endDate);

            try
            {
                if (startDate == default || endDate == default || startDate > endDate)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Rango de fechas inválido";
                    if (startDate == default || endDate == default) result.Errores.Add("Las fechas de inicio y fin son requeridas");
                    if (startDate > endDate) result.Errores.Add("La fecha de inicio no puede ser posterior a la fecha de fin");
                    return result;
                }

                var appointments = await _appointmentRepo.GetByDateRangeAsync(startDate, endDate);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
                _logger.LogInformation("Se obtuvieron {Count} citas en el rango especificado", result.Datos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas por rango de fechas");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas por rango de fechas";
                result.Errores.Add("Ocurrió un error al obtener las citas");
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetUpcomingForPatientAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            _logger.LogInformation("Obteniendo citas próximas del paciente: {PatientId}", patientId);

            try
            {
                if (patientId <= 0)
                {
                    _logger.LogWarning("PatientId inválido: {PatientId}", patientId);
                    result.Exitoso = false;
                    result.Mensaje = "ID de paciente inválido";
                    result.Errores.Add("El ID del paciente debe ser mayor a 0");
                    return result;
                }

                var now = DateTime.Now;
                var appointments = await _appointmentRepo.GetByPatientIdAsync(patientId);
                var upcoming = appointments
                    .Where(a => a.AppointmentDate > now && a.StatusId != AppointmentStatus.Cancelled)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();

                result.Datos = upcoming.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas próximas obtenidas correctamente";
                _logger.LogInformation("Se obtuvieron {Count} citas próximas para el paciente {PatientId}", result.Datos.Count, patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas próximas para el paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas próximas para el paciente";
                result.Errores.Add("Ocurrió un error al obtener las citas");
            }
            return result;
        }

        // --- MÉTODOS PRIVADOS ---

        private async Task<OperationResult<AppointmentDto>> ValidateCreateAppointmentDto(CreateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto> { Exitoso = true };

            if (dto == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Los datos de la cita son requeridos.";
                result.Errores.Add("El cuerpo de la solicitud no puede ser nulo.");
                return result;
            }
            if (dto.PatientId <= 0)
            {
                result.Exitoso = false;
                result.Errores.Add("El ID del paciente debe ser un número positivo.");
            }
            if (dto.DoctorId <= 0)
            {
                result.Exitoso = false;
                result.Errores.Add("El ID del doctor debe ser un número positivo.");
            }
            if (dto.AppointmentDate == default)
            {
                result.Exitoso = false;
                result.Errores.Add("La fecha de la cita es obligatoria.");
            }

            if (!result.Exitoso)
            {
                result.Mensaje = "Datos de entrada inválidos.";
                return result;
            }

            if (!await _appointmentRepo.PatientExistsAsync(dto.PatientId))
            {
                result.Exitoso = false;
                result.Mensaje = "El paciente especificado no existe.";
                result.Errores.Add($"No se encontró un paciente con ID {dto.PatientId}.");
                return result;
            }

            if (!await _appointmentRepo.DoctorExistsAsync(dto.DoctorId))
            {
                result.Exitoso = false;
                result.Mensaje = "El doctor especificado no existe.";
                result.Errores.Add($"No se encontró un doctor con ID {dto.DoctorId}.");
                return result;
            }

            var dateValidation = await ValidateAppointmentDateAsync(dto.DoctorId, dto.AppointmentDate);
            if (!dateValidation.Exitoso)
            {
                return new OperationResult<AppointmentDto>
                {
                    Exitoso = false,
                    Mensaje = "La fecha de la cita no es válida.",
                    Errores = dateValidation.Errores
                };
            }

            return result;
        }

        private async Task<OperationResult> ValidateAppointmentDateAsync(int doctorId, DateTime date)
        {
            var result = new OperationResult { Exitoso = true };

            if (date < DateTime.Now)
            {
                result.Exitoso = false;
                result.Errores.Add("No se pueden crear citas en fechas pasadas.");
            }

            if (await _appointmentRepo.ExistsInTimeSlotAsync(doctorId, date))
            {
                result.Exitoso = false;
                result.Errores.Add("El doctor ya tiene una cita programada en ese horario.");
            }

            return result;
        }

        private static AppointmentDto MapToDto(Appointment a) => new()
        {
            AppointmentId = a.AppointmentId,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDate = a.AppointmentDate,
            StatusId = a.StatusId,
            CreatedAt = a.CreatedAt
        };

        internal static class AppointmentStatus
        {
            public const int Scheduled = 1;
            public const int Confirmed = 2;
            public const int Cancelled = 3;
        }
    }
}