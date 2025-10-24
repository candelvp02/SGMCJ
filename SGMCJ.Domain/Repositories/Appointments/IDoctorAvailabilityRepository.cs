
using SGMCJ.Domain.Entities.Appointments;

namespace SGMCJ.Domain.Repositories.Appointments
{
    public interface IDoctorAvailabilityRepository
    {
        // Consultas básicas
        Task<DoctorAvailability?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<DoctorAvailability>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDateRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate);

        // Validaciones
        Task<bool> IsAvailableAsync(int doctorId, DateOnly date, TimeOnly time);
        Task<bool> HasConflictAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

        // Operaciones CRUD
        Task AddAsync(DoctorAvailability availability);
        Task UpdateAsync(DoctorAvailability availability);
        Task DeleteAsync(int availabilityId);
    }
}