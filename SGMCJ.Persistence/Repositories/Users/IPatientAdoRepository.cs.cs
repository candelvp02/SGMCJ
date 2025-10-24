using SGMCJ.Application.Dto.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IPatientAdoRepository
    {
        Task<List<PatientDto>> ListActiveAsync();
        Task<List<PatientDto>> SearchByNameAsync(string name);
        Task<int> GetTotalAppointmentsAsync(int patientId);
        Task<PatientDto?> GetByIdWithDetailsAsync(int patientId);
        Task<PatientDto?> GetByIdentificationNumberAsync(string identificationNumber);
        Task<List<PatientDto>> ListByInsuranceProviderAsync(int insuranceProviderId);
        Task<bool> UpdateInsuranceProviderAsync(int patientId, int insuranceProviderId);
    }
}