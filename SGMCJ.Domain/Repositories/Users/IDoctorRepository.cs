using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId);
        Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber);
        Task<bool> ExistsAsync(int doctorId);
        Task<bool> ExistsByLicenseNumberAsync(string licenseNumber);
        Task<Doctor?> GetByIdWithDetailsAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllWithDetailsAsync();
        Task<Doctor?> GetByIdWithAppointmentsAsync(int doctorId);
        Task GetByEmailAsync(string email);
    }
}