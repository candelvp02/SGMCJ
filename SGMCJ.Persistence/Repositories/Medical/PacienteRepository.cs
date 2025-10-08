using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Interfaces.Repositories;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
    {
        private new readonly SGMCJDbContext _context;

        public PacienteRepository(SGMCJDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Paciente?> GetByIdentificacionAsync(string identificacion)
        {
            return await _context.Pacientes
                .Include(p => p.Citas)
                .FirstOrDefaultAsync(p => p.Cedula == identificacion && !p.EstaEliminado);
        }

        public async Task<bool> ExistePacienteAsync(string identificacion)
        {
            return await _context.Pacientes
                .AnyAsync(p => p.Cedula == identificacion && !p.EstaEliminado);
        }

        public async Task<List<Paciente>> GetPacientesByNombreAsync(string nombre)
        {
            return await _context.Pacientes
                .Where(p => (p.Nombre.Contains(nombre) || p.Apellido.Contains(nombre)) && !p.EstaEliminado)
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Apellido)
                .ToListAsync();
        }

        public async Task<List<Paciente>> GetPacientesActivosAsync()
        {
            return await _context.Pacientes
                .Include(p => p.Citas)
                .Where(p => !p.EstaEliminado)
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Apellido)
                .ToListAsync();
        }
    }
}