using SGMCJ.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Medical
{
    public class Disponibilidad : AuditEntity
    {
        [Key]
        public int Id { get; set; } 
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool EsActivo { get; set; }

        public Disponibilidad()
        {
            EsActivo = true;
        }
        public bool EstaDisponibleEnHorario(TimeSpan hora)
        {
            return EsActivo && hora >= HoraInicio && hora <= HoraFin;
        }
        public TimeSpan DuracionJornada()
        {
            return HoraFin - HoraInicio;
        }
        public string ObtenerNombreDia()
        {
            return DiaSemana switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => "Desconocido"
            };
        }
        public void Activar()
        {
            EsActivo = true;
            FechaModificacion = DateTime.Now;
        }
        public void Desactivar()
        {
            EsActivo = false;
            FechaModificacion = DateTime.Now;
        }
    }
}
