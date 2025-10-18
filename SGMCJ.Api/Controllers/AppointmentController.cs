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

namespace SGMCJ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
       private readonly IAppointmentService _appointmentService;
       
       public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (!result.Exitoso)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto createDto)
        {
            var result = await _appointmentService.CreateAsync(createDto);
            if (!result.Exitoso)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.AppointmentId }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto updateDto)
        {
            if (id != updateDto.AppointmentId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
            }

            var result = await _appointmentService.UpdateAsync(updateDto);

            if (!result.Exitoso)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _appointmentService.ConfirmAsync(id);
            if (!result.Exitoso)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _appointmentService.CancelAsync(id);
            if (!result.Exitoso)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("{id}/reschedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleRequest request)
        {
            var result = await _appointmentService.RescheduleAsync(id, request.NewDate);
            if (!result.Exitoso)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
    public class RescheduleRequest
    {
        public DateTime NewDate { get; set; }
    }
}