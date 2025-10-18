using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepo;
        private readonly ILogger<PatientService> _logger;

        public PatientService(
            IPatientRepository patientRepo,
            ILogger<PatientService> logger)
        {
            _patientRepo = patientRepo;
            _logger = logger;
        }

        public async Task<OperationResult<List<PatientDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patients = await _patientRepo.GetAllWithDetailsAsync();
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pacientes");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener pacientes";
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<PatientDto>();
            _logger.LogInformation("Buscando paciente con ID: {PatientId}", id);
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de paciente inválido: {PatientId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "ID de paciente inválido.";
                    result.Errores.Add("El ID debe ser un número positivo.");
                    return result;
                }

                var patient = await _patientRepo.GetByIdWithDetailsAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Paciente no encontrado con ID: {PatientId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado.";
                    result.Errores.Add($"No existe un paciente con el ID {id}.");
                    return result;
                }

                result.Datos = MapToDto(patient);
                result.Exitoso = true;
                result.Mensaje = "Paciente obtenido correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> CreateAsync(RegisterPatientDto dto)
        {
            var result = new OperationResult<PatientDto>();
            _logger.LogInformation("Iniciando creación de nuevo paciente.");
            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Los datos del paciente son requeridos.";
                    result.Errores.Add("El objeto de entrada no puede ser nulo.");
                    return result;
                }

                if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) || string.IsNullOrWhiteSpace(dto.IdentificationNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos incompletos.";
                    result.Errores.Add("Nombre, Apellido y Cédula son obligatorios.");
                    return result;
                }

            var existingPatient = await _patientRepo.GetByIdentificationNumberAsync(dto.IdentificationNumber);
                if (existingPatient != null)
                {
                    _logger.LogWarning("Intento de crear paciente con cédula duplicada: {IdNumber}", dto.IdentificationNumber);
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe un paciente con ese número de identificación.";
                    result.Errores.Add($"La cédula '{dto.IdentificationNumber}' ya está registrada.");
                    return result;
                }

                _logger.LogInformation("Validaciones completadas exitosamente.");

                var person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    IdentificationNumber = dto.IdentificationNumber,
                    Gender = dto.Gender
                };

                var patient = new Patient
                {
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    EmergencyContactName = dto.EmergencyContactName,
                    EmergencyContactPhone = dto.EmergencyContactPhone,
                    BloodType = dto.BloodType,
                    Allergies = dto.Allergies ?? string.Empty,
                    InsuranceProviderId = dto.InsuranceProviderId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    PatientNavigation = person
                };

                var createdPatient = await _patientRepo.AddAsync(patient);
                result.Datos = MapToDto(createdPatient);
                result.Exitoso = true;
                result.Mensaje = "Paciente creado correctamente.";
                _logger.LogInformation("Paciente creado con ID: {PatientId}", createdPatient.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando paciente");
                result.Exitoso = false;
                result.Mensaje = "Error al crear paciente.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> UpdateAsync(UpdatePatientDto dto)
        {
            var result = new OperationResult<PatientDto>();
            _logger.LogInformation("Iniciando actualización de paciente ID: {PatientId}", dto?.PatientId);
            try
            {
                if (dto == null || dto.PatientId <= 0)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos de entrada inválidos.";
                    result.Errores.Add("Se requiere un DTO válido con un ID de paciente positivo.");
                    return result;
                }

                var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
                if (patient == null)
                {
                    _logger.LogWarning("Intento de actualizar paciente inexistente: {PatientId}", dto.PatientId);
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado.";
                    result.Errores.Add($"No existe un paciente con el ID {dto.PatientId}.");
                    return result;
                }

                _logger.LogInformation("Validaciones completadas, paciente encontrado.");

                patient.PhoneNumber = dto.PhoneNumber;
                patient.Address = dto.Address;
                patient.EmergencyContactName = dto.EmergencyContactName;
                patient.EmergencyContactPhone = dto.EmergencyContactPhone;
                patient.Allergies = dto.Allergies ?? string.Empty;
                patient.InsuranceProviderId = dto.InsuranceProviderId;
                patient.UpdatedAt = DateTime.Now;

                await _patientRepo.UpdateAsync(patient);

                result.Datos = MapToDto(patient);
                result.Exitoso = true;
                result.Mensaje = "Paciente actualizado correctamente.";
                _logger.LogInformation("Paciente {PatientId} actualizado exitosamente.", patient.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando paciente {PatientId}", dto?.PatientId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar paciente.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();
            _logger.LogInformation("Iniciando desactivación de paciente ID: {PatientId}", id);
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID de paciente inválido para eliminar: {PatientId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "ID de paciente inválido.";
                    result.Errores.Add("El ID debe ser un número positivo.");
                    return result;
                }

                var patient = await _patientRepo.GetByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Intento de eliminar paciente inexistente: {PatientId}", id);
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado.";
                    result.Errores.Add($"No existe un paciente con el ID {id}.");
                    return result;
                }

                if (!patient.IsActive)
                {
                    _logger.LogInformation("Paciente {PatientId} ya se encontraba inactivo.", id);
                    result.Exitoso = true;
                    result.Mensaje = "El paciente ya se encontraba inactivo.";
                    return result;
                }

                patient.IsActive = false;
                patient.UpdatedAt = DateTime.Now;
                await _patientRepo.UpdateAsync(patient);

                result.Exitoso = true;
                result.Mensaje = "Paciente desactivado correctamente.";
                _logger.LogInformation("Paciente {PatientId} desactivado exitosamente.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando paciente {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar paciente.";
                result.Errores.Add(ex.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<PatientDto>>> GetActiveAsync()
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patients = await _patientRepo.GetActivePatientsAsync();
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes activos obtenidos";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando pacientes activos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener pacientes activos";
            }
            return result;
        }

        public async Task<OperationResult<List<PatientDto>>> GetByInsuranceProviderAsync(int insuranceProviderId)
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patients = await _patientRepo.GetByInsuranceProviderIdAsync(insuranceProviderId);
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes obtenidos por proveedor de seguro";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando pacientes por proveedor de seguro {InsuranceProviderId}", insuranceProviderId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener pacientes por proveedor de seguro";
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> GetByPhoneNumberAsync(string phoneNumber)
        {
            var result = new OperationResult<PatientDto>();
            try
            {
                var patient = await _patientRepo.GetByPhoneNumberAsync(phoneNumber);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                result.Datos = MapToDto(patient);
                result.Exitoso = true;
                result.Mensaje = "Paciente obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo paciente por número de teléfono {PhoneNumber}", phoneNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente por número de teléfono";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsAsync(int patientId)
        {
            var result = new OperationResult<bool>();
            try
            {
                result.Datos = await _patientRepo.ExistsAsync(patientId);
                result.Exitoso = true;
                result.Mensaje = "Verificación completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando paciente con ID {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar paciente";
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> GetByIdWithDetailsAsync(int patientId)
        {
            return await GetByIdAsync(patientId);
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithAppointmentsAsync(int patientId)
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patient = await _patientRepo.GetByIdWithAppointmentsAsync(patientId);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                result.Datos = new List<PatientDto> { MapToDto(patient) };
                result.Exitoso = true;
                result.Mensaje = "Paciente con citas obtenido";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo paciente con citas {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente con citas";
            }
            return result;
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithMedicalRecordsAsync(int patientId)
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patient = await _patientRepo.GetByIdWithMedicalRecordsAsync(patientId);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                result.Datos = new List<PatientDto> { MapToDto(patient) };
                result.Exitoso = true;
                result.Mensaje = "Paciente con registros médicos obtenido";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo paciente con registros médicos {PatientId}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente con registros médicos";
            }
            return result;
        }

        private static PatientDto MapToDto(Patient p) => new()
        {
            PatientId = p.PatientId,
            FirstName = p.PatientNavigation?.FirstName ?? string.Empty,
            LastName = p.PatientNavigation?.LastName ?? string.Empty,
            Email = string.Empty,
            PhoneNumber = p.PhoneNumber,
            Address = p.Address,
            EmergencyContactName = p.EmergencyContactName,
            EmergencyContactPhone = p.EmergencyContactPhone,
            BloodType = p.BloodType,
            Allergies = p.Allergies,
            InsuranceProviderId = p.InsuranceProviderId,
            InsuranceProviderName = p.InsuranceProvider?.Name ?? string.Empty,
            IsActive = p.IsActive
        };
    }
}