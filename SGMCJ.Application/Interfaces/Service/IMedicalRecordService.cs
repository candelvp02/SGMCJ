using SGMCJ.Application.Dto.Medical;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IMedicalRecordService
    {

        //historial medico rf2.2.4 / 3.1.10
        Task<OperationResult<MedicalRecordDto>> CreateAsync(CreateMedicalRecordDto dto);
        Task<OperationResult<MedicalRecordDto>> UpdateAsync(UpdateMedicalRecordDto dto);

        //consultas
        Task<OperationResult<List<MedicalRecordDto>>> GetByPatientIdAsync(int patientId);
        Task<OperationResult<MedicalRecordDto>> GetByIdAsync(int id);
        Task<OperationResult<List<MedicalRecordDto>>> GetByDoctorIdAsync(int doctorId);
        Task<OperationResult<List<MedicalRecordDto>>> GetByDateRangeAsync(int patientId);
    }
}