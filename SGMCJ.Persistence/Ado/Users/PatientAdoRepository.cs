using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Common;
using System.Data;

namespace SGMCJ.Persistence.Ado.Users
{
    public class PatientAdoRepository : IPatientAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<PatientAdoRepository> _logger;

        public PatientAdoRepository(StoredProcedureExecutor sp, ILogger<PatientAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<PatientDto>> ListActiveAsync()
        {
            var patients = new List<PatientDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("users.usp_Patient_ListActive");
                while (await r.ReadAsync())
                {
                    patients.Add(new PatientDto
                    {
                        PatientId = r.GetInt32("PatientID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        PhoneNumber = r.GetString("PhoneNumber"),
                        Address = r.GetString("Address"),
                        EmergencyContactName = r.GetString("EmergencyContactName"),
                        EmergencyContactPhone = r.GetString("EmergencyContactPhone"),
                        BloodType = r.GetString("BloodType"),
                        Allergies = r.IsDBNull("Allergies") ? null : r.GetString("Allergies"),
                        InsuranceProviderId = r.GetInt32("InsuranceProviderID"),
                        InsuranceProviderName = r.GetString("InsuranceProviderName"),
                        IsActive = r.GetBoolean("IsActive")
                    });
                }
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing active patients");
                return new List<PatientDto>();
            }
        }

        public async Task<List<PatientDto>> SearchByNameAsync(string name)
        {
            var patients = new List<PatientDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Patient_SearchByName",
                    ("@Name", name)
                );
                while (await r.ReadAsync())
                {
                    patients.Add(new PatientDto
                    {
                        PatientId = r.GetInt32("PatientID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        PhoneNumber = r.GetString("PhoneNumber"),
                        Address = r.GetString("Address"),
                        EmergencyContactName = r.GetString("EmergencyContactName"),
                        EmergencyContactPhone = r.GetString("EmergencyContactPhone"),
                        BloodType = r.GetString("BloodType"),
                        Allergies = r.IsDBNull("Allergies") ? null : r.GetString("Allergies"),
                        InsuranceProviderId = r.GetInt32("InsuranceProviderID"),
                        InsuranceProviderName = r.GetString("InsuranceProviderName"),
                        IsActive = r.GetBoolean("IsActive")
                    });
                }
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching patients by name: {Name}", name);
                return new List<PatientDto>();
            }
        }

        public async Task<int> GetTotalAppointmentsAsync(int patientId)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "users.usp_Patient_GetTotalAppointments",
                    ("@PatientID", patientId)
                );
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total appointments for patient {PatientId}", patientId);
                return 0;
            }
        }
    }
}