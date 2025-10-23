using SGMCJ.Application.Dto.Insurance;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IInsuranceProviderService
    {
        //crud rrf3.1.12 gestion de proveedores de salud
        Task<OperationResult<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto);
        Task<OperationResult<InsuranceProviderDto>> UpdateAsync(UpdateInsuranceProviderDto dto);
        Task<OperationResult> DeleteAsync(int id);

        // consultas
        Task<OperationResult<List<InsuranceProviderDto>>> GetAllAsync();
        Task<OperationResult<InsuranceProviderDto>> GetByIdAsync(int id);
        Task<OperationResult<List<InsuranceProviderDto>>> GetActiveAsync();
        Task<OperationResult<bool>> ExistsAsync(int id);
    }
}