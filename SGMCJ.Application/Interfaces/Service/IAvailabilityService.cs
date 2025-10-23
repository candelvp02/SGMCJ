using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IAvailabilityService
    {
        //gestion de disponibilidad rf3.1.7
        Task<OperationResult<AvailabilityDto>> CreateAsync(CreateAvailabilityDto dto);
        Task<OperationResult<AvailabilityDto>> UpdateAsync(UpdateAvailabilityDto dto);
        Task<OperationResult> DeleteAsync(int id);

        //consultas
        Task<OperationResult<List<AvailabilityDto>>> GetByDoctorIdAsync(int doctorId);
        Task<OperationResult<List<AvailabilityDto>>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<OperationResult<bool>> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDate);
        Task<OperationResult<List<DateTime>>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    }
}