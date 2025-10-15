using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public sealed class MedicalRecordRepository : BaseRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(HealtSyncContext context) : base(context) { }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
            => await _dbSet.Where(m => m.PatientId == patientId).ToListAsync();

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(m => m.DoctorId == doctorId).ToListAsync();

        public async Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Where(m => m.DateOfVisit >= startDate && m.DateOfVisit <= endDate).ToListAsync();

        public async Task<MedicalRecord?> GetLatestByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.DateOfVisit)
                .FirstOrDefaultAsync();
        }

        public async Task<MedicalRecord?> GetByIdWithDetailsAsync(int recordId)
        {
            return await _dbSet
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .FirstOrDefaultAsync(m => m.RecordId == recordId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdWithDetailsAsync(int patientId)
        {
            return await _dbSet
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }

        Task IMedicalRecordRepository.DeleteAsync(int recordId)
        {
            return DeleteAsync(recordId);
        }
    }
}
