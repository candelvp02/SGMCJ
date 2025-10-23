using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Medical;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Domain.Repositories.Users;

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

                var specialty = new Specialty
                {
                    SpecialtyName = dto.Name,
                    Description = dto.Description ?? string.Empty,
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

        private static SpecialtyDto MapToDto(Specialty s) => new()
        {
            Id = s.Id,
            Name = s.SpecialtyName,
            Description = s.Description
        };
    }
}