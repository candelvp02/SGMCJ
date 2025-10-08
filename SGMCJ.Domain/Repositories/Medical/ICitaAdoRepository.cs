using SGMCJ.Domain.Dto;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface ICitaAdoRepository
    {
        Task<List<CitaDto>> ListarConDetallesAsync();
        Task<List<CitaDto>> ListarPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<bool> CancelarCitaAsync(int citaId, string motivo);
        Task<bool> ConfirmarCitaAsync(int citaId);
        Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora);
    }
}