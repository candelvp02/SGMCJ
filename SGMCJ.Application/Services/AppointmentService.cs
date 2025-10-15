using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
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
            try
            {
                var appointments = await _appointmentRepo.GetAllWithDetailsAsync();
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las citas");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas";
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<AppointmentDto>();
            try
            {
                var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(id);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    result.Errores.Add("Cita no encontrada");
                    return result;
                }

                result.Datos = MapToDto(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita obtenida correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cita {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener cita";
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "El cuerpo de la solicitud es requerido.";
                    result.Errores.Add("El cuerpo de la solicitud es requerido.");
                    return result;
                }

                if (dto.PatientId <= 0 || dto.DoctorId <= 0)
                {
                    result.Exitoso = false;
                    result.Mensaje = "PatientId y DoctorId son obligatorios.";
                    result.Errores.Add("PatientId y DoctorId son obligatorios.");
                    return result;
                }

                if (dto.AppointmentDate == default)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La AppointmentDate de la cita es obligatoria.";
                    result.Errores.Add("La AppointmentDate de la cita es obligatoria.");
                    return result;
                }

                if (dto.AppointmentDate <= DateTime.Now.AddMinutes(10))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita debe programarse con al menos 10 minutos de antelación.";
                    result.Errores.Add("La cita debe programarse con al menos 10 minutos de antelación.");
                    return result;
                }

                var patient = await _appointmentRepo.GetByPatientIdAsync(dto.PatientId);
                if (!patient.Any())
                {
                    result.Exitoso = false;
                    result.Mensaje = "El paciente no existe.";
                    result.Errores.Add("El paciente no existe.");
                    return result;
                }

                var doctor = await _appointmentRepo.GetByDoctorIdAsync(dto.DoctorId);
                if (!doctor.Any())
                {
                    result.Exitoso = false;
                    result.Mensaje = "El médico no existe.";
                    result.Errores.Add("El médico no existe.");
                    return result;
                }

                var isAvailable = await _appointmentRepo.ExistsInTimeSlotAsync(dto.DoctorId, dto.AppointmentDate);
                if (isAvailable)
                {
                    result.Exitoso = false;
                    result.Mensaje = "El médico ya tiene una cita en ese horario.";
                    result.Errores.Add("El médico ya tiene una cita en ese horario.");
                    return result;
                }

                var appointment = new SGMCJ.Domain.Entities.Appointments.Appointment
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    StatusId = 1,
                    Reason = dto.Reason,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now
                };

                var created = await _appointmentRepo.AddAsync(appointment);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Cita creada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear la cita. PatientId={PatientId}, DoctorId={DoctorId}", dto?.PatientId, dto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error interno al crear la cita.";
            }
            return result;
        }

        public async Task<OperationResult<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();
            try
            {
                var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    result.Errores.Add("Cita no encontrada");
                    return result;
                }

                appointment.AppointmentDate = dto.AppointmentDate;
                appointment.StatusId = dto.StatusId;
                appointment.Notes = dto.Notes;
                appointment.UpdatedAt = DateTime.Now;

                await _appointmentRepo.UpdateAsync(appointment);

                result.Datos = MapToDto(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita actualizada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cita {AppointmentId}", dto.AppointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar cita";
            }
            return result;
        }

        public async Task<OperationResult> CancelAsync(int appointmentId, string reason)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add("La cita no existe.");
                    return result;
                }

                appointment.StatusId = 3;
                appointment.Notes = string.IsNullOrWhiteSpace(reason)
                    ? appointment.Notes
                    : $"{appointment.Notes}\nCancelación: {reason}";
                appointment.UpdatedAt = DateTime.Now;

                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita cancelada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar la cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error interno al cancelar la cita.";
            }
            return result;
        }

        public async Task<OperationResult> ConfirmAsync(int appointmentId)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add("La cita no existe.");
                    return result;
                }

                appointment.StatusId = 2;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita confirmada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar la cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error interno al confirmar la cita.";
            }
            return result;
        }

        public async Task<OperationResult> RescheduleAsync(int appointmentId, DateTime newDate)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add("La cita no existe.");
                    return result;
                }

                if (newDate <= DateTime.Now.AddMinutes(10))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La nueva fecha/hora debe ser a futuro.";
                    result.Errores.Add("La nueva fecha/hora debe ser a futuro.");
                    return result;
                }

                var isAvailable = await _appointmentRepo.ExistsInTimeSlotAsync(appointment.DoctorId, newDate);
                if (isAvailable)
                {
                    result.Exitoso = false;
                    result.Mensaje = "El médico ya tiene una cita en ese horario.";
                    result.Errores.Add("El médico ya tiene una cita en ese horario.");
                    return result;
                }

                appointment.AppointmentDate = newDate;
                appointment.StatusId = 1;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita reprogramada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar la cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrió un error interno al reprogramar la cita.";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByPatientIdAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _appointmentRepo.GetByPatientIdAsync(patientId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del paciente";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDoctorIdAsync(int doctorId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _appointmentRepo.GetByDoctorIdAsync(doctorId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del médico {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del médico";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _appointmentRepo.GetByDateRangeAsync(startDate, endDate);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas por rango de fechas");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas por rango de fechas";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetUpcomingForPatientAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var now = DateTime.Now;
                var appointments = await _appointmentRepo.GetByPatientIdAsync(patientId);
                var upcoming = appointments.Where(a => a.AppointmentDate > now).OrderBy(a => a.AppointmentDate).ToList();
                result.Datos = upcoming.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas próximas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas próximas para el paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas próximas para el paciente";
            }
            return result;
        }

        private static AppointmentDto MapToDto(SGMCJ.Domain.Entities.Appointments.Appointment a) => new()
        {
            AppointmentId = a.AppointmentId,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDate = a.AppointmentDate,
            StatusId = a.StatusId,
            Reason = a.Reason,
            Notes = a.Notes,
            CreatedAt = a.CreatedAt
        };
    }
}