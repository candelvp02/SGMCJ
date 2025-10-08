using SGMCJ.Domain.Dto.Common;

namespace SGMCJ.Domain.Dto
{
    public class PacienteDto : PersonaGetDtoBase
    {
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}