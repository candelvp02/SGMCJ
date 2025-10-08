using SGMCJ.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Security
{
    public class Usuario : AuditEntity
    {
        public int Id { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public ICollection<UsuarioRol> UsuarioRoles { get; set; }

        public Usuario()
        {
            EsActivo = true;
            UsuarioRoles = new List<UsuarioRol>();
        }

        public List<string> ObtenerNombresRoles()
        {
            return UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();
        }

        public bool TieneRol(string nombreRol)
        {
            return UsuarioRoles.Any(ur => ur.Rol.Nombre.Equals(nombreRol, StringComparison.OrdinalIgnoreCase));
        }

        public bool EsAdministrador()
        {
            return TieneRol("Administrador");
        }

        public bool EsMedico()
        {
            return TieneRol("Medico");
        }

        public bool EsRecepcionista()
        {
            return TieneRol("Recepcionista");
        }

        public void ActualizarUltimoAcceso()
        {
            UltimoAcceso = DateTime.Now;
        }

        public void Activar()
        {
            EsActivo = true;
            FechaModificacion = DateTime.Now;
        }

        public void Desactivar()
        {
            EsActivo = false;
            FechaModificacion = DateTime.Now;
        }

        public TimeSpan TiempoDesdeUltimoAcceso()
        {
            return DateTime.Now - UltimoAcceso;
        }
    }
}