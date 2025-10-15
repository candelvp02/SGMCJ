//using SGMCJ.Domain.Dto;

//namespace SGMCJ.Domain.Repositories.Medical
//{
//    public interface ICitaAdoRepository
//    {
//        Task<List<CitaDto>> ListarConDetallesAsync();
//        Task<List<CitaDto>> ListarPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
//        Task<bool> CancelarCitaAsync(int citaId, string motivo);
//        Task<bool> ConfirmarCitaAsync(int citaId);
//        Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora);
//    }
//}


//using SGMCJ.Domain.Dto;

//namespace SGMCJ.Domain.Repositories.Medical
//{
//    public interface ICitaAdoRepository
//    {
//        Task<List<CitaDto>> ListarConDetallesAsync();
//        Task<List<CitaDto>> ListarPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
//        Task<bool> CancelarCitaAsync(int citaId, string motivo);
//        Task<bool> ConfirmarCitaAsync(int citaId);
//        Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora);
//    }
//}

using SGMCJ.Application.Dto.Appointments;

namespace SGMCJ.Domain.Repositories.Ado.Appointments
{
    public interface IAppointmentAdoRepository
    {
        Task<List<AppointmentDto>> ListWithDetailsAsync();
        Task<List<AppointmentDto>> ListByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> CancelAsync(int appointmentId, string reason);
        Task<bool> ConfirmAsync(int appointmentId);
        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);
    }
}