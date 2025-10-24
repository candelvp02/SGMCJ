using SGMCJ.Application.Dto.Appointments;

namespace SGMCJ.Domain.Repositories.Ado.Appointments
{
    public interface IAppointmentAdoRepository
    {
        Task<List<AppointmentDto>> ListWithDetailsAsync();
        Task<List<AppointmentDto>> ListByDateRangeAsync(DateTime startDate, DateTime endDate);
      //  Task<bool> CancelAsync(int appointmentId, string reason);
        Task<bool> ConfirmAsync(int appointmentId);
        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);
    }
}