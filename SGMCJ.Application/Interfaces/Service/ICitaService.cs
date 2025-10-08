using SGMCJ.Domain.Dto;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface ICitaService
    {
        Task<OperationResult<List<CitaDto>>> GetAllAsync();
        Task<OperationResult<CitaDto>> GetByIdAsync(int id);
        Task<OperationResult<CitaDto>> CreateAsync(CitaDto citaDto);
        Task<OperationResult<CitaDto>> UpdateAsync(CitaDto citaDto);
        Task<OperationResult<Cita>> CreateEntityAsync(Cita cita);
        Task<OperationResult<Cita>> UpdateEntityAsync(Cita cita);
        Task<OperationResult<List<CitaDto>>> GetByPacienteIdAsync(int pacienteId);
        Task<OperationResult<List<CitaDto>>> GetByMedicoIdAsync(int medicoId);
        Task<OperationResult<List<CitaDto>>> GetByFechaAsync(DateTime fecha);
        Task<OperationResult> CancelarCitaAsync(int citaId, string motivo);
        Task<OperationResult> ConfirmarCitaAsync(int citaId);
        Task<OperationResult> ReprogramarCitaAsync(int id, DateTime nuevaFecha);
    }
}