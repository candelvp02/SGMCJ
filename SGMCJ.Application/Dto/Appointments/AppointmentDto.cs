namespace SGMCJ.Application.Dto.Appointments
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        //public string? Reason { get; set; }
        //public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}