using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Base
{
    public abstract class AuditEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public string? CreadoPor { get; set; }
        public string? ModificadoPor { get; set; }
        public bool EstaEliminado { get; set; } = false;
    }
}