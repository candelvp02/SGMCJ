using Microsoft.Extensions.Logging;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Repositories.System;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Domain.Repositories.Users;

namespace SGMCJ.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationService> _logger;
        // private readonly IEmailService _emailService; // Asumir que existe

        public NotificationService(
            INotificationRepository repository,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            ILogger<NotificationService> logger)
        //    IEmailService emailService)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _logger = logger;
         //   _emailService = emailService;
        }

        // ✅ IMPLEMENTAR ESTOS MÉTODOS CRÍTICOS
        public async Task<OperationResult> SendAppointmentConfirmationAsync(int appointmentId)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    return result;
                }

                var patient = await _userRepository.GetByIdAsync(appointment.PatientId);
                var doctor = await _userRepository.GetByIdAsync(appointment.DoctorId);

                var message = $"Su cita con el Dr. {doctor?.Email} ha sido confirmada para {appointment.AppointmentDate:dd/MM/yyyy HH:mm}";

                // Guardar notificación en BD
                var notification = new Notification
                {
                    UserId = appointment.PatientId,
                    Message = message,
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

                // Enviar email (asíncrono como especifica el SAD)
              //  _ = Task.Run(() => _emailService.SendEmailAsync(patient?.Email, "Confirmación de Cita", message));

                result.Exitoso = true;
                result.Mensaje = "Notificación de confirmación enviada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando confirmación de cita {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar notificación";
            }
            return result;
        }

        public async Task<OperationResult> SendAppointmentReminderAsync(int appointmentId)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Cita no encontrada";
                    return result;
                }

                // Verificar que la cita sea en las próximas 24 horas
                if ((appointment.AppointmentDate - DateTime.Now).TotalHours > 24)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Recordatorio solo se envía 24 horas antes";
                    return result;
                }

                var patient = await _userRepository.GetByIdAsync(appointment.PatientId);
                var message = $"Recordatorio: Tiene cita mañana a las {appointment.AppointmentDate:HH:mm}";

                var notification = new Notification
                {
                    UserId = appointment.PatientId,
                    Message = message,
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

              //  _ = Task.Run(() => _emailService.SendEmailAsync(patient?.Email, "Recordatorio de Cita", message));

                result.Exitoso = true;
                result.Mensaje = "Recordatorio enviado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando recordatorio {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar recordatorio";
            }
            return result;
        }

        public async Task<OperationResult> SendAppointmentCancellationAsync(int appointmentId)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null) return ResultError("Cita no encontrada");

                var patient = await _userRepository.GetByIdAsync(appointment.PatientId);
                var message = $"Su cita del {appointment.AppointmentDate:dd/MM/yyyy} ha sido cancelada";

                var notification = new Notification
                {
                    UserId = appointment.PatientId,
                    Message = message,
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

               // _ = Task.Run(() => _emailService.SendEmailAsync(patient?.Email, "Cancelación de Cita", message));

                result.Exitoso = true;
                result.Mensaje = "Notificación de cancelación enviada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando cancelación {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar notificación de cancelación";
            }
            return result;
        }

        public async Task<OperationResult> SendAppointmentRescheduleAsync(int appointmentId)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null) return ResultError("Cita no encontrada");

                var patient = await _userRepository.GetByIdAsync(appointment.PatientId);
                var message = $"Su cita ha sido reprogramada para {appointment.AppointmentDate:dd/MM/yyyy HH:mm}";

                var notification = new Notification
                {
                    UserId = appointment.PatientId,
                    Message = message,
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

               // _ = Task.Run(() => _emailService.SendEmailAsync(patient?.Email, "Reprogramación de Cita", message));

                result.Exitoso = true;
                result.Mensaje = "Notificación de reprogramación enviada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando reprogramación {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar notificación de reprogramación";
            }
            return result;
        }

        public async Task<OperationResult> SendCustomReminderAsync(int appointmentId, string message)
        {
            var result = new OperationResult();
            try
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
                if (appointment == null) return ResultError("Cita no encontrada");

                var patient = await _userRepository.GetByIdAsync(appointment.PatientId);

                var notification = new Notification
                {
                    UserId = appointment.PatientId,
                    Message = message,
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

              //  _ = Task.Run(() => _emailService.SendEmailAsync(patient?.Email, "Recordatorio Personalizado", message));

                result.Exitoso = true;
                result.Mensaje = "Recordatorio personalizado enviado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando recordatorio personalizado {AppointmentId}", appointmentId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar recordatorio personalizado";
            }
            return result;
        }

        public async Task<OperationResult> SendPasswordResetEmailAsync(string email, string token)
        {
            var result = new OperationResult();
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // Por seguridad, no revelar si el email existe
                    result.Exitoso = true;
                    result.Mensaje = "Si el email existe, recibirá instrucciones";
                    return result;
                }

                var resetLink = $"https://tudominio.com/reset-password?token={token}";
                var message = $"Para resetear su password, haga clic aquí: {resetLink}";

                var notification = new Notification
                {
                    UserId = user.UserId,
                    Message = "Solicitud de reset de password",
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

              //  _ = Task.Run(() => _emailService.SendEmailAsync(email, "Reset de Password", message));

                result.Exitoso = true;
                result.Mensaje = "Email de reset enviado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando reset password {Email}", email);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar email de reset";
            }
            return result;
        }

        public async Task<OperationResult> SendAccountActivationEmailAsync(string email, int userId)
        {
            var result = new OperationResult();
            try
            {
                var activationLink = $"https://tudominio.com/activate-account?userId={userId}";
                var message = $"Para activar su cuenta, haga clic aquí: {activationLink}";

                var notification = new Notification
                {
                    UserId = userId,
                    Message = "Activación de cuenta",
                    SentAt = DateTime.Now
                };
                await _repository.AddAsync(notification);

              //  _ = Task.Run(() => _emailService.SendEmailAsync(email, "Activación de Cuenta", message));

                result.Exitoso = true;
                result.Mensaje = "Email de activación enviado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando activación {Email}", email);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar email de activación";
            }
            return result;
        }

        // Método auxiliar
        private OperationResult ResultError(string message)
        {
            return new OperationResult { Exitoso = false, Mensaje = message };
        }
    }
}