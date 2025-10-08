using Microsoft.Extensions.Logging;
using SGMCJ.Domain.Dto;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Interfaces.Repositories;
using SGMCJ.Domain.Repositories.Medical;

namespace SGMCJ.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _repoEf;
        private readonly IMedicoAdoRepository _repoAdo;
        private readonly ILogger<MedicoService> _logger;

        public MedicoService(
            IMedicoRepository repoEf,
            IMedicoAdoRepository repoAdo,
            ILogger<MedicoService> logger)
        {
            _repoEf = repoEf;
            _repoAdo = repoAdo;
            _logger = logger;
        }

        public async Task<OperationResult<List<MedicoDto>>> GetAllAsync()
        {
            var result = new OperationResult<List<MedicoDto>>();
            try
            {
                var medicos = await _repoEf.GetAllAsync();
                result.Datos = medicos.Select(MapToDto).ToList();
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

        public async Task<OperationResult<MedicoDto>> GetByIdAsync(int id)
        {
            var result = new OperationResult<MedicoDto>();
            try
            {
                var medico = await _repoEf.GetByIdAsync(id);
                if (medico == null)
                    return Fail(result, "Médico no encontrado");

                result.Datos = MapToDto(medico);
                result.Exitoso = true;
                result.Mensaje = "Médico obtenido correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo médico {MedicoId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médico";
            }
            return result;
        }

        public async Task<OperationResult<MedicoDto>> CreateAsync(MedicoDto medicoDto)
        {
            var result = new OperationResult<MedicoDto>();
            try
            {
                var medico = new Medico
                {
                    Nombre = medicoDto.Nombre,
                    Apellido = medicoDto.Apellido,
                    Cedula = medicoDto.Cedula,
                    NumeroLicencia = medicoDto.NumeroLicencia,
                    Especialidad = Enum.Parse<Especialidad>(medicoDto.Especialidad, true),
                    Telefono = medicoDto.Telefono,
                    Email = medicoDto.Email,
                    EsActivo = true,
                    FechaCreacion = DateTime.Now
                };

                var medicoCreado = await _repoEf.AddAsync(medico);
                result.Datos = MapToDto(medicoCreado);
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

        public async Task<OperationResult<MedicoDto>> UpdateAsync(MedicoDto medicoDto)
        {
            var result = new OperationResult<MedicoDto>();
            try
            {
                var medico = await _repoEf.GetByIdAsync(medicoDto.Id);
                if (medico == null)
                    return Fail(result, "Médico no encontrado");

                medico.Nombre = medicoDto.Nombre;
                medico.Apellido = medicoDto.Apellido;
                medico.Cedula = medicoDto.Cedula;
                medico.NumeroLicencia = medicoDto.NumeroLicencia;
                medico.Especialidad = Enum.Parse<Especialidad>(medicoDto.Especialidad, true);
                medico.Telefono = medicoDto.Telefono;
                medico.Email = medicoDto.Email;

                await _repoEf.UpdateAsync(medico);
                result.Datos = MapToDto(medico);
                result.Exitoso = true;
                result.Mensaje = "Médico actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando médico {MedicoId}", medicoDto.Id);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar médico";
            }
            return result;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var medico = await _repoEf.GetByIdAsync(id);
                if (medico == null)
                    return Fail(result, "Médico no encontrado");

                medico.EsActivo = false;
                await _repoEf.UpdateAsync(medico);

                result.Exitoso = true;
                result.Mensaje = "Médico eliminado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando médico {MedicoId}", id);
                result.Exitoso = false;
                result.Mensaje = "Error al eliminar médico";
            }
            return result;
        }

        public async Task<OperationResult<List<MedicoDto>>> ListarPorEspecialidadAsync(Especialidad especialidad)
        {
            var result = new OperationResult<List<MedicoDto>>();
            try
            {
                var medicos = await _repoAdo.ListarPorEspecialidadAsync(especialidad);
                result.Datos = medicos.Select(MapToDto).ToList();
                result.Exitoso = true;
                result.Mensaje = "Médicos obtenidos por especialidad";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando médicos por especialidad {Especialidad}", especialidad);
                result.Exitoso = false;
                result.Mensaje = "Error al obtener médicos por especialidad";
            }
            return result;
        }

        public async Task<OperationResult<List<MedicoDto>>> ListarActivosAsync()
        {
            var result = new OperationResult<List<MedicoDto>>();
            try
            {
                var medicos = await _repoAdo.ListarActivosAsync();
                result.Datos = medicos.Select(MapToDto).ToList();
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

        public async Task<OperationResult<bool>> ExisteMedicoAsync(string cedula)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existe = await _repoEf.ExisteMedicoAsync(cedula);
                result.Datos = existe;
                result.Exitoso = true;
                result.Mensaje = "Verificación completada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando médico con cédula {Cedula}", cedula);
                result.Exitoso = false;
                result.Mensaje = "Error al verificar médico";
            }
            return result;
        }

        public async Task<OperationResult<Medico>> CreateEntityAsync(Medico medico)
        {
            var result = new OperationResult<Medico>();
            try
            {
                var medicoCreado = await _repoEf.AddAsync(medico);
                result.Datos = medicoCreado;
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

        public async Task<OperationResult<Medico>> UpdateEntityAsync(Medico medico)
        {
            var result = new OperationResult<Medico>();
            try
            {
                await _repoEf.UpdateAsync(medico);
                result.Datos = medico;
                result.Exitoso = true;
                result.Mensaje = "Médico actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando médico entity {MedicoId}", medico.Id);
                result.Exitoso = false;
                result.Mensaje = "Error al actualizar médico";
            }
            return result;
        }

        private static OperationResult<T> Fail<T>(OperationResult<T> r, string mensaje)
        {
            r.Exitoso = false;
            r.Mensaje = mensaje;
            r.Errores.Add(mensaje);
            return r;
        }

        private static OperationResult Fail(OperationResult r, string mensaje)
        {
            r.Exitoso = false;
            r.Mensaje = mensaje;
            r.Errores.Add(mensaje);
            return r;
        }

        private static MedicoDto MapToDto(Medico m) => new()
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Apellido = m.Apellido,
            Cedula = m.Cedula,
            NumeroLicencia = m.NumeroLicencia,
            Especialidad = m.Especialidad.ToString(),
            Telefono = m.Telefono,
            Email = m.Email
        };

        public Task<OperationResult<List<MedicoDto>>> GetByEspecialidadAsync(Especialidad especialidad)
        {
            throw new NotImplementedException();
        }
    }
}