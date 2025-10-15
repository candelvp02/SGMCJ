using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUserService _usuarioService;

        public UsuariosController(IUserService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult<List<UserDto>>>> GetAll()
        {
            var result = await _usuarioService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult<UserDto>>> GetById(int id)
        {
            var result = await _usuarioService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult<UserDto>>> Create(RegisterUserDto registerDto)
        {
            var result = await _usuarioService.CreateAsync(registerDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.UserId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult<UserDto>>> Update(int id, UpdateUserDto updateDto)
        {
            if (id != updateDto.UserId)
                return BadRequest(OperationResult.Fallo("ID no coincide"));

            var result = await _usuarioService.UpdateAsync(updateDto);
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
        public async Task<ActionResult<OperationResult<List<UserDto>>>> GetActivos()
        {
            var result = await _usuarioService.GetActiveAsync();
            return Ok(result);
        }

        [HttpGet("rol/{roleId}")]
        public async Task<ActionResult<OperationResult<List<UserDto>>>> GetPorRol(int roleId)
        {
            var result = await _usuarioService.GetByRoleAsync(roleId);
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<OperationResult<UserDto>>> GetByEmail(string email)
        {
            var result = await _usuarioService.GetByEmailAsync(email);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<ActionResult<OperationResult<bool>>> ValidateCredentials([FromBody] LoginRequest request)
        {
            var result = await _usuarioService.ValidateCredentialsAsync(request.Username, request.Password);
            return Ok(result);
        }

        [HttpPatch("{id}/activate")]
        public async Task<ActionResult<OperationResult>> Activate(int id)
        {
            var result = await _usuarioService.ActivateAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult<OperationResult>> Deactivate(int id)
        {
            var result = await _usuarioService.DeactivateAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}