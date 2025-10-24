using SGMCJ.Domain.Entities.Users;
using System.Linq.Expressions;

namespace SGMCJ.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(int id);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<IEnumerable<Doctor>> FindAsync(Expression<Func<Doctor, bool>> predicate);
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
        Task<Doctor?> GetByEmailAsync(string email);
        Task<bool> ExistsByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<Doctor>> GetAllWithDetailsAsync();
        Task<Doctor> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId);
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<Doctor> GetByLicenseNumberAsync(string licenseNumber);
    }
}