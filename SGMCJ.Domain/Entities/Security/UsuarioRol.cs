using SGMCJ.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Security
{
    public class UsuarioRol : AuditEntity
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
        public DateTime FechaAsignacion { get; set; }
        public UsuarioRol()
        {
            FechaAsignacion = DateTime.Now;
        }
        public string ObtenerInformacionCompleta()
        {
            return $"{Usuario?.NombreUsuario} - {Rol?.Nombre}";
        }
        public TimeSpan TiempoConRol()
        {
            return DateTime.Now - FechaAsignacion;
        }
    }
}