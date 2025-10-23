namespace SGMCJ.Application.Dto.Appointments
{
    public class AvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class CreateAvailabilityDto
    {
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateAvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class AvailabilityModeDto
    {
        public short AvailabilityModeId { get; set; }
        public string AvailabilityMode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}