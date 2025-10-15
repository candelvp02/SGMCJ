using Microsoft.Extensions.Logging;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;

namespace SGMCJ.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repoEf;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository repoEf,
            ILogger<DoctorService> logger)
        {
            _repoEf = repoEf;
            _logger = logger;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctors = await _repoEf.GetAllAsync();
                result.Datos = doctors.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Médicos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todos los médicos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médicos";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetAllWithDetailsAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var doctors = await _repoEf.GetAllWithDetailsAsync();
                result.Datos = doctors.Select(MapToDtoWithDetails).ToList();
                result.Exitoso = true;
                result.Mensaje = "Médicos con detalles obtenidos correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todos los médicos con detalles");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médicos con detalles";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repoEf.GetByIdAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Médico no encontrado";
                    result.Errores.Add("Médico no encontrado");
                    return result;
                }

                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Médico obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo médico {DoctorId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médico";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdWithDetailsAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repoEf.GetByIdWithDetailsAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Médico no encontrado";
                    result.Errores.Add("Médico no encontrado");
                    return result;
                }

                result.Datos = MapToDtoWithDetails(doctor);
                result.Exitoso = true;
                result.Mensaje = "Médico con detalles obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo médico con detalles {DoctorId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médico con detalles";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByIdWithAppointmentsAsync(int id)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repoEf.GetByIdWithAppointmentsAsync(id);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Médico no encontrado";
                    result.Errores.Add("Médico no encontrado");
                    return result;
                }

                result.Datos = MapToDtoWithDetails(doctor);
                result.Exitoso = true;
                result.Mensaje = "Médico con citas obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo médico con citas {DoctorId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médico con citas";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> CreateAsync(DoctorDto doctorDto)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                if (doctorDto == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Los datos del médico son requeridos";
                    result.Errores.Add("Los datos del médico son requeridos");
                    return result;
                }

                // Validar licencia única
                var existeLicencia = await _repoEf.ExistsByLicenseNumberAsync(doctorDto.LicenseNumber);
                if (existeLicencia)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Ya existe un médico con ese número de licencia";
                    result.Errores.Add("Ya existe un médico con ese número de licencia");
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

                var createdDoctor = await _repoEf.AddAsync(doctor);
                result.Datos = MapToDto(createdDoctor);
                result.Exitoso = true;
                result.Mensaje = "Médico creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando médico");
                result.Exitoso = false;
                result.Mensaje = "Error al crear médico";
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
                    result.Mensaje = "Los datos del médico son requeridos";
                    result.Errores.Add("Los datos del médico son requeridos");
                    return result;
                }

                var doctor = await _repoEf.GetByIdAsync(doctorDto.DoctorId);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Médico no encontrado";
                    result.Errores.Add("Médico no encontrado");
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

                await _repoEf.UpdateAsync(doctor);
                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Médico actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando médico {DoctorId}", doctorDto?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar médico";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetBySpecialtyIdAsync(short specialtyId)
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var allDoctors = await _repoEf.GetAllAsync();
                var doctors = allDoctors.Where(d => d.SpecialtyId == specialtyId).ToList();
                result.Datos = doctors.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Médicos obtenidos por especialidad";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando médicos por especialidad {SpecialtyId}", specialtyId);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médicos por especialidad";
            }
            return result;
        }

        public async Task<OperationResult<List<DoctorDto>>> GetActiveDoctorsAsync()
        {
            var result = new OperationResult<List<DoctorDto>>();
            try
            {
                var allDoctors = await _repoEf.GetAllAsync();
                var doctors = allDoctors.Where(d => d.IsActive).ToList();
                result.Datos = doctors.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Médicos activos obtenidos";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando médicos activos");
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médicos activos";
            }
            return result;
        }

        public async Task<OperationResult<DoctorDto>> GetByLicenseNumberAsync(string licenseNumber)
        {
            var result = new OperationResult<DoctorDto>();
            try
            {
                var doctor = await _repoEf.GetByLicenseNumberAsync(licenseNumber);
                if (doctor == null)
                {
                    result.Exitoso = false;
                    result.Mensaje = "Médico no encontrado";
                    result.Errores.Add("Médico no encontrado");
                    return result;
                }

                result.Datos = MapToDto(doctor);
                result.Exitoso = true;
                result.Mensaje = "Médico obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo médico por licencia {LicenseNumber}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médico";
            }
            return result;
        }

        public async Task<OperationResult<bool>> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existe = await _repoEf.ExistsByLicenseNumberAsync(licenseNumber);
                result.Datos = existe;
                result.Exitoso = true;
                result.Mensaje = "Verificación completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando médico con licencia {LicenseNumber}", licenseNumber);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar médico";
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
                    result.Mensaje = "La entidad doctor es requerida";
                    result.Errores.Add("La entidad doctor es requerida");
                    return result;
                }

                var createdDoctor = await _repoEf.AddAsync(doctor);
                result.Datos = createdDoctor;
                result.Exitoso = true;
                result.Mensaje = "Médico creado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando médico entity");
                result.Exitoso = false;
                result.Mensaje = "Error al crear médico";
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
                    result.Mensaje = "La entidad doctor es requerida";
                    result.Errores.Add("La entidad doctor es requerida");
                    return result;
                }

                await _repoEf.UpdateAsync(doctor);
                result.Datos = doctor;
                result.Exitoso = true;
                result.Mensaje = "Médico actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando médico entity {DoctorId}", doctor?.DoctorId);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar médico";
            }
            return result;
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