//using Microsoft.AspNetCore.Mvc;
//using SGMCJ.Domain.Entities.Appointments;
//using SGMCJ.Domain.Repositories.Appointments;

//namespace SGMCJ.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AppointmentController : ControllerBase
//    {
//        private readonly IAppointmentRepository appointmentRepository;

//        public AppointmentController(IAppointmentRepository appoinmentRepository)
//        {
//            this.appointmentRepository = appoinmentRepository;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var appointments = await appointmentRepository.GetAllAsync();
//            return Ok(appointments);
//        }
//        [HttpPost("Save")]
//        public async Task<IActionResult> Save(Appointment appointment)
//        {
//            var newAppointment = await appointmentRepository.AddAsync(appointment);
//            return Ok(newAppointment);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Domain.Base;

namespace SGMCJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> GetById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Fallo("Datos inválidos"));

            var result = await _appointmentService.CreateAsync(dto);
            if (!result.Exitoso)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));

            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Fallo("Datos inválidos"));

            var result = await _appointmentService.UpdateAsync(dto);
            if (!result.Exitoso)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Delete(int id)
        {
            var result = await _appointmentService.DeleteAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetByPatient(int patientId)
        {
            var result = await _appointmentService.GetByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetByDoctor(int doctorId)
        {
            var result = await _appointmentService.GetByDoctorIdAsync(doctorId);
            return Ok(result);
        }
    }
}