using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IPatientService
    {
        Task<OperationResult<List<PatientDto>>> GetAllAsync();
        Task<OperationResult<PatientDto>> GetByIdAsync(int id);
        Task<OperationResult<PatientDto>> CreateAsync(RegisterPatientDto patientDto);
        Task<OperationResult<PatientDto>> UpdateAsync(UpdatePatientDto patientDto);
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult<List<PatientDto>>> GetActiveAsync();
        Task<OperationResult<List<PatientDto>>> GetByInsuranceProviderAsync(int insuranceProviderId);
        Task<OperationResult<PatientDto>> GetByPhoneNumberAsync(string phoneNumber);
        Task<OperationResult<bool>> ExistsAsync(int patientId);
        Task<OperationResult<PatientDto>> GetByIdWithDetailsAsync(int patientId);
        Task<OperationResult<List<PatientDto>>> GetWithAppointmentsAsync(int patientId);
        Task<OperationResult<List<PatientDto>>> GetWithMedicalRecordsAsync(int patientId);
    }
}