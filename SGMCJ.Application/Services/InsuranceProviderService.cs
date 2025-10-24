using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Insurance;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Insurance;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Insurance;
using SGMCJ.Domain.Repositories.Users;

namespace SGMCJ.Application.Services
{
    public class InsuranceProviderService : IInsuranceProviderService
    {
        private readonly IInsuranceProviderRepository _repository;
        private readonly ILogger<InsuranceProviderService> _logger;

        public InsuranceProviderService(IInsuranceProviderRepository repository, ILogger<InsuranceProviderService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OperationResult<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto)
        {
            var result = new OperationResult<InsuranceProviderDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                var provider = new InsuranceProvider
                {
                    Name = dto.Name,
                    ContactPhone = dto.ContactPhone,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(provider);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Proveedor de seguro creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor de seguro");
                result.Exitoso = false;
                result.Mensaje = "Error al crear proveedor de seguro";
            }
            return result;
        }

        public async Task<OperationResult<List<InsuranceProviderDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<InsuranceProviderDto>>();
            try
            {
                var providers = await _repository.GetAllAsync();
                result.Datos = providers.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Proveedores obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores de seguro");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener proveedores";
            }
            return result;
        }

        private static InsuranceProviderDto MapToDto(InsuranceProvider p) => new()
        {
           // Id = p.Id,
            Name = p.Name,
            // ContactPhone = p.ContactPhone,
            Website = p.Website,
            IsActive = p.IsActive
        };

        public Task<OperationResult<InsuranceProviderDto>> UpdateAsync(UpdateInsuranceProviderDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<InsuranceProviderDto>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<List<InsuranceProviderDto>>> GetActiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}