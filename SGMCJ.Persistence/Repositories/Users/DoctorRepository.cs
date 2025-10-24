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
        {
            return await _context.Doctors.FindAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> FindAsync(Expression<Func<Doctor, bool>> predicate)
        {
            return await _context.Doctors
                .Where(predicate)
                .ToListAsync();
        }

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
            if (string.IsNullOrEmpty(email))
                return null;

            return await _context.Doctors
                .FirstOrDefaultAsync(d => d.Email != null && d.Email.Equals(email));
        }
    }
}