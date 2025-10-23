using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IDoctorService
    {
        //crud rf3.1.12 gestion de drs
        Task<OperationResult<DoctorDto>> CreateAsync(DoctorDto doctorDto);
        Task<OperationResult<DoctorDto>> UpdateAsync(UpdateDoctorDto doctorDto);

        //consultas
        Task<OperationResult<List<DoctorDto>>> GetAllAsync();
        Task<OperationResult<List<DoctorDto>>> GetAllWithDetailsAsync();
        Task<OperationResult<DoctorDto>> GetByIdAsync(int id);
        Task<OperationResult<DoctorDto>> GetByIdWithDetailsAsync(int id);
        //Task<OperationResult<DoctorDto>> GetByIdAppointmentsAsync(int id);
        Task<OperationResult<List<DoctorDto>>> GetBySpecialtyIdAsync(short specialtyId);
        Task<OperationResult<List<DoctorDto>>> GetActiveDoctorsAsync();
        Task<OperationResult<DoctorDto>> GetByLicenseNumberAsync(string licenseNumber);
        Task<OperationResult<bool>> ExistsByLicenseNumberAsync(string licenseNumber);

        //metodos para ef entities
        Task<OperationResult<Doctor>> CreateEntityAsync(Doctor doctor);
        Task<OperationResult<Doctor>> UpdateEntityAsync(Doctor doctor);
    }
}