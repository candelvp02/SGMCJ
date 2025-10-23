using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface INotificationService
    {
        //notificaciones rf2.2.5/3.1.11
        Task<OperationResult> SendAppointmentConfirmationAsync(int appointmentId);
        Task<OperationResult> SendAppointmentReminderAsync(int appointmentId);
        Task<OperationResult> SendAppointmentCancellationAsync(int appointmentId);
        Task<OperationResult> SendAppointmentRescheduleAsync(int appointmentId);
        Task<OperationResult> SendCustomReminderAsync(int appointmentId, string message);
        Task<OperationResult> SendPasswordResetEmailAsync(string email, string token);
        Task<OperationResult> SendAccountActivationEmailAsync(string email, int userId);
    }
}