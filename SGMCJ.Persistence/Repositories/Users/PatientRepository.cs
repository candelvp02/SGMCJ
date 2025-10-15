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
                .Include(p => p.Appointments)
                .ThenInclude(a => a.StatusId)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Patient?> GetByIdWithMedicalRecordsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.MedicalRecords)
                .ThenInclude(mr => mr.Doctor)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        Task IPatientRepository.DeleteAsync(int patientId)
        {
            return DeleteAsync(patientId);
        }

        public Task<bool> ExistsAsync(int patientId)
        {
            throw new NotImplementedException();
        }
    }
}