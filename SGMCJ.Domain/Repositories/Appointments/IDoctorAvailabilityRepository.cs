using SGMCJ.Domain.Entities.Appointments;

namespace SGMCJ.Domain.Repositories.Appointments
{
    public interface IDoctorAvailabilityRepository
    {
        Task<DoctorAvailability?> GetByIdAsync(int availabilityId);
        Task<IEnumerable<DoctorAvailability>> GetAllAsync();
        Task<DoctorAvailability> AddAsync(DoctorAvailability availability);
        Task UpdateAsync(DoctorAvailability availability);
        Task DeleteAsync(int availabilityId);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<DoctorAvailability>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDateRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate);
        Task<bool> IsAvailableAsync(int doctorId, DateOnly date, TimeOnly time);
        Task<bool> HasConflictAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime);
    }
}