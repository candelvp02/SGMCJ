namespace SGMCJ.Application.Dto.System
{
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class CreateStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
    }
}