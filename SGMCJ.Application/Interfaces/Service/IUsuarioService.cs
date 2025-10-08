using SGMCJ.Domain.Base;
using SGMCJ.Domain.Dto;
using SGMCJ.Domain.Entities.Security;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IUsuarioService
    {
        // DTOs para APIs
        Task<OperationResult<List<UsuarioDto>>> GetAllAsync();
        Task<OperationResult<UsuarioDto>> GetByIdAsync(int id);
        Task<OperationResult<UsuarioDto>> CreateAsync(UsuarioDto usuarioDto);
        Task<OperationResult<UsuarioDto>> UpdateAsync(UsuarioDto usuarioDto);

        // Entidades para EF interno
        Task<OperationResult<Usuario>> CreateEntityAsync(Usuario usuario);
        Task<OperationResult<Usuario>> UpdateEntityAsync(Usuario usuario);

        // Métodos de negocio
        Task<OperationResult<List<UsuarioDto>>> ListarActivosAsync();
        Task<OperationResult<List<UsuarioDto>>> ListarPorRolAsync(string rol);
        Task<OperationResult> ActualizarUltimoAccesoAsync(int usuarioId);
        Task<OperationResult> ActivarUsuarioAsync(int usuarioId);
        Task<OperationResult> DesactivarUsuarioAsync(int usuarioId);
        Task<OperationResult<UsuarioDto>> GetByNombreUsuarioAsync(string nombreUsuario);
        Task<OperationResult<UsuarioDto>> GetByEmailAsync(string email);
        Task<OperationResult<bool>> ValidarCredencialesAsync(string nombreUsuario, string password);
        Task<OperationResult> DeleteAsync(int id);
    }
}