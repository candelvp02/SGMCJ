using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Appointments
{
    public sealed class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
            => await _dbSet.Where(a => a.PatientId == patientId).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(a => a.DoctorId == doctorId).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId)
            => await _dbSet.Where(a => a.StatusId == statusId).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            var now = DateTime.Now;
            return await _dbSet
                .Where(a => a.PatientId == patientId && a.AppointmentDate > now)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int appointmentId)
            => await _dbSet.AnyAsync(a => a.AppointmentId == appointmentId);

        public override async Task<Appointment?> GetByIdAsync(int appointmentId)
            => await _dbSet.FindAsync(appointmentId);

        public async Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId)
            => await _dbSet.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
            => await _dbSet.ToListAsync();

        public async Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(int patientId)
            => await _dbSet.Where(a => a.PatientId == patientId).ToListAsync();

        public async Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate)
            => await _dbSet.AnyAsync(a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate);
    }
}