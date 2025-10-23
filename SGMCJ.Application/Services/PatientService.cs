using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using System.Text.RegularExpressions;

namespace SGMCJ.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly ILogger _logger;

        public PatientService(
            IPatientRepository repository,
            ILogger<PatientService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // crear paciente
        public async Task<OperationResult<PatientDto>> CreateAsync(RegisterPatientDto dto)
        {
            var result = new OperationResult<PatientDto>();

            try
            {
                // validar datos requeridos
                if (!ValidateRequiredFields(dto, result))
                    return result;

                // validar formatos
                if (!ValidateFormats(dto, result))
                    return result;

                // verificar unicidad de cedula
                var existsByCedula = await _repository.GetByIdentificationNumberAsync(dto.IdentificationNumber);
                if (existsByCedula != null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe un paciente con ese numero de id";
                    return result;
                }

                //verificar unicidad de telefono
                var existsByPhone = await _repository.GetByPhoneNumberAsync(dto.PhoneNumber);
                if(existsByPhone != null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe un paciente con ese numero de tel.";
                    return result;
                }

                // crear Person
                var person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    IdentificationNumber = dto.IdentificationNumber,
                    Gender = dto.Gender
                };

                // crear Patient
                var patient = new Patient
                {
                    Gender = dto.Gender,
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

                var createdPatient = await _repository.AddAsync(patient);
                result.Datos = MapToDto(createdPatient);
                result.Exitoso = true;
                result.Mensaje = "Paciente creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear paciente");
                result.Exitoso = false;
                result.Mensaje = "error al crear paciente";
            }

            return result;
        }

        // actualizar paciente
        public async Task<OperationResult<PatientDto>> UpdateAsync(UpdatePatientDto dto)
        {
            var result = new OperationResult<PatientDto>();

            try
            {
                if (dto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Los datos del paciente son requeridos";
                    return result;
                }

                // verificar que existe
                var patient = await _repository.GetByIdAsync(dto.PatientId);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                //validar telefono
                if (!IsValidPhoneNumber(dto.PhoneNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Formato de telefono invalido, debe ser XXX-XXX-XXXX";
                    return result;
                }

                // validar direccion
                if (string.IsNullOrWhiteSpace(dto.Address) || dto.Address.Length == 10)
                {
                    result.Exitoso = false;
                    result.Mensaje = "la direccion debe tener al menos 10 caracteres";
                    return result;
                }

                // validar telefono de emergencia
                if (!IsValidPhoneNumber(dto.EmergencyContactPhone))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Formato de telefono de emergencia invalido";
                    return result;
                }

                // actualizar
                patient.PhoneNumber = dto.PhoneNumber;
                patient.Address = dto.Address;
                patient.EmergencyContactName = dto.EmergencyContactName;
                patient.EmergencyContactPhone = dto.EmergencyContactPhone;
                patient.Allergies = dto.Allergies ?? string.Empty;
                patient.InsuranceProviderId = dto.InsuranceProviderId;
                patient.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(patient);

                result.Datos = MapToDto(patient);
                result.Exitoso = true;
                result.Mensaje = "Paciente actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente {Id}", dto?.PatientId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar paciente";
            }

            return result;
        }

        // eliminar/desactivar paciente
        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();

            try
            {
                var patient = await _repository.GetByIdAsync(id);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                // desactivar
                patient.IsActive = false;
                patient.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(patient);

                result.Exitoso = true;
                result.Mensaje = "Paciente desactivado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paciente {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar paciente";
            }

            return result;
        }

        // consultas
        public async Task<OperationResult<List<PatientDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patients = await _repository.GetAllWithDetailsAsync();
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener pacientes";
            }
            return result;
        }
        public async Task<OperationResult<List<PatientDto>>> GetActiveAsync()
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patients = await _repository.GetActivePatientsAsync();
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes activos obtenidos";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes activos");
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
                var patients = await _repository.GetByInsuranceProviderIdAsync(insuranceProviderId);
                result.Datos = patients.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Pacientes obtenidos por proveedor de seguro";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes por seguro {Id}", insuranceProviderId);
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
                var patient = await _repository.GetByPhoneNumberAsync(phoneNumber);
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
                _logger.LogError(ex, "Error al obtener paciente por telefono {Phone}", phoneNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente por numero de telefono";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsAsync(int patientId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var exists = await _repository.ExistsAsync(patientId);
                result.Datos = exists;
                result.Exitoso = false;
                result.Mensaje = "Verificacion completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error al verificar paciente {Id}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar paciente";
            }
            return result;
        }

        public async Task<OperationResult<PatientDto>> GetByIdWithDetailsAsync(int patientId)
        {
            var result = new OperationResult<PatientDto>();
            try
            {
                var patient = await _repository.GetByIdWithDetailsAsync(patientId);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                result.Datos = MapToDto(patient);
                result.Exitoso = true;
                result.Mensaje = "Paciente con detalles obtenido";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente con detalles {Id}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente con detalles";
            }
            return result;
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithAppointmentsAsync(int patientId)
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patient = await _repository.GetByIdWithAppointmentsAsync(patientId);
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
                _logger.LogError(ex, "Erroor al obtener paciente con citas {Id}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Erroor al obtener paciente con citas";
            }
            return result;
        }

        public async Task<OperationResult<List<PatientDto>>> GetWithMedicalRecordsAsync (int patientId)
        {
            var result = new OperationResult<List<PatientDto>>();
            try
            {
                var patient = await _repository.GetByIdWithMedicalRecordsAsync(patientId);
                if (patient == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Paciente no encontrado";
                    return result;
                }

                result.Datos = new List<PatientDto> { MapToDto(patient) };
                result.Exitoso = true;
                result.Mensaje = "paciente con registros medicos obtenido";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente con registros {Id}", patientId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente con registros medicos";
            }
            return result;
        }
        public async Task<OperationResult<PatientDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<PatientDto>();
            try
            {
                var patient = await _repository.GetByIdAsync(id);
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
                _logger.LogError(ex, "Error al obtener paciente {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener paciente";
            }
            return result;
        }

        //metodos privados de validacion

        private bool ValidateRequiredFields(RegisterPatientDto dto, OperationResult result)
        {
            if (dto == null)
            {
                result.Exitoso = false;
                result.Mensaje = "los datos del paciente son requeridos";
                return false;
            }

            // nombre
            if (string.IsNullOrWhiteSpace(dto.FirstName) || dto.FirstName.Length < 2)
            {
                result.Exitoso = false;
                result.Mensaje = "el nombre debe tener al menos 2 caracteres";
                return false;
            }

            // apellido
            if (string.IsNullOrWhiteSpace(dto.LastName) || dto.LastName.Length < 2)
            {
                result.Exitoso = false;
                result.Mensaje = "el apellido debe tener al menos 2 caracteres";
                return false;
            }

            // cedula 
            if (string.IsNullOrWhiteSpace(dto.IdentificationNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "el numero de identificacion es requerido";
                return false;
            }

            // genero
            if (string.IsNullOrWhiteSpace(dto.Gender) || (dto.Gender != "M" && dto.Gender != "F"))
            {
                result.Exitoso = false;
                result.Mensaje = "el genero debe ser M o F";
                return false;
            }

            //email
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                result.Exitoso = false;
                result.Mensaje = "el email es requerido";
                return false;
            }

            // telefono
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "El telefono es requerido";
                return false;
            }

            // direccion
            if (string.IsNullOrWhiteSpace(dto.Address) || dto.Address.Length < 10)
            {
                result.Exitoso = false;
                result.Mensaje = "la direccion debe tener al menos 10 caracteres";
                return false;
            }

            // contacto de emergencia
            if (string.IsNullOrWhiteSpace(dto.EmergencyContactName))
            {
                result.Exitoso = false;
                result.Mensaje = "El nombre del contacto de emergencia es requerido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.EmergencyContactPhone))
            {
                result.Exitoso = false;
                result.Mensaje = "El telefono del contacto de emergencia es requerido";
                return false;
            }

            // tipo de sangre
            if (string.IsNullOrWhiteSpace(dto.BloodType))
            {
                result.Exitoso = false;
                result.Mensaje = "El tipo de sangre es requerido";
                return false;
            }

            // proveedor de seguro
            if (dto.InsuranceProviderId < 0)
            {
                result.Exitoso = false;
                result.Mensaje = "El proveedor de seguro es requerido";
                return false;
            }

            return true;
        }

        private bool ValidateFormats(RegisterPatientDto dto, OperationResult result)
        {
            // validar formato de cedula XXX-XXXXXXX-X
            if (!Regex.IsMatch(dto.IdentificationNumber, @"^\d{3}-\d{7}-\d{1}$"))
            {
                result.Exitoso = false;
                result.Mensaje = "formato de cedula invalido debe ser XXX-XXXXXXX-X";
                return false;
            }

            //validar formato de email
            if (Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                result.Exitoso = false;
                result.Mensaje = "formato de email invalido";
                return false;
            }

            // validar formato de telefono
            if (!IsValidPhoneNumber(dto.PhoneNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "Formato de telefono invalido deber ser XXX-XXX-XXXX";
                return false;
            }

            //validar formato de telefono de emergencia
            if (!IsValidPhoneNumber(dto.EmergencyContactPhone))
            {
                result.Exitoso = false;
                result.Mensaje = "Formato de telefono de emergencia invalido debe ser XXX-XXX-XXXX";
                return false;
            }

            //validar tipo de sangre
            var validBloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            if (!validBloodTypes.Contains(dto.BloodType))
            {
                result.Exitoso = false;
                result.Mensaje = "Tipo de sangre invalido debe ser A+, A-, B+, B-, AB+, AB-, O+, O-";
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{3}-\d{3}-\d{4}$");
        }

        //mappers

        private static PatientDto MapToDto(Patient p) => new()
        {
            PatientId = p.PatientId,
            FirstName = p.PatientNavigation?.FirstName ?? string.Empty,
            LastName = p.PatientNavigation?.LastName ?? string.Empty,
            DateOfBirth = p.PatientNavigation?.DateOfBirth,
            IdentificationNumber = p.PatientNavigation?.IdentificationNumber ?? string.Empty,
            Gender = p.Gender,
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