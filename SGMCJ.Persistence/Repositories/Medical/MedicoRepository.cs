using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Interfaces.Repositories;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public class MedicoRepository : BaseRepository<Medico>, IMedicoRepository
    {
        private new readonly SGMCJDbContext _context;

        public MedicoRepository(SGMCJDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<bool> ExisteMedicoAsync(string cedula)
        {
            throw new NotImplementedException();
        }

        public Task<List<Medico>> GetByEspecialidadAsync(string especialidad)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<Medico>> GetByEspecialidadAsync(string especialidad)
        //{
        //    if (!Enum.TryParse<Especialidad>(especialidad, true, out var especialidadEnum))
        //    {
        //        return new List<Medico>();
        //    }

        //    return await _context.Medicos
        //        .Where(m => m.Especialidad == especialidadEnum && m.EsActivo)
        //        .ToListAsync();
        //}

        //public async Task<bool> ExisteMedicoAsync(string cedula)
        //{
        //    return await _context.Medicos
        //        .AnyAsync(m => m.Cedula == cedula);
        //}
    }
}