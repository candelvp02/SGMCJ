using SGMCJ.Domain.Base;
using SGMCJ.Domain.Dto;
using SGMCJ.Domain.Entities.Medical;


namespace SGMCJ.Application.Interfaces.Service
{
    public interface IPacienteService
    {
        // DTOs para APIs
        Task<OperationResult<List<PacienteDto>>> GetAllAsync();
        Task<OperationResult<PacienteDto>> GetByIdAsync(int id);
        Task<OperationResult<PacienteDto>> CreateAsync(PacienteDto pacienteDto);
        Task<OperationResult<PacienteDto>> UpdateAsync(PacienteDto pacienteDto);

        // Entidades para EF interno
        Task<OperationResult<Paciente>> CreateEntityAsync(Paciente paciente);
        Task<OperationResult<Paciente>> UpdateEntityAsync(Paciente paciente);

        // Métodos específicos
        Task<OperationResult<List<PacienteDto>>> BuscarPorNombreAsync(string texto);
        Task<OperationResult<PacienteDto>> ObtenerPorCedulaAsync(string cedula);
        Task<OperationResult<bool>> ExisteCedulaAsync(string cedula);
        Task<OperationResult<List<PacienteDto>>> ListarActivosAsync();
        Task GetByIdentificacionAsync(string identificacion);
    }
}