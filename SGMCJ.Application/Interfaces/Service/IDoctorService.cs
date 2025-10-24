using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IDoctorService
    {
        Task<OperationResult<DoctorDto>> CreateAsync(RegisterDoctorDto doctorDto);
        Task<OperationResult<DoctorDto>> UpdateAsync(UpdateDoctorDto doctorDto);
        Task<OperationResult> DeleteAsync(int id);

        Task<OperationResult<List<DoctorDto>>> GetAllAsync();
        Task<OperationResult<List<DoctorDto>>> GetAllWithDetailsAsync();
        Task<OperationResult<DoctorDto>> GetByIdAsync(int id);
        Task<OperationResult<DoctorDto>> GetByIdWithDetailsAsync(int id);
        Task<OperationResult<List<DoctorDto>>> GetBySpecialtyIdAsync(short specialtyId);
        Task<OperationResult<List<DoctorDto>>> GetActiveDoctorsAsync();
        Task<OperationResult<DoctorDto>> GetByLicenseNumberAsync(string licenseNumber);
        Task<OperationResult<bool>> ExistsByLicenseNumberAsync(string licenseNumber);
        Task<OperationResult<List<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId);

        Task<OperationResult<Doctor>> CreateEntityAsync(Doctor doctor);
        Task<OperationResult<Doctor>> UpdateEntityAsync(Doctor doctor);
    }
}