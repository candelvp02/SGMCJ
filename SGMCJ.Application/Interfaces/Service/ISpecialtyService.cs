using SGMCJ.Application.Dto.Medical;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface ISpecialtyService
    {
        //crud rf3.1.12 gestion de especialidades
        Task<OperationResult<SpecialtyDto>> CreateAsync(CreateSpecialtyDto dto);
        Task<OperationResult<SpecialtyDto>> UpdateAsync(UpdateSpecialtyDto dto);
        Task<OperationResult> DeleteAsync(short id);

        //consultas
        Task<OperationResult<List<SpecialtyDto>>> GetAllAsync();
        Task<OperationResult<SpecialtyDto>> GetByIdAsync(short id);
        Task<OperationResult<List<SpecialtyDto>>> GetActiveAsync();
        Task<OperationResult<bool>> ExistsAsync(short id);
        Task<OperationResult<bool>> ExistsByNameAsync(string name);
    }
}