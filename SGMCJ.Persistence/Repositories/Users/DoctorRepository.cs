using Microsoft.EntityFrameworkCore;
using SGMCJ.Application.Interfaces;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Persistence.Context;
using System.Linq.Expressions;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HealtSyncContext _context;

        public DoctorRepository(HealtSyncContext context)
        {
            _context = context;
        }

        public async Task<Doctor?> GetByIdAsync(int id)
            => await _context.Doctors.FindAsync(id);

        public async Task<IEnumerable<Doctor>> GetAllAsync()
            => await _context.Doctors.Where(d => d.IsActive).ToListAsync();

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.AvailabilityMode)
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.AvailabilityMode)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId)
        {
            return await _context.Doctors
                .Where(d => d.SpecialtyId == specialtyId && d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
            => await _context.Doctors.Where(d => d.IsActive).ToListAsync();

        public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.Doctors
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
            => await _context.Doctors.AnyAsync(d => d.LicenseNumber == licenseNumber);

        public async Task<IEnumerable<Doctor>> FindAsync(Expression<Func<Doctor, bool>> predicate)
            => await _context.Doctors.Where(predicate).ToListAsync();

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor> UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await _context.Doctors
                .Include(d => d.DoctorNavigation)
                .FirstOrDefaultAsync(d => d.Email != null && d.Email == email);
        }
    }
}