namespace SGMCJ.Domain.Dto
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}