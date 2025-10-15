using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IDoctorService
    {
        Task<OperationResult<List<DoctorDto>>> GetAllAsync();
        Task<OperationResult<DoctorDto>> GetByIdAsync(int id);
        Task<OperationResult<DoctorDto>> CreateAsync(DoctorDto doctorDto);
        Task<OperationResult<DoctorDto>> UpdateAsync(UpdateDoctorDto doctorDto);

        // Entidades para EF interno
        Task<OperationResult<Doctor>> CreateEntityAsync(Doctor doctor);
        Task<OperationResult<Doctor>> UpdateEntityAsync(Doctor doctor);

        // Métodos específicos
        Task<OperationResult<List<DoctorDto>>> GetActiveDoctorsAsync();
        Task<OperationResult<List<DoctorDto>>> GetBySpecialtyIdAsync(short specialtyId);
        Task<OperationResult<DoctorDto>> GetByLicenseNumberAsync(string licenseNumber);
        Task<OperationResult<bool>> ExistsByLicenseNumberAsync(string licenseNumber);
        Task<OperationResult<DoctorDto>> GetByIdWithDetailsAsync(int id);
        Task<OperationResult<List<DoctorDto>>> GetAllWithDetailsAsync();
        Task<OperationResult<DoctorDto>> GetByIdWithAppointmentsAsync(int id);
    }
}