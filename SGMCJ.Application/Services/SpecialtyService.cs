using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Medical;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;

namespace SGMCJ.Application.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository;
        private readonly ILogger<SpecialtyService> _logger;

        public SpecialtyService(ISpecialtyRepository repository, ILogger<SpecialtyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OperationResult<SpecialtyDto>> CreateAsync(CreateSpecialtyDto dto)
        {
            var result = new OperationResult<SpecialtyDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                // Verificar si ya existe una especialidad con el mismo nombre
                var exists = await _repository.ExistsByNameAsync(dto.SpecialtyName);
                if (exists)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe una especialidad con ese nombre";
                    return result;
                }

                var specialty = new Specialty
                {
                    SpecialtyName = dto.SpecialtyName,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(specialty);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Especialidad creada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear especialidad");
                result.Exitoso = false;
                result.Mensaje = "Error al crear especialidad";
            }
            return result;
        }

        public async Task<OperationResult<SpecialtyDto>> UpdateAsync(UpdateSpecialtyDto dto)
        {
            var result = new OperationResult<SpecialtyDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                var existing = await _repository.GetByIdAsync(dto.SpecialtyId);
                if (existing == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Especialidad no encontrada";
                    return result;
                }

                // Verificar si el nombre ya existe en otra especialidad
                var exists = await _repository.ExistsByNameAsync(dto.SpecialtyName);
                if (exists && existing.SpecialtyName != dto.SpecialtyName)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe una especialidad con ese nombre";
                    return result;
                }

                existing.SpecialtyName = dto.SpecialtyName;
                existing.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(existing);
                result.Datos = MapToDto(existing);
                result.Exitoso = true;
                result.Mensaje = "Especialidad actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar especialidad {Id}", dto?.SpecialtyId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar especialidad";
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(short id)
        {
            var result = new OperationResult();
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Especialidad no encontrada";
                    return result;
                }

                await _repository.DeleteAsync(existing);
                result.Exitoso = true;
                result.Mensaje = "Especialidad eliminada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar especialidad {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar especialidad";
            }
            return result;
        }

        public async Task<OperationResult<List<SpecialtyDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<SpecialtyDto>>();
            try
            {
                var specialties = await _repository.GetAllAsync();
                result.Datos = specialties.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Especialidades obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener especialidades");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener especialidades";
            }
            return result;
        }

        public async Task<OperationResult<SpecialtyDto>> GetByIdAsync(short id)
        {
            var result = new OperationResult<SpecialtyDto>();
            try
            {
                var specialty = await _repository.GetByIdAsync(id);
                if (specialty == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Especialidad no encontrada";
                    return result;
                }

                result.Datos = MapToDto(specialty);
                result.Exitoso = true;
                result.Mensaje = "Especialidad obtenida correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener especialidad {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener especialidad";
            }
            return result;
        }

        public async Task<OperationResult<List<SpecialtyDto>>> GetActiveAsync()
        {
            var result = new OperationResult<List<SpecialtyDto>>();
            try
            {
                var specialties = await _repository.GetActiveAsync();
                result.Datos = specialties.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Especialidades activas obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener especialidades activas");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener especialidades activas";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsAsync(short id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var exists = await _repository.ExistsAsync(id);
                result.Datos = exists;
                result.Exitoso = true;
                result.Mensaje = exists ? "La especialidad existe" : "La especialidad no existe";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de especialidad {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar existencia de especialidad";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsByNameAsync(string name)
        {
            var result = new OperationResult<bool>();
            try
            {
                var exists = await _repository.ExistsByNameAsync(name);
                result.Datos = exists;
                result.Exitoso = true;
                result.Mensaje = exists ? "Ya existe una especialidad con ese nombre" : "El nombre está disponible";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar nombre de especialidad {Name}", name);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar nombre de especialidad";
            }
            return result;
        }

        private static SpecialtyDto MapToDto(Specialty s) => new()
        {
            SpecialtyId = s.SpecialtyId,
            SpecialtyName = s.SpecialtyName,
            IsActive = s.IsActive
        };
    }
}