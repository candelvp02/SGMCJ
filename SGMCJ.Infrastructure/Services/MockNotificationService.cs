using Microsoft.Extensions.Logging;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Repositories.Appointments;

namespace SGMCJ.Infrastructure.Services
{
    public class MockNotificationService : INotificationService
    {
        private readonly ILogger<MockNotificationService> _logger;
        private readonly IAppointmentRepository _appointmentRepository;

        // Para tracking en tests
        private readonly List<string> _sentNotifications = new();

        public MockNotificationService(
            ILogger<MockNotificationService> logger,
            IAppointmentRepository appointmentRepository)
        {
            _logger = logger;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<OperationResult> SendAppointmentConfirmationAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando confirmacion de cita {AppointmentId}", appointmentId);

            // Simular procesamiento
            await Task.Delay(100);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
            {
                return new OperationResult
                {
                    Exitoso = false,
                    Mensaje = "Cita no encontrada"
                };
            }

            // Simular exito/error
            var success = new Random().Next(0, 100) < 90;

            if (success)
            {
                _sentNotifications.Add($"CONFIRMATION_{appointmentId}");
                _logger.LogInformation("Confirmacion enviada para cita {AppointmentId}", appointmentId);
                return new OperationResult
                {
                    Exitoso = true,
                    Mensaje = $"Confirmacion enviada al paciente {appointment.PatientId}"
                };
            }
            else
            {
                _logger.LogWarning("Error simulado enviando confirmacion {AppointmentId}", appointmentId);
                return new OperationResult
                {
                    Exitoso = false,
                    Mensaje = "Error simulado: No se pudo enviar la confirmacion"
                };
            }
        }

        public async Task<OperationResult> SendAppointmentReminderAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando recordatorio de cita {AppointmentId}", appointmentId);

            await Task.Delay(80);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            // Simular envio 24 hrs antes
            var hoursUntil = (appointment.AppointmentDate - DateTime.Now).TotalHours;

            _sentNotifications.Add($"REMINDER_{appointmentId}");
            _logger.LogInformation("Recordatorio enviado para cita {AppointmentId} ({HoursUntil}h antes)",
                appointmentId, hoursUntil);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Recordatorio enviado ({hoursUntil:F1}h antes)"
            };
        }

        public async Task<OperationResult> SendAppointmentCancellationAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando cancelacion de cita {AppointmentId}", appointmentId);

            await Task.Delay(120);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"CANCELLATION_{appointmentId}");
            _logger.LogInformation("Cancelacion enviada para cita {AppointmentId}", appointmentId);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Cancelacion notificada al paciente {appointment.PatientId}"
            };
        }

        public async Task<OperationResult> SendAppointmentRescheduleAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando reprogramacion de cita {AppointmentId}", appointmentId);

            await Task.Delay(100);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"RESCHEDULE_{appointmentId}");
            _logger.LogInformation("Reprogramacion enviada para cita {AppointmentId}", appointmentId);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Reprogramacion notificada para {appointment.AppointmentDate}"
            };
        }

        public async Task<OperationResult> SendCustomReminderAsync(int appointmentId, string message)
        {
            _logger.LogInformation("Enviando recordatorio personalizado para cita {AppointmentId}", appointmentId);

            await Task.Delay(90);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"CUSTOM_{appointmentId}");
            _logger.LogInformation("Recordatorio personalizado: {Message}", message);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Recordatorio personalizado enviado: {message}"
            };
        }

        public async Task<OperationResult> SendPasswordResetEmailAsync(string email, string token)
        {
            _logger.LogInformation("Enviando reset de password a {Email}", email);

            await Task.Delay(150);

            _sentNotifications.Add($"PASSWORD_RESET_{email}");
            _logger.LogInformation("Email de reset enviado a {Email}", email);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Email de recuperacion enviado a {email}"
            };
        }

        public async Task<OperationResult> SendAccountActivationEmailAsync(string email, int userId)
        {
            _logger.LogInformation("Enviando activacion de cuenta a {Email}", email);

            await Task.Delay(150);

            _sentNotifications.Add($"ACCOUNT_ACTIVATION_{userId}");
            _logger.LogInformation("Email de activacion enviado a {Email}", email);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Email de activacion enviado a {email}"
            };
        }

        // Metodos para testing
        public List<string> GetSentNotifications() => new(_sentNotifications);
        public void ClearSentNotifications() => _sentNotifications.Clear();
        public bool WasNotificationSent(string type, int appointmentId) =>
            _sentNotifications.Contains($"{type}_{appointmentId}");
    }
}
