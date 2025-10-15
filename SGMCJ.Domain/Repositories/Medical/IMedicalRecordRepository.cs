using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface IMedicalRecordRepository
    {
        Task<MedicalRecord?> GetByIdAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord);
        Task UpdateAsync(MedicalRecord medicalRecord);
        Task DeleteAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<MedicalRecord?> GetLatestByPatientIdAsync(int patientId);
        Task<MedicalRecord?> GetByIdWithDetailsAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdWithDetailsAsync(int patientId);
    }
}