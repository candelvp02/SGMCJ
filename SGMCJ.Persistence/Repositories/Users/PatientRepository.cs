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

        public override async Task<Patient?> GetByIdAsync(int patientId) 
            => await _dbSet.FindAsync(patientId);

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
            => await _dbSet.Where(p => p.IsActive).ToListAsync();

        public async Task<IEnumerable<Patient>> GetByInsuranceProviderIdAsync(int insuranceProviderId)
            => await _dbSet.Where(p => p.InsuranceProviderId == insuranceProviderId).ToListAsync();

        public async Task<Patient?> GetByPhoneNumberAsync(string phoneNumber)
            => await _dbSet.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

        public async Task<Patient?> GetByIdWithAppointmentsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Appointments)
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
                .FirstOrDefaultAsync(p => p.PatientNavigation != null &&
                                         p.PatientNavigation.IdentificationNumber == identificationNumber);
        }

        public async Task<bool> ExistsAsync(int patientId)
            => await _dbSet.AnyAsync(p => p.PatientId == patientId);

        public override async Task UpdateAsync(Patient patient) 
        {
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int patientId) 
        {
            var patient = await GetByIdAsync(patientId);
            if (patient != null)
            {
                _dbSet.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public Task GetByIdWithDetailsAsync(object id)
        {
            if (id is int patientId)
                return GetByIdWithDetailsAsync(patientId);

            return Task.CompletedTask;
        }
    }
}