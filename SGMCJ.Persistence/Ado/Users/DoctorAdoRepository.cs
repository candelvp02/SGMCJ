using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Common;
using System.Data;

namespace SGMCJ.Persistence.Ado.Users
{
    public class DoctorAdoRepository : IDoctorAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<DoctorAdoRepository> _logger;

        public DoctorAdoRepository(StoredProcedureExecutor sp, ILogger<DoctorAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<DoctorDto>> ListActiveAsync()
        {
            var doctors = new List<DoctorDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("users.usp_Doctor_ListActive");
                while (await r.ReadAsync())
                {
                    doctors.Add(new DoctorDto
                    {
                        DoctorId = r.GetInt32("DoctorID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        SpecialtyId = r.GetInt16("SpecialtyID"),
                        SpecialtyName = r.GetString("SpecialtyName"),
                        LicenseNumber = r.GetString("LicenseNumber"),
                        PhoneNumber = r.GetString("PhoneNumber"),
                        YearsOfExperience = r.GetInt32("YearsOfExperience"),
                        Education = r.GetString("Education"),
                        Bio = r.IsDBNull("Bio") ? null : r.GetString("Bio"),
                        ConsultationFee = r.IsDBNull("ConsultationFee") ? null : r.GetDecimal("ConsultationFee"),
                        ClinicAddress = r.IsDBNull("ClinicAddress") ? null : r.GetString("ClinicAddress"),
                        AvailabilityMode = r.IsDBNull("AvailabilityMode") ? null : r.GetString("AvailabilityMode"),
                        LicenseExpirationDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("LicenseExpirationDate"))),
                        IsActive = r.GetBoolean("IsActive")
                    });
                }
                return doctors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing active doctors");
                return new List<DoctorDto>();
            }
        }

        public async Task<List<DoctorDto>> SearchByNameAsync(string name)
        {
            var doctors = new List<DoctorDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Doctor_SearchByName",
                    ("@Name", name)
                );
                while (await r.ReadAsync())
                {
                    doctors.Add(new DoctorDto
                    {
                        DoctorId = r.GetInt32("DoctorID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        SpecialtyId = r.GetInt16("SpecialtyID"),
                        SpecialtyName = r.GetString("SpecialtyName"),
                        LicenseNumber = r.GetString("LicenseNumber"),
                        PhoneNumber = r.GetString("PhoneNumber"),
                        YearsOfExperience = r.GetInt32("YearsOfExperience"),
                        Education = r.GetString("Education"),
                        Bio = r.IsDBNull("Bio") ? null : r.GetString("Bio"),
                        ConsultationFee = r.IsDBNull("ConsultationFee") ? null : r.GetDecimal("ConsultationFee"),
                        ClinicAddress = r.IsDBNull("ClinicAddress") ? null : r.GetString("ClinicAddress"),
                        AvailabilityMode = r.IsDBNull("AvailabilityMode") ? null : r.GetString("AvailabilityMode"),
                        LicenseExpirationDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("LicenseExpirationDate"))),
                        IsActive = r.GetBoolean("IsActive")
                    });
                }
                return doctors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors by name: {Name}", name);
                return new List<DoctorDto>();
            }
        }

        public async Task<int> GetTotalAppointmentsAsync(int doctorId)
        {
            try
            {
                var result = await _sp.ExecuteScalarAsync<int?>(
                    "users.usp_Doctor_GetTotalAppointments",
                    ("@DoctorID", doctorId)
                );
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total appointments for doctor {DoctorId}", doctorId);
                return 0;
            }
        }

        public async Task<List<DoctorDto>> ListBySpecialtyAsync(short specialtyId)
        {
            var doctors = new List<DoctorDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_Doctor_ListBySpecialty",
                    ("@SpecialtyID", specialtyId)
                );
                while (await r.ReadAsync())
                {
                    doctors.Add(new DoctorDto
                    {
                        DoctorId = r.GetInt32("DoctorID"),
                        FirstName = r.GetString("FirstName"),
                        LastName = r.GetString("LastName"),
                        DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth"))),
                        IdentificationNumber = r.GetString("IdentificationNumber"),
                        Gender = r.GetString("Gender"),
                        Email = r.GetString("Email"),
                        SpecialtyId = r.GetInt16("SpecialtyID"),
                        SpecialtyName = r.GetString("SpecialtyName"),
                        LicenseNumber = r.GetString("LicenseNumber"),
                        PhoneNumber = r.GetString("PhoneNumber"),
                        YearsOfExperience = r.GetInt32("YearsOfExperience"),
                        Education = r.GetString("Education"),
                        Bio = r.IsDBNull("Bio") ? null : r.GetString("Bio"),
                        ConsultationFee = r.IsDBNull("ConsultationFee") ? null : r.GetDecimal("ConsultationFee"),
                        ClinicAddress = r.IsDBNull("ClinicAddress") ? null : r.GetString("ClinicAddress"),
                        AvailabilityMode = r.IsDBNull("AvailabilityMode") ? null : r.GetString("AvailabilityMode"),
                        LicenseExpirationDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("LicenseExpirationDate"))),
                        IsActive = r.GetBoolean("IsActive")
                    });
                }
                return doctors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing doctors by specialty {SpecialtyId}", specialtyId);
                return new List<DoctorDto>();
            }
        }
    }
}