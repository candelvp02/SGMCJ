using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(HealtSyncContext context) : base(context) { }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.DoctorNavigation)
                .Include(d => d.Specialty)
                .Include(d => d.AvailabilityMode)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(d => d.DoctorNavigation)
                .Include(d => d.Specialty)
                .Include(d => d.AvailabilityMode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet.Where(d => d.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId)
        {
            return await _dbSet.Where(d => d.SpecialtyId == specialtyId).ToListAsync();
        }

        public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber);
        }

        Task IDoctorRepository.DeleteAsync(int doctorId)
        {
            return DeleteAsync(doctorId);
        }

        public Task<bool> ExistsAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetByIdWithAppointmentsAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public Task GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}