using SGMCJ.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMCJ.Domain.Entities.System
{
    [Table("Notifications", Schema = "system")]

    public partial class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime? SentAt { get; set; }

        public virtual User? User { get; set; }
    }
}