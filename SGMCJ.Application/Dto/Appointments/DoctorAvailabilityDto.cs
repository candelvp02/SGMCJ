namespace SGMCJ.Application.Dto.Appointments
{
    public class DoctorAvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class CreateDoctorAvailabilityDto
    {
        public int DoctorId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public class UpdateDoctorAvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}