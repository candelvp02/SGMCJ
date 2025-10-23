using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Medical;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;

namespace SGMCJ.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(IMedicalRecordRepository repository, ILogger<MedicalRecordService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OperationResult<MedicalRecordDto>> CreateAsync(CreateMedicalRecordDto dto)
        {
            var result = new OperationResult<MedicalRecordDto>();
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos requeridos";
                    return result;
                }

                var record = new MedicalRecord
                {
                    PatientId = dto.PatientId,
                    DoctorId = dto.DoctorId,
                    Diagnosis = dto.Diagnosis,
                    Treatment = dto.Treatment,
                    CreatedAt = DateTime.Now
                };

                var created = await _repository.AddAsync(record);
                result.Datos = MapToDto(created);
                result.Exitoso = true;
                result.Mensaje = "Registro médico creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro médico para paciente {PatientId}", dto?.PatientId);
                result.Exitoso = false;
                result.Mensaje = "Error al crear registro médico";
            }
            return result;
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByPatientAsync(int patientId)
        {
            var result = new OperationResult<List<MedicalRecordDto>>();
            try
            {
                var records = await _repository.GetByPatientIdAsync(patientId);
                result.Datos = records.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Historial médico obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial médico del paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener historial médico";
            }
            return result;
        }

        private static MedicalRecordDto MapToDto(MedicalRecord r) => new()
        {
            Id = r.Id,
            PatientId = r.PatientId,
            DoctorId = r.DoctorId,
            Diagnosis = r.Diagnosis,
            Treatment = r.Treatment,
            CreatedAt = r.CreatedAt
        };
    }
}