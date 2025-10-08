using SGMCJ.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Security
{
    public class Rol : AuditEntity
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public ICollection<UsuarioRol> UsuarioRoles { get; set; }
        public Rol()
        {
            Nombre = string.Empty;
            Descripcion = string.Empty;
            UsuarioRoles = new List<UsuarioRol>();
        }
        public int ObtenerCantidadUsuarios()
        {
            return UsuarioRoles.Count;
        }
        public bool EsRolAdministrador()
        {
            return Nombre.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
        }
        public bool EsRolMedico()
        {
            return Nombre.Equals("Medico", StringComparison.OrdinalIgnoreCase);
        }
        public bool EsRolRecepcionista()
        {
            return Nombre.Equals("Recepcionista", StringComparison.OrdinalIgnoreCase);
        }
    }
}
