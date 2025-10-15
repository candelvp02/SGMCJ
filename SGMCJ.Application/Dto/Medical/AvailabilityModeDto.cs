namespace SGMCJ.Application.Dto.Medical
{
    public class AvailabilityModeDto
    {
        public short AvailabilityModeId { get; set; }
        public string AvailabilityMode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}