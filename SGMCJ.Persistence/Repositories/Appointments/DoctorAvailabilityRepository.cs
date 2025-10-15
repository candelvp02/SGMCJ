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

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(d => d.DoctorId == doctorId).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Where(d => d.AvailableDate >= DateOnly.FromDateTime(startDate) && d.AvailableDate <= DateOnly.FromDateTime(endDate)).ToListAsync();

        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDateRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate)
            => await _dbSet.Where(d => d.DoctorId == doctorId && d.AvailableDate >= startDate && d.AvailableDate <= endDate).ToListAsync();

        public async Task<bool> IsAvailableAsync(int doctorId, DateOnly date, TimeOnly time)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                d.StartTime <= time &&
                time < d.EndTime);
        }

        public async Task<bool> HasConflictAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return await _dbSet.AnyAsync(d =>
                d.DoctorId == doctorId &&
                d.AvailableDate == date &&
                !(d.EndTime <= startTime || d.StartTime >= endTime));
        }

        Task IDoctorAvailabilityRepository.DeleteAsync(int availabilityId)
        {
            return DeleteAsync(availabilityId);
        }
    }
}