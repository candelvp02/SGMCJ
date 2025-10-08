using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface ICitaRepository : IBaseRepository<Cita>
    {
        Task<List<Cita>> GetByPacienteIdAsync(int pacienteId);
        Task<List<Cita>> GetByMedicoIdAsync(int medicoId);
        Task<List<Cita>> GetByFechaAsync(DateTime fecha);
        Task<List<Cita>> GetByEstadoAsync(string estado);
        Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora);
        Task<List<Cita>> GetCitasByPacienteAsync(int pacienteId);
        Task<List<Cita>> GetCitasProximasAsync(DateTime desde, DateTime hasta);
    }
}