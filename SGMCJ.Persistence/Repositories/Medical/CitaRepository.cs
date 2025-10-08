using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Entities;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Interfaces.Repositories;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public class CitaRepository : BaseRepository<Cita>, ICitaRepository
    {
        private new readonly SGMCJDbContext _context;

        public CitaRepository(SGMCJDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Cita>> GetByPacienteIdAsync(int pacienteId)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.PacienteId == pacienteId && !c.EstaEliminado)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<List<Cita>> GetByMedicoIdAsync(int medicoId)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.MedicoId == medicoId && !c.EstaEliminado)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<List<Cita>> GetByFechaAsync(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fechaInicio.AddDays(1);

            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.FechaHora >= fechaInicio && c.FechaHora < fechaFin && !c.EstaEliminado)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<List<Cita>> GetByEstadoAsync(string estado)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.Estado.ToString() == estado && !c.EstaEliminado)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora)
        {
            return await _context.Citas
                .AnyAsync(c => c.MedicoId == medicoId &&
                          c.FechaHora == fechaHora &&
                          c.Estado != EstadoCita.Cancelada &&
                          !c.EstaEliminado);
        }

        public async Task<List<Cita>> GetCitasProximasAsync(DateTime desde, DateTime hasta)
        {
            return await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c => c.FechaHora >= desde &&
                           c.FechaHora <= hasta &&
                           c.Estado == EstadoCita.Programada &&
                           !c.EstaEliminado)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<List<Cita>> GetCitasByPacienteAsync(int pacienteId)
        {
            return await GetByPacienteIdAsync(pacienteId);
        }
    }
}