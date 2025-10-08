using SGMCJ.Domain.Entities.Security;

namespace SGMCJ.Domain.Repositories.Security
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioByNombreUsuarioAsync(string nombreUsuario);
        Task<Usuario?> GetUsuarioByEmailAsync(string email);
        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario);
        Task<bool> ExisteEmailAsync(string email);
        Task<List<Usuario>> GetUsuariosActivosAsync();
        Task<List<Usuario>> GetUsuariosByRolAsync(string nombreRol);
        Task<List<Usuario>> ListarActivosAsync();
        Task<List<Usuario>> ListarPorRolAsync(string rol);
    }
}