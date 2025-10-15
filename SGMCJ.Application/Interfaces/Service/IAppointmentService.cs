using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IAppointmentService
    {
        Task<OperationResult<List<AppointmentDto>>> GetAllAsync();
        Task<OperationResult<AppointmentDto>> GetByIdAsync(int id);
        Task<OperationResult<AppointmentDto>> CreateAsync(CreateAppointmentDto appointmentDto);
        Task<OperationResult<AppointmentDto>> UpdateAsync(UpdateAppointmentDto appointmentDto);
        Task<OperationResult> CancelAsync(int appointmentId, string reason);
        Task<OperationResult> ConfirmAsync(int appointmentId);
        Task<OperationResult> RescheduleAsync(int appointmentId, DateTime newDate);
        Task<OperationResult<List<AppointmentDto>>> GetByPatientIdAsync(int patientId);
        Task<OperationResult<List<AppointmentDto>>> GetByDoctorIdAsync(int doctorId);
        Task<OperationResult<List<AppointmentDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<OperationResult<List<AppointmentDto>>> GetUpcomingForPatientAsync(int patientId);
    }
}