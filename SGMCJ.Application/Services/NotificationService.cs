using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.System;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.System;
using SGMCJ.Domain.Repositories.System;

namespace SGMCJ.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository repository, ILogger<NotificationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OperationResult<NotificationDto>> SendAsync(CreateNotificationDto dto)
        {
            var result = new OperationResult<NotificationDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                var notification = new Notification
                {
                    UserId = dto.UserId,
                    TypeId = dto.TypeId,
                    Message = dto.Message,
                    IsRead = false,
                    SentAt = DateTime.Now
                };

                var created = await _repository.AddAsync(notification);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Notificación enviada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación al usuario {UserId}", dto?.UserId);
                result.Exitoso = false;
                result.Mensaje = "Error al enviar notificación";
            }
            return result;
        }

        private static NotificationDto MapToDto(Notification n) => new()
        {
            Id = n.Id,
            UserId = n.UserId,
            TypeId = n.TypeId,
            Message = n.Message,
            IsRead = n.IsRead,
            SentAt = n.SentAt
        };
    }
}