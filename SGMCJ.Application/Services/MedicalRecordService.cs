using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Medical;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Medical;

namespace SGMCJ.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(IMedicalRecordRepository repository, ILogger<MedicalRecordService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public async Task<OperationResult<MedicalRecordDto>> UpdateAsync(UpdateMedicalRecordDto dto)
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

                var existing = await _repository.GetByIdAsync(dto.Id);
                if (existing == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Registro médico no encontrado";
                    return result;
                }

                var updated = await _repository.UpdateAsync(existing);

                result.Datos = MapToDto(updated);
                result.Exitoso = true;
                result.Mensaje = "Registro médico actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar registro médico {Id}", dto?.Id);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar registro médico";
            }

            return result;
        }

        public async Task<OperationResult<MedicalRecordDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<MedicalRecordDto>();

            try
            {
                var record = await _repository.GetByIdAsync(id);

                if (record == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Registro médico no encontrado";
                    return result;
                }

                result.Datos = MapToDto(record);
                result.Exitoso = true;
                result.Mensaje = "Registro médico obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registro médico {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener registro médico";
            }

            return result;
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByPatientIdAsync(int patientId)
        {
            var result = new OperationResult<List<MedicalRecordDto>>();

            try
            {
                var records = await _repository.GetByPatientIdAsync(patientId);
                var recordsList = records?.ToList() ?? new List<MedicalRecord>();
                result.Datos = recordsList.Select(MapToDto).ToList();
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

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByDoctorIdAsync(int doctorId)
        {
            var result = new OperationResult<List<MedicalRecordDto>>();

            try
            {
                var records = await _repository.GetByDoctorIdAsync(doctorId);
                var recordsList = records?.ToList() ?? new List<MedicalRecord>();
                result.Datos = recordsList.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Registros médicos del doctor obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros médicos del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener registros médicos";
            }

            return result;
        }

        public async Task<OperationResult<List<MedicalRecordDto>>> GetByDateRangeAsync(int patientId)
        {
            var result = new OperationResult<List<MedicalRecordDto>>();

            try
            {
                var records = await _repository.GetByPatientIdAsync(patientId);
                var recordsList = records?.ToList() ?? new List<MedicalRecord>();

                result.Datos = recordsList
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(MapToDto)
                    .ToList();

                result.Exitoso = true;
                result.Mensaje = "Registros médicos por rango de fecha obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener registros médicos por fecha del paciente {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener registros médicos";
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