using SGMCJ.Domain.Entities.Security;

namespace SGMCJ.Domain.Repositories.Security
{
    public interface IUsuarioAdoRepository
    {
        // Métodos específicos para ADO
        Task<List<Usuario>> ListarActivosAsync();
        Task<List<Usuario>> ListarPorRolAsync(string rol);
        Task<bool> ActualizarUltimoAccesoAsync(int usuarioId);
        Task<bool> ActivarUsuarioAsync(int usuarioId);
        Task<bool> DesactivarUsuarioAsync(int usuarioId);
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> AddAsync(Usuario entity);
    }
}