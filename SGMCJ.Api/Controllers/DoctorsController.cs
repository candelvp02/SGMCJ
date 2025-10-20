using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Dto.Users;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult<List<DoctorDto>>>> GetAll()
        {
            var result = await _doctorService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("with-details")]
        public async Task<ActionResult<OperationResult<List<DoctorDto>>>> GetAllWithDetails()
        {
            var result = await _doctorService.GetAllWithDetailsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult<DoctorDto>>> GetById(int id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/with-details")]
        public async Task<ActionResult<OperationResult<DoctorDto>>> GetByIdWithDetails(int id)
        {
            var result = await _doctorService.GetByIdWithDetailsAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}/with-appointments")]
        public async Task<ActionResult<OperationResult<DoctorDto>>> GetByIdWithAppointments(int id)
        {
            var result = await _doctorService.GetByIdWithAppointmentsAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult<DoctorDto>>> Create(DoctorDto doctorDto)
        {
            var result = await _doctorService.CreateAsync(doctorDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.DoctorId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult<DoctorDto>>> Update(int id, UpdateDoctorDto updateDto)
        {
            if (id != updateDto.DoctorId)
                return BadRequest(OperationResult.Fallo("ID no coincide"));

            var result = await _doctorService.UpdateAsync(updateDto);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("specialty/{specialtyId}")]
        public async Task<ActionResult<OperationResult<List<DoctorDto>>>> GetBySpecialty(short specialtyId)
        {
            var result = await _doctorService.GetBySpecialtyIdAsync(specialtyId);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<OperationResult<List<DoctorDto>>>> GetActive()
        {
            var result = await _doctorService.GetActiveDoctorsAsync();
            return Ok(result);
        }

        [HttpGet("license/{licenseNumber}")]
        public async Task<ActionResult<OperationResult<DoctorDto>>> GetByLicenseNumber(string licenseNumber)
        {
            var result = await _doctorService.GetByLicenseNumberAsync(licenseNumber);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("license/{licenseNumber}/exists")]
        public async Task<ActionResult<OperationResult<bool>>> ExistsByLicenseNumber(string licenseNumber)
        {
            var result = await _doctorService.ExistsByLicenseNumberAsync(licenseNumber);
            return Ok(result);
        }
    }
}
