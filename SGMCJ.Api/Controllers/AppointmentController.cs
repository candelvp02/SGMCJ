using Microsoft.AspNetCore.Mvc;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Repositories.Appointments;

namespace SGMCJ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentController(IAppointmentRepository appoinmentRepository)
        {
            this.appointmentRepository = appoinmentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await appointmentRepository.GetAllAsync();
            return Ok(appointments);
        }
        [HttpPost("Save")]
        public async Task<IActionResult> Save(Appointment appointment)
        {
            var newAppointment = await appointmentRepository.AddAsync(appointment);
            return Ok(newAppointment);
        }
    }
}