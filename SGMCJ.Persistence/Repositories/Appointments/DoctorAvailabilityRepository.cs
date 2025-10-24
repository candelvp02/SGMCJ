using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Appointments
{
    public sealed class DoctorAvailabilityRepository : BaseRepository<DoctorAvailability>, IDoctorAvailabilityRepository
    {
        public DoctorAvailabilityRepository(HealtSyncContext context) : base(context) { }

        public override async Task<DoctorAvailability?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(d => d.DoctorId == doctorId).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
            => await _dbSet.Where(d => d.AvailableDate >= startDate && d.AvailableDate <= endDate).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDateRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate)
            => await _dbSet.Where(d => d.DoctorId == doctorId && d.AvailableDate >= startDate && d.AvailableDate <= endDate).ToListAsync();

        public async Task<bool> IsAvailableAsync(int doctorId, DateOnly date, TimeOnly time)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                d.StartTime <= time &&
                time < d.EndTime &&
                d.IsActive);
        }

        public async Task<bool> HasConflictAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                d.IsActive &&
                !(d.EndTime <= startTime || d.StartTime >= endTime));
        }

        public new async Task AddAsync(DoctorAvailability availability)
        {
            await _dbSet.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        public new async Task UpdateAsync(DoctorAvailability availability)
        {
            _dbSet.Update(availability);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int availabilityId)
        {
            var entity = await _dbSet.FindAsync(availabilityId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}