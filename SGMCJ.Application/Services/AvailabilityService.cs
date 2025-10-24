using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;

namespace SGMCJ.Application.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IDoctorAvailabilityRepository _repository;
        private readonly ILogger<AvailabilityService> _logger;

        public AvailabilityService(IDoctorAvailabilityRepository repository, ILogger<AvailabilityService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AvailabilityDto>> CreateAsync(CreateAvailabilityDto dto)
        {
            var result = new OperationResult<AvailabilityDto>();

            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                // Convertir DateTime a DateOnly y TimeSpan a TimeOnly
                var dateOnly = DateOnly.FromDateTime(dto.Date);
                var startTimeOnly = TimeOnly.FromTimeSpan(dto.StartTime);
                var endTimeOnly = TimeOnly.FromTimeSpan(dto.EndTime);

                // Verificar conflictos usando DateOnly y TimeOnly
                var hasConflict = await _repository.HasConflictAsync(
                    dto.DoctorId, dateOnly, startTimeOnly, endTimeOnly);

                if (hasConflict)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe una disponibilidad en ese horario";
                    return result;
                }

                var availability = new DoctorAvailability
                {
                    DoctorId = dto.DoctorId,
                    AvailableDate = dateOnly,
                    StartTime = startTimeOnly,
                    EndTime = endTimeOnly,
                    IsActive = true
                };

                await _repository.AddAsync(availability);

                result.Datos = new AvailabilityDto
                {
                    Id = availability.Id,
                    DoctorId = availability.DoctorId,
                    Date = availability.AvailableDate.ToDateTime(TimeOnly.MinValue),
                    StartTime = availability.StartTime.ToTimeSpan(),
                    EndTime = availability.EndTime.ToTimeSpan(),
                    IsAvailable = availability.IsActive
                };
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad creada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear disponibilidad para doctor {DoctorId}", dto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error interno al crear disponibilidad";
            }

            return result;
        }

        public async Task<OperationResult<AvailabilityDto>> UpdateAsync(UpdateAvailabilityDto dto)
        {
            var result = new OperationResult<AvailabilityDto>();

            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                var existing = await _repository.GetByIdAsync(dto.Id);
                if (existing == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Disponibilidad no encontrada";
                    return result;
                }

                // Convertir DateTime a DateOnly y TimeSpan a TimeOnly
                var dateOnly = DateOnly.FromDateTime(dto.Date);
                var startTimeOnly = TimeOnly.FromTimeSpan(dto.StartTime);
                var endTimeOnly = TimeOnly.FromTimeSpan(dto.EndTime);

                var otherAvailabilities = await _repository.GetByDoctorAndDateRangeAsync(
                    dto.DoctorId, dateOnly, dateOnly);

                var hasConflict = otherAvailabilities
                    .Where(a => a.Id != dto.Id)
                    .Any(a => !(a.EndTime <= startTimeOnly || a.StartTime >= endTimeOnly));

                if (hasConflict)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe otra disponibilidad en ese horario";
                    return result;
                }

                existing.DoctorId = dto.DoctorId;
                existing.AvailableDate = dateOnly;
                existing.StartTime = startTimeOnly;
                existing.EndTime = endTimeOnly;
                existing.IsActive = dto.IsAvailable;

                await _repository.UpdateAsync(existing);

                result.Datos = new AvailabilityDto
                {
                    Id = existing.Id,
                    DoctorId = existing.DoctorId,
                    Date = existing.AvailableDate.ToDateTime(TimeOnly.MinValue),
                    StartTime = existing.StartTime.ToTimeSpan(),
                    EndTime = existing.EndTime.ToTimeSpan(),
                    IsAvailable = existing.IsActive
                };
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar disponibilidad {Id}", dto?.Id);
                result.Exitoso = false;
                result.Mensaje = "Error interno al actualizar disponibilidad";
            }

            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();

            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Disponibilidad no encontrada";
                    return result;
                }

                await _repository.DeleteAsync(id);
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad eliminada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar disponibilidad {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error interno al eliminar disponibilidad";
            }

            return result;
        }

        public async Task<OperationResult<List<AvailabilityDto>>> GetByDoctorIdAsync(int doctorId)
        {
            var result = new OperationResult<List<AvailabilityDto>>();

            try
            {
                var availabilities = await _repository.GetByDoctorIdAsync(doctorId);
                result.Datos = availabilities.Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    Date = a.AvailableDate.ToDateTime(TimeOnly.MinValue),
                    StartTime = a.StartTime.ToTimeSpan(),
                    EndTime = a.EndTime.ToTimeSpan(),
                    IsAvailable = a.IsActive
                }).ToList();
                result.Exitoso = true;
                result.Mensaje = "Disponibilidades obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidades del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error interno al obtener disponibilidades";
            }

            return result;
        }

        public async Task<OperationResult<List<AvailabilityDto>>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var result = new OperationResult<List<AvailabilityDto>>();

            try
            {
                // Convertir DateTime a DateOnly
                var startDateOnly = DateOnly.FromDateTime(startDate);
                var endDateOnly = DateOnly.FromDateTime(endDate);

                var availabilities = await _repository.GetByDoctorAndDateRangeAsync(doctorId, startDateOnly, endDateOnly);
                result.Datos = availabilities.Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    Date = a.AvailableDate.ToDateTime(TimeOnly.MinValue),
                    StartTime = a.StartTime.ToTimeSpan(),
                    EndTime = a.EndTime.ToTimeSpan(),
                    IsAvailable = a.IsActive
                }).ToList();
                result.Exitoso = true;
                result.Mensaje = "Disponibilidades obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidades del doctor {DoctorId} entre {StartDate} y {EndDate}",
                    doctorId, startDate, endDate);
                result.Exitoso = false;
                result.Mensaje = "Error interno al obtener disponibilidades";
            }

            return result;
        }

        public async Task<OperationResult<bool>> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDate)
        {
            var result = new OperationResult<bool>();

            try
            {
                // Convertir DateTime a DateOnly y extraer TimeOnly
                var dateOnly = DateOnly.FromDateTime(appointmentDate);
                var timeOnly = TimeOnly.FromDateTime(appointmentDate);

                // Usar el método específico del repositorio
                var isAvailable = await _repository.IsAvailableAsync(doctorId, dateOnly, timeOnly);

                result.Datos = isAvailable;
                result.Exitoso = true;
                result.Mensaje = isAvailable ? "Doctor disponible" : "Doctor no disponible";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad del doctor {DoctorId} para {AppointmentDate}",
                    doctorId, appointmentDate);
                result.Exitoso = false;
                result.Mensaje = "Error interno al verificar disponibilidad";
            }

            return result;
        }

        public async Task<OperationResult<List<DateTime>>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var result = new OperationResult<List<DateTime>>();

            try
            {
                // Convertir DateTime a DateOnly
                var dateOnly = DateOnly.FromDateTime(date);

                var availabilities = await _repository.GetByDoctorAndDateRangeAsync(doctorId, dateOnly, dateOnly);
                var slots = new List<DateTime>();

                foreach (var availability in availabilities.Where(a => a.IsActive))
                {
                    var currentTimeSpan = availability.StartTime.ToTimeSpan();
                    var slotDuration = TimeSpan.FromMinutes(30); // Slots de 30 minutos
                    var endTimeSpan = availability.EndTime.ToTimeSpan();

                    while (currentTimeSpan.Add(slotDuration) <= endTimeSpan)
                    {
                        var slotDateTime = date.Date.Add(currentTimeSpan);
                        slots.Add(slotDateTime);
                        currentTimeSpan = currentTimeSpan.Add(slotDuration);
                    }
                }

                result.Datos = slots.OrderBy(s => s).ToList();
                result.Exitoso = true;
                result.Mensaje = "Slots disponibles obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener slots disponibles del doctor {DoctorId} para {Date}",
                    doctorId, date);
                result.Exitoso = false;
                result.Mensaje = "Error interno al obtener slots disponibles";
            }

            return result;
        }
    }
}