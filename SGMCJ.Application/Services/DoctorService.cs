using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Domain.Repositories.Users;
using System.Text.RegularExpressions;

namespace SGMCJ.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository repository,
            IAppointmentRepository appointmentRepository,
            ILogger<DoctorService> logger)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<OperationResult<DoctorDto>> CreateAsync(RegisterDoctorDto doctorDto)
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

                if (string.IsNullOrWhiteSpace(doctorDto.LicenseNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Numero de licencia es requerido";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(doctorDto.PhoneNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Numero de telefono es requerido";
                    return result;
                }

                if (!IsValidPhoneNumber(doctorDto.PhoneNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Numero de telefono invalido debe ser XXX-XXX-XXXX";
                    return result;
                }

                if (doctorDto.YearsOfExperience < 0 || doctorDto.YearsOfExperience > 60)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Anos de experiencia invalidos deben estar entre 0 y 60";
                    return result;
                }

                if (doctorDto.LicenseExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La licencia debe estar vigente";
                    return result;
                }

                var existeLicencia = await _repository.ExistsByLicenseNumberAsync(doctorDto.LicenseNumber);
                if (existeLicencia)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Numero de licencia ya existe";
                    return result;
                }

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

                var doctor = await _repository.GetByIdAsync(doctorDto.DoctorId);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                if (!IsValidPhoneNumber(doctorDto.PhoneNumber))
                {
                    result.Exitoso = false;
                    result.Mensaje = "Numero de telefono invalido debe ser XXX-XXX-XXXX";
                    return result;
                }

                if (doctorDto.YearsOfExperience < 0 || doctorDto.YearsOfExperience > 60)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Anos de experiencia invalidos deben estar entre 0 y 60";
                    return result;
                }

                if (doctorDto.LicenseExpirationDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    result.Exitoso = false;
                    result.Mensaje = "La licencia debe estar vigente";
                    return result;
                }

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

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var doctor = await _repository.GetByIdAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Doctor no encontrado";
                    return result;
                }

                doctor.IsActive = false;
                doctor.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(doctor);

                result.Exitoso = true;
                result.Mensaje = "Doctor desactivado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar doctor {Id}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar doctor";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctors = await _repository.GetAllAsync();
                result.Datos = doctors.Select(MapToDto).ToList();
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
                var doctors = await _repository.GetAllWithDetailsAsync();
                result.Datos = doctors.Select(MapToDtoWithDetails).ToList();
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

        public async Task<OperationResult<List<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            var result = new OperationResult<List<AppointmentDto>>();
            try
            {
                var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
                result.Datos = appointments.Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    AppointmentDate = a.AppointmentDate,
                    StatusId = a.StatusId,
                    CreatedAt = a.CreatedAt
                }).ToList();
                result.Exitoso = true;
                result.Mensaje = "Citas del doctor obtenidas correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas del doctor {DoctorId}", doctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener citas del doctor";
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
                result.Mensaje = "Doctor obtenido correctamente por numero de licencia";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor por numero de licencia {License}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener doctor por numero de licencia";
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
                result.Mensaje = "Verificacion completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar licencia {License}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar doctor";
            }
            return result;
        }

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

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{3}-\d{3}-\d{4}$");
        }

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