using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Medical
{
    public class Medico : Person
    {
        [Key]
        public int Id { get; set; }

        public string NumeroLicencia { get; set; } = string.Empty;
        public Especialidad Especialidad { get; set; }
        public bool EsActivo { get; set; }
        public DateTime FechaNacimiento { get; set; } 
        public string Sexo { get; set; } = string.Empty;
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<Disponibilidad> Disponibilidades { get; set; } = new List<Disponibilidad>();

        public Medico()
        {
            EsActivo = true;
            Sexo = string.Empty;
            FechaNacimiento = DateTime.Now;
        }

        public string ObtenerEspecialidadNombre()
        {
            return Especialidad switch
            {
                Especialidad.Alergologia => "Alergologia",
                Especialidad.Anestesiologia => "Anestesiologia",
                Especialidad.Cardiologia => "Cardiologia",
                Especialidad.CirugiaGeneral => "Cirugia General",
                Especialidad.Dermatologia => "Dermatologia",
                Especialidad.Endocrinologia => "Endocrinologia",
                Especialidad.FisiatriaYRehabilitacion => "Fisiatria y Rehabilitacion",
                Especialidad.Gastroenterologia => "Gastroenterologia",
                Especialidad.Geriatria => "Geriatria",
                Especialidad.GinecologiaYObstetricia => "Ginecologia y Obstetricia",
                Especialidad.Hematologia => "Hematologia",
                Especialidad.Infectologia => "Infectologia",
                Especialidad.MedicinaGeneral => "Medicina General",
                Especialidad.Nefrologia => "Nefrologia",
                Especialidad.Neumologia => "Neumologia",
                Especialidad.Neurocirugia => "Neurocirugia",
                Especialidad.Neurologia => "Neurologia",
                Especialidad.Nutricion => "Nutricion",
                Especialidad.Oftalmologia => "Oftalmologia",
                Especialidad.Oncologia => "Oncologia",
                Especialidad.Ortopedia => "Ortopedia",
                Especialidad.Otorrinolaringologia => "Otorrinolaringologia",
                Especialidad.Pediatria => "Pediatria",
                Especialidad.Psiquiatria => "Psiquiatria",
                Especialidad.Reumatologia => "Reumatologia",
                Especialidad.Traumatologia => "Traumatologia",
                Especialidad.Urologia => "Urologia",
                _ => "No Especificada"
            };
        }

        public int ObtenerCantidadCitasPendientes()
        {
            return Citas.Count(c => c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada);
        }

        public bool TieneDisponibilidadActiva()
        {
            return Disponibilidades.Any(d => d.EsActivo);
        }

        public List<Disponibilidad> ObtenerDisponibilidadesPorDia(DayOfWeek dia)
        {
            return Disponibilidades.Where(d => d.DiaSemana == dia && d.EsActivo).ToList();
        }

        public List<Cita> ObtenerCitasPorDia(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fechaInicio.AddDays(1);
            return Citas.Where(c => c.FechaHora >= fechaInicio && c.FechaHora < fechaFin).ToList();
        }
    }
}
