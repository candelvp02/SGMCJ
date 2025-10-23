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
        private readonly IAvailabilityRepository _repository;
        private readonly ILogger<AvailabilityService> _logger;

        public AvailabilityService(IAvailabilityRepository repository, ILogger<AvailabilityService> logger)
        {
            _repository = repository;
            _logger = logger;
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

                var availability = new Availability
                {
                    DoctorId = dto.DoctorId,
                    Date = dto.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(availability);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad creada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear disponibilidad para doctor {DoctorId}", dto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al crear disponibilidad";
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

                existing.DoctorId = dto.DoctorId;
                existing.Date = dto.Date;
                existing.StartTime = dto.StartTime;
                existing.EndTime = dto.EndTime;
                existing.IsAvailable = dto.IsAvailable;
                existing.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(existing);
                result.Datos = MapToDto(existing);
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar disponibilidad {Id}", dto?.Id);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar disponibilidad";
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

                await _repository.DeleteAsync(existing);
                result.Exitoso = true;
                result.Mensaje = "Disponibilidad eliminada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar disponibilidad {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar disponibilidad";
            }
            return result;
        }

        public async Task<OperationResult<List<AvailabilityDto>>> GetByDoctorAsync(int doctorId)
        {
            var result = new OperationResult<List<AvailabilityDto>>();
            try
            {
                var availabilities = await _repository.GetByDoctorIdAsync(doctorId);
                result.Datos = availabilities.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Disponibilidades obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener disponibilidades del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener disponibilidades";
            }
            return result;
        }

        private static AvailabilityDto MapToDto(Availability a) => new()
        {
            Id = a.Id,
            DoctorId = a.DoctorId,
            Date = a.Date,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            IsAvailable = a.IsAvailable
        };
    }
}