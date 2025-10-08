using SGMCJ.Domain.Dto.Common;

namespace SGMCJ.Domain.Dto
{
    public class MedicoDto : PersonaGetDtoBase
    {
        public string NumeroLicencia { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }
}