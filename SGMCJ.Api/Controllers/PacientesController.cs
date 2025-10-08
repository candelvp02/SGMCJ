using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Dto;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> GetAll()
        {
            var result = await _pacienteService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> GetById(int id)
        {
            var result = await _pacienteService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Create(PacienteDto pacienteDto)
        {
            var result = await _pacienteService.CreateAsync(pacienteDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Update(int id, PacienteDto pacienteDto)
        {
            if (id != pacienteDto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));
            var result = await _pacienteService.UpdateAsync(pacienteDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("identificacion/{identificacion}")]
        public async Task<ActionResult<OperationResult>> GetByIdentificacion(string identificacion)
        {
            var result = await _pacienteService.ObtenerPorCedulaAsync(identificacion); // ✅ Cambiado a 'ObtenerPorCedulaAsync'
            if (!result.Exitoso)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("activos")]
        public async Task<ActionResult<OperationResult>> GetActivos()
        {
            var result = await _pacienteService.ListarActivosAsync();
            return Ok(result);
        }
    }
}