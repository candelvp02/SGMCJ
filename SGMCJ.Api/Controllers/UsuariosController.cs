using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;
using SGMCJ.Domain.Dto;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> GetAll()
        {
            var result = await _usuarioService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> GetById(int id)
        {
            var result = await _usuarioService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Create(UsuarioDto usuarioDto)
        {
            var result = await _usuarioService.CreateAsync(usuarioDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Update(int id, UsuarioDto usuarioDto)
        {
            if (id != usuarioDto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));
            var result = await _usuarioService.UpdateAsync(usuarioDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Delete(int id)
        {
            var result = await _usuarioService.DeleteAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("activos")]
        public async Task<ActionResult<OperationResult>> GetActivos()
        {
            var result = await _usuarioService.ListarActivosAsync();
            return Ok(result);
        }

        [HttpGet("rol/{rol}")]
        public async Task<ActionResult<OperationResult>> GetPorRol(string rol)
        {
            var result = await _usuarioService.ListarPorRolAsync(rol);
            return Ok(result);
        }
    }
}