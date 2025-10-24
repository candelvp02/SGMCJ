using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using System.Text.RegularExpressions;

namespace SGMCJ.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository repository,
            ILogger<DoctorService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // crear doctor
        public async Task<OperationResult<DoctorDto>> CreateAsync(DoctorDto doctorDto)
        {
            var result = new OperationResult<DoctorDto>();

            try
            {
                // validar datos requeridos
                if (!ValidateRequiredFields(doctorDto, result))
                    return result;

                // validar formatos
                if (!ValidateFormats(doctorDto, result))
                    return result;

                // validar rangos numericos
                if (!ValidateRanges(doctorDto, result))
                    return result;

                //validar unicidad de licencia
                var existeLicencia = await _repository.ExistsByLicenseNumberAsync(doctorDto.LicenseNumber);
                if (existeLicencia)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Número de licencia ya existe";
                    return result;
                }

                // crear doctor
                var doctor = new Doctor
                {
                    SpecialtyId = doctorDto.SpecialtyId,
                    LicenseNumber = doctorDto.LicenseNumber,
                    PhoneNumber = doctorDto.PhoneNumber,
                    YearsOfExperience = doctorDto.YearsOfExperience,
                    Education = doctorDto.Education,
                    Bio = doctorDto.Bio ?? string.Empty,
                    ConsultationFee = doctorDto.ConsultationFee,
                    ClinicAddress = doctorDto.ClinicAddress ?? string.Empty,
                    LicenseExpirationDate = doctorDto.LicenseExpirationDate,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var createdDoctor = await _repository.AddAsync(doctor);
                result.Datos = MapToDto(createdDoctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el doctor");
                result.Exitoso = false;
                result.Mensaje = "Error al crear el doctor";
            }

            return result;
        }

        // actualizar doctor
        public async Task<OperationResult<DoctorDto>> UpdateAsync(UpdateDoctorDto doctorDto)
        {
            var result = new OperationResult<DoctorDto>();

            try
            {
                if (doctorDto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Datos del doctor son requeridos";
                    return result;
                }

                // verificar q existe
                var doctor = await _repository.GetByIdAsync(doctorDto.DoctorId);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                // validar telefono
                if (!IsValidPhoneNumber(doctorDto.PhoneNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Número de teléfono inválido debe ser XXX-XXX-XXXX";
                    return result;
                }

                // validate years of experience
                if (doctorDto.YearsOfExperience < 0 || doctorDto.YearsOfExperience > 60)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Años de experiencia inválidos deben estar entre 0 y 60";
                    return result;
                }

                // validar licencia vigente
                if (doctorDto.LicenseExpirationDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La licencia debe estar vigente";
                    return result;
                }

                //actualizar
                doctor.PhoneNumber = doctorDto.PhoneNumber;
                doctor.YearsOfExperience = doctorDto.YearsOfExperience;
                doctor.Education = doctorDto.Education;
                doctor.Bio = doctorDto.Bio ?? string.Empty;
                doctor.ConsultationFee = doctorDto.ConsultationFee;
                doctor.ClinicAddress = doctorDto.ClinicAddress ?? string.Empty;
                doctor.AvailabilityModeId = doctorDto.AvailabilityMode;
                doctor.LicenseExpirationDate = doctorDto.LicenseExpirationDate;
                doctor.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(doctor);
                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el doctor {DoctorId}", doctorDto.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar el doctor";
            }

            return result;
        }

        // consultas
        public async Task<OperationResult<List<DoctorDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctor = await _repository.GetAllAsync();
                result.Datos = doctor.Select(d => MapToDto(d)).ToList();
                result.Exitoso = true;
                result.Mensaje = "Doctores obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctores";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetAllWithDetailsAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctor = await _repository.GetAllWithDetailsAsync();
                result.Datos = doctor.Select(MapToDtoWithDetails).ToList();
                result.Exitoso = true;
                result.Mensaje = "Doctores con detalles obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores con detalles");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctores con detalles";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repository.GetByIdAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctor";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdWithDetailsAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repository.GetByIdWithDetailsAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                result.Datos = MapToDtoWithDetails(doctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor con detalles obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor con detalles {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctor con detalles";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdWithAppointmentsAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repository.GetByIdWithDetailsAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                result.Datos = MapToDtoWithDetails(doctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor con citas obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor con citas {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctor con citas";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetBySpecialtyIdAsync(short specialtyId)
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctors = await _repository.GetBySpecialtyIdAsync(specialtyId);
                result.Datos = doctors.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Doctores obtenidos correctamente por especialidad";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores por especialidad {Id}", specialtyId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctores por especialidad";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetActiveDoctorsAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctors = await _repository.GetActiveDoctorsAsync();
                result.Datos = doctors.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Doctores activos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores activos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctores activos";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByLicenseNumberAsync(string licenseNumber)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repository.GetByLicenseNumberAsync(licenseNumber);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Doctor obtenido correctamente por número de licencia";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor por número de licencia {License}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctor por número de licencia";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            var result = new OperationResult<bool>();
            try
            {
                var exists = await _repository.ExistsByLicenseNumberAsync(licenseNumber);
                result.Datos = exists;
                result.Exitoso = true;
                result.Mensaje = "Verificación completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar licencia {License}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar doctor";
            }
            return result;
        }

        // metodos para EF ENTITIES
        public async Task<OperationResult<Doctor>> CreateEntityAsync(Doctor doctor)
        {
            var result = new OperationResult<Doctor>();
            try
            {
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Entidad doctor es requerida";
                    return result;
                }

                var created = await _repository.AddAsync(doctor);
                result.Datos = created;
                result.Exitoso = true;
                result.Mensaje = "Entidad doctor creada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la entidad doctor");
                result.Exitoso = false;
                result.Mensaje = "Error al crear la entidad doctor";
            }
            return result;
        }

        public async Task<OperationResult<Doctor>> UpdateEntityAsync(Doctor doctor)
        {
            var result = new OperationResult<Doctor>();
            try
            {
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Entidad doctor es requerida";
                    return result;
                }

                await _repository.UpdateAsync(doctor);
                result.Datos = doctor;
                result.Exitoso = true;
                result.Mensaje = "Entidad doctor actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la entidad doctor {Id}", doctor?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar la entidad doctor";
            }
            return result;
        }

        // metodos privados de validacion
        private bool ValidateRequiredFields(DoctorDto dto, OperationResult result)
        {
            if (dto == null)
            {
                result.Exitoso = false;
                result.Mensaje = "Datos del doctor son requeridos";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "Número de licencia es requerido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "Número de teléfono es requerido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Education))
            {
                result.Exitoso = false;
                result.Mensaje = "Educación es requerida";
                return false;
            }

            if (dto.SpecialtyId <= 0)
            {
                result.Exitoso = false;
                result.Mensaje = "Especialidad es requerida";
                return false;
            }

            return true;
        }

        private bool ValidateFormats(DoctorDto dto, OperationResult result)
        {

            // validar formato telefono
            if (!IsValidPhoneNumber(dto.PhoneNumber))
            {
                result.Exitoso = false;
                result.Mensaje = "Número de teléfono inválido debe ser XXX-XXX-XXXX";
                return false;
            }

            return true;
        }

        private bool ValidateRanges(DoctorDto dto, OperationResult result)
        {
            // validar años de experiencia
            if (dto.YearsOfExperience < 0 || dto.YearsOfExperience > 60)
            {
                result.Exitoso = false;
                result.Mensaje = "Años de experiencia inválidos deben estar entre 0 y 60";
                return false;
            }
           
            // licencia vigente
            if (dto.LicenseExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                result.Exitoso = false;
                result.Mensaje = "La licencia debe estar vigente";
                return false;
            }

            //tarifa de consulta
            if(dto.ConsultationFee.HasValue && dto.ConsultationFee.Value < 0)
            {
                result.Exitoso = false;
                result.Mensaje = "La tarifa de consulta no puede ser negativa";
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{3}-\d{3}-\d{4}$"); //formato para recibir el numero en el formato correcto
        }

        // mappers
        private static DoctorDto MapToDto(Doctor d) => new()
        {
            DoctorId = d.DoctorId,
            SpecialtyId = d.SpecialtyId,
            LicenseNumber = d.LicenseNumber,
            PhoneNumber = d.PhoneNumber,
            YearsOfExperience = d.YearsOfExperience,
            Education = d.Education,
            Bio = d.Bio,
            ConsultationFee = d.ConsultationFee,
            ClinicAddress = d.ClinicAddress,
            LicenseExpirationDate = d.LicenseExpirationDate,
            IsActive = d.IsActive
        };

        private static DoctorDto MapToDtoWithDetails(Doctor d) => new()
        {
            DoctorId = d.DoctorId,
            SpecialtyId = d.SpecialtyId,
            SpecialtyName = d.Specialty?.SpecialtyName ?? string.Empty,
            LicenseNumber = d.LicenseNumber,
            PhoneNumber = d.PhoneNumber,
            YearsOfExperience = d.YearsOfExperience,
            Education = d.Education,
            Bio = d.Bio,
            ConsultationFee = d.ConsultationFee,
            ClinicAddress = d.ClinicAddress,
            AvailabilityMode = d.AvailabilityModeId?.ToString() ?? string.Empty,
            LicenseExpirationDate = d.LicenseExpirationDate,
            IsActive = d.IsActive
        };
    }
}