using Microsoft.EntityFrameworkCore;
using SGMCJ.Application.Interfaces;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Persistence.Context;
using System;
using System.Linq.Expressions;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HealtSyncContext _context;
        private readonly DbSet<Doctor> _dbSet;

        public DoctorRepository(HealtSyncContext context)
        {
            _context = context;
            _dbSet = _context.Set<Doctor>();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _dbSet.Where(d => d.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> FindAsync(Expression<Func<Doctor, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            _dbSet.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor> UpdateAsync(Doctor doctor)
        {
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _dbSet.FindAsync(id);
            if (doctor != null)
            {
                _dbSet.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task<Doctor?> GetByEmailAsync(string email)
        //{
        //    return await _dbSet
        //        .FirstOrDefaultAsync(d => d.Email != null && string.Equals((string?)d.Email, email, StringComparison.OrdinalIgnoreCase));
        //}
        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(d => d.DoctorNavigation)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(d => d.DoctorNavigation != null
                                          && d.DoctorNavigation.User != null
                                          && d.DoctorNavigation.User.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _dbSet.Where(d => d.IsActive).ToListAsync();
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialtyIdAsync(short specialtyId)
        {
            return await _dbSet
                .Where(d => d.SpecialtyId == specialtyId && d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet.Where(d => d.IsActive).ToListAsync();
        }

        public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }
    }
}