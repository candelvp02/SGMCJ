using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Medical
{
    public class Paciente : Person
    {
        [Key]
        public int Id { get; set; } 

        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string ContactoEmergencia { get; set; } = string.Empty;
        public string TelefonoEmergencia { get; set; } = string.Empty;
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        public Paciente()
        {
            Citas = new List<Cita>();
            Sexo = string.Empty;
            Direccion = string.Empty;
            ContactoEmergencia = string.Empty;
            TelefonoEmergencia = string.Empty;
        }

        public int ObtenerCantidadCitas()
        {
            return Citas.Count;
        }
        public Cita ObtenerUltimaCita()
        {
            return Citas
                .Where(c => c.Estado == Configuration.EstadoCita.Completada)
                .OrderByDescending(c => c.FechaHora)
                .FirstOrDefault()!;
        }

        public Cita? ObtenerProximaCita()
        {
            return Citas
                .Where(c => (c.Estado == EstadoCita.Programada || c.Estado == Configuration.EstadoCita.Confirmada) && c.FechaHora > DateTime.Now)
                .OrderBy(c => c.FechaHora)
                .FirstOrDefault();
        }
        public bool TieneCitasPendientes()
        {
            return Citas.Any(c => c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada);
        }
        public List<Cita> ObtenerHistorialCitas()
        {
            return Citas
                 .Where(c => c.Estado == EstadoCita.Completada)
                 .OrderByDescending(c => c.FechaHora)
                 .ToList();
        }
        public int ObtenerCitasCanceladas()
        {
            return Citas.Count(c => c.Estado == EstadoCita.Cancelada);
        }
        public int ObtenerCitasNoAsistidas()
        {
            return Citas.Count(c => c.Estado == EstadoCita.NoAsistio);
        }
    }
}
