using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(HealtSyncContext context) : base(context) { }

        public async Task<Patient?> GetByIdWithDetailsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.PatientNavigation)
                .Include(p => p.InsuranceProvider)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<IEnumerable<Patient>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.PatientNavigation)
                .Include(p => p.InsuranceProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetActivePatientsAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetByInsuranceProviderIdAsync(int insuranceProviderId)
        {
            return await _dbSet.Where(p => p.InsuranceProviderId == insuranceProviderId).ToListAsync();
        }

        public async Task<Patient?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
        }

        public async Task<Patient?> GetByIdWithAppointmentsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.DoctorId)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Patient?> GetByIdWithMedicalRecordsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.MedicalRecords)
                    .ThenInclude(mr => mr.Doctor)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Patient?> GetByIdentificationNumberAsync(string identificationNumber)
        {
            return await _dbSet
                .Include(p => p.PatientNavigation)
                .FirstOrDefaultAsync(p => p.PatientNavigation.IdentificationNumber == identificationNumber);
        }

        public async Task<bool> ExistsAsync(int patientId)
        {
            return await _dbSet.AnyAsync(p => p.PatientId == patientId);
        }
        public Task UpdateAsync(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        public async Task DeleteAsync(int patientId)
        {
            var patientToDelete = await _dbSet.FindAsync(patientId);
            if (patientToDelete != null)
            {
                _dbSet.Remove(patientToDelete);
            }
        }
    }
}