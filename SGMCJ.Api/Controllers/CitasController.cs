using Microsoft.AspNetCore.Mvc;
using SGMCJ.Domain.Dto;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;
        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> GetAll()
        {
            var result = await _citaService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> GetById(int id)
        {
            var result = await _citaService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Create(CitaDto citaDto)
        {
            var result = await _citaService.CreateAsync(citaDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Update(int id, CitaDto citaDto)
        {
            if (id != citaDto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));
            var result = await _citaService.UpdateAsync(citaDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("paciente/{pacienteId}")]
        public async Task<ActionResult<OperationResult>> GetPorPaciente(int pacienteId)
        {
            var result = await _citaService.GetByPacienteIdAsync(pacienteId);
            return Ok(result);
        }

        [HttpGet("medico/{medicoId}")]
        public async Task<ActionResult<OperationResult>> GetPorMedico(int medicoId)
        {
            var result = await _citaService.GetByMedicoIdAsync(medicoId);
            return Ok(result);
        }

        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<OperationResult>> GetPorFecha(DateTime fecha)
        {
            var result = await _citaService.GetByFechaAsync(fecha);
            return Ok(result);
        }

        [HttpPut("{id}/cancelar")]
        public async Task<ActionResult<OperationResult>> Cancelar(int id, [FromBody] CancelarCitaRequest request)
        {
            var result = await _citaService.CancelarCitaAsync(id, request.Motivo);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/confirmar")]
        public async Task<ActionResult<OperationResult>> Confirmar(int id)
        {
            var result = await _citaService.ConfirmarCitaAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/reprogramar")]
        public async Task<ActionResult<OperationResult>> Reprogramar(int id, [FromBody] ReprogramarCitaRequest request)
        {
            if (request == null)
                return BadRequest(OperationResult.Fallo("La solicitud no puede ser nula"));

            if (id <= 0)
                return BadRequest(OperationResult.Fallo("ID de cita inválido"));

            if (request.NuevaFecha == default)
                return BadRequest(OperationResult.Fallo("La nueva fecha es requerida"));

            var result = await _citaService.ReprogramarCitaAsync(id, request.NuevaFecha);

            if (result == null)
                return StatusCode(500, OperationResult.Fallo("Error interno del servidor"));

            if (!result.Exitoso)
                return BadRequest(result);

            return Ok(result);
        }
    }

    public class CancelarCitaRequest
    {
        public string Motivo { get; set; } = string.Empty;
    }

    public class ReprogramarCitaRequest
    {
        public DateTime NuevaFecha { get; set; }
    }
}