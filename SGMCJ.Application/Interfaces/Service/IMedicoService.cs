using SGMCJ.Domain.Dto;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Configuration;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IMedicoService
    {
        // DTOs para APIs
        Task<OperationResult<List<MedicoDto>>> GetAllAsync();
        Task<OperationResult<MedicoDto>> GetByIdAsync(int id);
        Task<OperationResult<MedicoDto>> CreateAsync(MedicoDto medicoDto);
        Task<OperationResult<MedicoDto>> UpdateAsync(MedicoDto medicoDto);

        // Entidades para EF interno
        Task<OperationResult<Medico>> CreateEntityAsync(Medico medico);
        Task<OperationResult<Medico>> UpdateEntityAsync(Medico medico);

        // Métodos específicos
        Task<OperationResult<List<MedicoDto>>> ListarPorEspecialidadAsync(Especialidad especialidad);
        Task<OperationResult<List<MedicoDto>>> ListarActivosAsync();
        Task<OperationResult<bool>> ExisteMedicoAsync(string cedula);
        Task<OperationResult<List<MedicoDto>>> GetByEspecialidadAsync(Especialidad especialidad);
    }
}