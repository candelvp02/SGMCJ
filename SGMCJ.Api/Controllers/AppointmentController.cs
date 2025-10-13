using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGMCJ.Domain.Repositories.Medical;

namespace SGMCJ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppoinmentRepository appoinmentRepository;

        public AppointmentController(IAppoinmentRepository appoinmentRepository)
        {
            this.appoinmentRepository = appoinmentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await appoinmentRepository.GetAllAsync();
            return Ok(appointments);
        }
        [HttpPost("Save")]
        public async Task<IActionResult> Save(SGMCJ.Domain.Entities.Medical.Appointment appointment)
        {
            var newAppointment = await appoinmentRepository.AddAsync(appointment);
            return Ok(newAppointment);
        }
    }
}
