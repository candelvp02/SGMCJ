namespace SGMCJ.Domain.Base
{
    public abstract class Person : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}