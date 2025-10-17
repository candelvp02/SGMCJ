using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
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
            catch(Exception ex)
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
                    result.Errores.Add("Cita no enconctrada");
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
            var  result = new OperationResult<AppointmentDto>();
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
                    result.Mensaje = "La fecha de la cita es obligatoria.";
                    result.Errores.Add("La fecha de la cita es obligatoria.");
                    return result;
                }
                // validacion existencia de paciente y doctor
                if (!await _appointmentRepo.PatientExistsAsync(dto.PatientId))   //OJO AQUIIIIIII AGREGAR EN IAPPOINTMENTREPOSITORY
                {
                    result.Exitoso = false;
                    result.Mensaje = "El paciente no existe.";
                    result.Errores.Add("El paciente no existe.");
                    return result;
                }
                if (!await _appointmentRepo.DoctorExistsAsync(dto.DoctorId)) // OJO AQUIIII AGREGAR EN IAPPOINTMENTREPOSITORY.CS
                {
                    result.Exitoso = false;
                    result.Mensaje = "El doctor no existe.";
                    result.Errores.Add("El doctor no existe.");
                    return result;
                }
                // valicacion de reglas de negocio de appointment
                var validation = await ValidateAppointmentDateAsync(dto.DoctorId, dto.AppointmentDate);
                if (!validation.Exitoso)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Validacion fallida.";
                    result.Errores.AddRange(validation.Errores);
                    return result;
                }
                var appointment = new SGMCJ.Domain.Entities.Appointments.Appointment
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
                result.Mensaje = "Cita creada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear la cita. PatientId={PatientId}, DoctorId={DoctorId}", dto?.PatientId, dto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Ocurrio un error interno al crear la cita.";
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
         public async Task<OperationResult> CancelAsync(int appointmentId)
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
                appointment.StatusId = AppointmentStatus.Cancelled; // OJO NO EXISTEEEE AGREGAR
                appointment.UpdatedAt = DateTime.Now;

                await _appointmentRepo.UpdateAsync(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita cancelada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {ApppointmentId}", appointmentId);
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
                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add("La cita no existe.");
                    return result;
                }
                appointment.StatusId = AppointmentStatus.Confirmed; // OJO NO EXISTEEEE AGREGAR
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita confirmada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar cita {AppointmentId}", appointmentId);
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
                var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "La cita no existe.";
                    result.Errores.Add("La cita no existe.");
                    return result;
                }
                var validation = await ValidateAppointmentDateAsync(appointment.DoctorId, newDate);
                if (!validation.Exitoso)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Validacion fallida.";
                    result.Errores.AddRange(validation.Errores);
                    return result;
                }
                appointment.AppointmentDate = newDate;
                appointment.StatusId = AppointmentStatus.Scheduled; // OJO NO EXISTEEEE AGREGAR
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepo.UpdateAsync(appointment);
                result.Exitoso = true;
                result.Mensaje = "Cita reprogramada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al reprogramar cita";
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
                result.Mensaje = "Citas proximas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas proximas para el paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas proximas para el paciente";
            }
            return result;
        }
        // metodos privado de validacion
        private async Task<OperationResult> ValidateAppointmentDateAsync(int doctorId, DateTime date)
        {
            var result = new OperationResult();
            if (date <= DateTime.Now.AddMinutes(10))
            {
                result.Errores.Add("La cita debe programarse con al menos 10 minutos de antelacion.");
                return result;
            }
            if (await _appointmentRepo.ExistsInTimeSlotAsync(doctorId, date))
            {
                result.Errores.Add("El doctor ya tiene una cita en ese horario.");
                return result;
            }
            result.Exitoso = true;
            return result;
        }
        // mapeo
        private static AppointmentDto MapToDto(SGMCJ.Domain.Entities.Appointments.Appointment a) => new()
        {
            AppointmentId = a.AppointmentId,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDate = a.AppointmentDate,
            StatusId = a.StatusId,
            CreatedAt = a.CreatedAt
        };
    }
    // constantes para evitar numeros magicos
    internal static class AppointmentStatus
    {
        public const int Scheduled = 1;
        public const int Confirmed = 2;
        public const int Cancelled = 3;
    }
}