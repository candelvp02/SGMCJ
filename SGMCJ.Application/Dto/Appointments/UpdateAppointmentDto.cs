namespace SGMCJ.Application.Dto.Appointments
{
    public class UpdateAppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public string? Notes { get; set; }
    }
}