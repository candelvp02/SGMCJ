using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Domain.Repositories.Users;

namespace SGMCJ.Application.Services
{
    public class AppointmentService(
        IAppointmentRepository repository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        ILogger<AppointmentService> logger) : IAppointmentService
    {
        private readonly IAppointmentRepository _repository = repository;
        private readonly ILogger<AppointmentService> _logger = logger;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;

        public async Task<OperationResult<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();

            try
            {
                if (!ValidateBasicData(dto, result))
                    return result;

                if (!ValidateDateTime(dto.AppointmentDate, result))
                    return result;

                if (!await PatientExists(dto.PatientId, result))
                    return result;

                if (!await DoctorExists(dto.DoctorId, result))
                    return result;

                if (!await IsDoctorAvailable(dto.DoctorId, dto.AppointmentDate, result))
                    return result;

                if (!await IsPatientAvailable(dto.PatientId, dto.AppointmentDate, result))
                    return result;

                var appointment = new Appointment
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    StatusId = 1,
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(appointment);

                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Cita creada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cita");
                result.Exitoso = false;
                result.Mensaje = "Error al crear cita";
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();

            try
            {
                var appointmentExists = await _repository.ExistsAsync(id);
                if (!appointmentExists)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    return result;
                }

                await _repository.GetByIdAsync(id);

                result.Exitoso = true;
                result.Mensaje = "Cita eliminada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cita {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar cita";
            }

            return result;
        }

        public async Task<OperationResult> CancelAsync(int appointmentId)
        {
            var result = new OperationResult();

            try
            {
                var appointment = await _repository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no existe";
                    return result;
                }

                if (appointment.StatusId == 3)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita ya está cancelada";
                    return result;
                }

                var hoursUntil = (appointment.AppointmentDate - DateTime.Now).TotalHours;
                if (hoursUntil < 24)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Debe cancelar con al menos 24 hrs de anticipacion";
                    return result;
                }

                appointment.StatusId = 3;
                appointment.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita cancelada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {Id}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al cancelar cita";
            }
            return result;
        }

        public async Task<OperationResult> ConfirmAsync(int appointmentId)
        {
            var result = new OperationResult();

            try
            {
                var appointment = await _repository.GetByIdWithDetailsAsync(appointmentId);

                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no existe";
                    return result;
                }

                if (appointment.StatusId != 1)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Solo se pueden confirmar citas pendientes";
                    return result;
                }

                appointment.StatusId = 2;
                appointment.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita confirmada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar cita {Id}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al confirmar cita";
            }
            return result;
        }

        public async Task<OperationResult> RescheduleAsync(int appointmentId, DateTime newDate)
        {
            var result = new OperationResult();

            try
            {
                var appointment = await _repository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no existe";
                    return result;
                }

                if (appointment.StatusId == 3)
                {
                    result.Exitoso = false;
                    result.Mensaje = "No se puede reprogramar una cita cancelada";
                    return result;
                }

                if (!ValidateDateTime(newDate, result))
                    return result;

                if (!await IsDoctorAvailable(appointment.DoctorId, newDate, result))
                    return result;

                appointment.AppointmentDate = newDate;
                appointment.StatusId = 1;
                appointment.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(appointment);

                result.Exitoso = true;
                result.Mensaje = "Cita reprogramada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {Id}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al reprogramar cita";
            }

            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _repository.GetAllWithDetailsAsync();
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas");
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
                var appointment = await _repository.GetByIdWithDetailsAsync(id);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
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

        public async Task<OperationResult<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto)
        {
            var result = new OperationResult<AppointmentDto>();
            try
            {
                var appointment = await _repository.GetByIdWithDetailsAsync(dto.AppointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    return result;
                }

                appointment.AppointmentDate = dto.AppointmentDate;
                appointment.StatusId = dto.StatusId;
                appointment.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(appointment);

                result.Datos = MapToDto(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cita {Id}", dto.AppointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar cita";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByPatientIdAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _repository.GetByPatientIdWithDetailsAsync(patientId);
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
                var appointments = await _repository.GetByDoctorIdAsync(doctorId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del doctor";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _repository.GetByDateRangeAsync(startDate, endDate);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas en rango de fechas");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas en rango de fechas";
            }
            return result;
        }

        public async Task<OperationResult<List<AppointmentDto>>> GetUpcomingForPatientAsync(int patientId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _repository.GetUpcomingAppointmentsAsync(patientId);
                result.Datos = appointments.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas futuras obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas futuras del paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas futuras del paciente";
            }
            return result;
        }

        private static bool ValidateBasicData(CreateAppointmentDto dto, OperationResult result)
        {
            if (dto == null || dto.PatientId <= 0 || dto.DoctorId <= 0)
            {
                result.Exitoso = false;
                result.Mensaje = "Datos inválidos: PatientId y DoctorId son requeridos";
                return false;
            }
            return true;
        }

        private static bool ValidateDateTime(DateTime appointmentDate, OperationResult result)
        {
            if (appointmentDate <= DateTime.Now.AddMinutes(10))
            {
                result.Exitoso = false;
                result.Mensaje = "La cita debe ser al menos 10 min en el futuro";
                return false;
            }

            var time = appointmentDate.TimeOfDay;
            if (time < new TimeSpan(8, 0, 0) || time > new TimeSpan(20, 0, 0))
            {
                result.Exitoso = false;
                result.Mensaje = "La cita debe ser entre las 8:00 AM y las 8:00 PM";
                return false;
            }

            if (appointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                result.Exitoso = false;
                result.Mensaje = "No se permiten citas los domingos";
                return false;
            }
            return true;
        }

        private async Task<bool> PatientExists(int patientId, OperationResult result)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Paciente no existe";
                return false;
            }
            return true;
        }

        private async Task<bool> DoctorExists(int doctorId, OperationResult result)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Doctor no existe";
                return false;
            }
            return true;
        }

        private async Task<bool> IsDoctorAvailable(int doctorId, DateTime appointmentDate, OperationResult result)
        {
            var hasConflict = await _repository.ExistsInTimeSlotAsync(doctorId, appointmentDate);
            if (hasConflict)
            {
                result.Exitoso = false;
                result.Mensaje = "El doctor no está disponible en la fecha y hora seleccionadas";
                return false;
            }
            return true;
        }

        private async Task<bool> IsPatientAvailable(int patientId, DateTime appointmentDate, OperationResult result)
        {
            var patientAppointments = await _repository.GetByPatientIdAsync(patientId);
            var hasSameDay = patientAppointments.Any(a => a.AppointmentDate.Date == appointmentDate.Date && a.StatusId != 3);

            if (hasSameDay)
            {
                result.Exitoso = false;
                result.Mensaje = "El paciente ya tiene una cita programada para la misma fecha";
                return false;
            }
            return true;
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
    }
}