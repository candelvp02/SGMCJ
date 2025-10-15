namespace SGMCJ.Application.Dto.System
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
    }
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}