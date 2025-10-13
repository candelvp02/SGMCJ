using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Security;
using SGMCJ.Domain.Repositories.Security;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Security
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private new readonly SGMCJDbContext _context;

        public UsuarioRepository(SGMCJDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<bool> ExisteEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> GetUsuarioByNombreUsuarioAsync(string nombreUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> GetUsuariosActivosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> GetUsuariosByRolAsync(string nombreRol)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ListarActivosAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ListarPorRolAsync(string rol)
        {
            throw new NotImplementedException();
        }

        //public async Task<Usuario?> GetUsuarioByNombreUsuarioAsync(string nombreUsuario)
        //{
        //    return await _context.Usuarios
        //        .Include(u => u.UsuarioRoles)
        //        .ThenInclude(ur => ur.Rol)
        //        .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && !u.EstaEliminado);
        //}

        //public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        //{
        //    return await _context.Usuarios
        //        .Include(u => u.UsuarioRoles)
        //        .ThenInclude(ur => ur.Rol)
        //        .FirstOrDefaultAsync(u => u.Email == email && !u.EstaEliminado);
        //}
        //public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
        //{
        //    return await _context.Usuarios
        //        .AnyAsync(u => u.NombreUsuario == nombreUsuario && !u.EstaEliminado);
        //}

        //public async Task<bool> ExisteEmailAsync(string email)
        //{
        //    return await _context.Usuarios
        //        .AnyAsync(u => u.Email == email && !u.EstaEliminado);
        //}

        //public async Task<List<Usuario>> GetUsuariosActivosAsync()
        //{
        //    return await _context.Usuarios
        //        .Include(u => u.UsuarioRoles)
        //        .ThenInclude(ur => ur.Rol)
        //        .Where(u => u.EsActivo && !u.EstaEliminado)
        //        .ToListAsync();
        //}

        //public async Task<List<Usuario>> GetUsuariosByRolAsync(string nombreRol)
        //{
        //    return await _context.Usuarios
        //        .Include(u => u.UsuarioRoles)
        //        .ThenInclude(ur => ur.Rol)
        //        .Where(u => u.UsuarioRoles.Any(ur => ur.Rol.Nombre == nombreRol) && !u.EstaEliminado)
        //        .ToListAsync();
        //}

        //public async Task<List<Usuario>> ListarActivosAsync()
        //{
        //    return await GetUsuariosActivosAsync();
        //}

        //public async Task<List<Usuario>> ListarPorRolAsync(string rol)
        //{
        //    return await GetUsuariosByRolAsync(rol);
        //}
    }
}