using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using SGMCJ.Domain.Dto;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicosController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> GetAll()
        {
            var result = await _medicoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> GetById(int id)
        {
            var result = await _medicoService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Create(MedicoDto medicoDto)
        {
            var result = await _medicoService.CreateAsync(medicoDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Update(int id, MedicoDto medicoDto)
        {
            if (id != medicoDto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));
            var result = await _medicoService.UpdateAsync(medicoDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("especialidad/{especialidad}")]
        public async Task<ActionResult<OperationResult>> GetPorEspecialidad(string especialidad)
        {
            if (!Enum.TryParse<Especialidad>(especialidad, true, out var especialidadEnum))
            {
                return BadRequest(OperationResult.Fallo($"Especialidad '{especialidad}' no válida."));
            }

            var result = await _medicoService.GetByEspecialidadAsync(especialidadEnum);
            return Ok(result);
        }

        [HttpGet("activos")]
        public async Task<ActionResult<OperationResult>> GetActivos()
        {
            var result = await _medicoService.ListarActivosAsync();
            return Ok(result);
        }
    }
}