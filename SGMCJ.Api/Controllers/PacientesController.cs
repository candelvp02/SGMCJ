using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPatientService _pacienteService;

        public PacientesController(IPatientService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult<List<PatientDto>>>> GetAll()
        {
            var result = await _pacienteService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult<PatientDto>>> GetById(int id)
        {
            var result = await _pacienteService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<OperationResult<PatientDto>>> GetByIdWithDetails(int id)
        {
            var result = await _pacienteService.GetByIdWithDetailsAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult<PatientDto>>> Create(RegisterPatientDto registerDto)
        {
            var result = await _pacienteService.CreateAsync(registerDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.PatientId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult<PatientDto>>> Update(int id, UpdatePatientDto updateDto)
        {
            if (id != updateDto.PatientId)
                return BadRequest(OperationResult.Fallo("ID no coincide"));

            var result = await _pacienteService.UpdateAsync(updateDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Delete(int id)
        {
            var result = await _pacienteService.DeleteAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("activos")]
        public async Task<ActionResult<OperationResult<List<PatientDto>>>> GetActivos()
        {
            var result = await _pacienteService.GetActiveAsync();
            return Ok(result);
        }

        [HttpGet("phone/{phoneNumber}")]
        public async Task<ActionResult<OperationResult<PatientDto>>> GetByPhoneNumber(string phoneNumber)
        {
            var result = await _pacienteService.GetByPhoneNumberAsync(phoneNumber);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("insurance/{insuranceProviderId}")]
        public async Task<ActionResult<OperationResult<List<PatientDto>>>> GetByInsuranceProvider(int insuranceProviderId)
        {
            var result = await _pacienteService.GetByInsuranceProviderAsync(insuranceProviderId);
            return Ok(result);
        }

        [HttpGet("{id}/appointments")]
        public async Task<ActionResult<OperationResult<List<PatientDto>>>> GetWithAppointments(int id)
        {
            var result = await _pacienteService.GetWithAppointmentsAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/medical-records")]
        public async Task<ActionResult<OperationResult<List<PatientDto>>>> GetWithMedicalRecords(int id)
        {
            var result = await _pacienteService.GetWithMedicalRecordsAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/exists")]
        public async Task<ActionResult<OperationResult<bool>>> Exists(int id)
        {
            var result = await _pacienteService.ExistsAsync(id);
            return Ok(result);
        }
    }
}
