using SGMCJ.Domain.Base;
using SGMCJ.Domain.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SGMCJ.Domain.Entities.Medical
{
    public class Cita : AuditEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;
        public ICollection<Recordatorio> Recordatorios { get; set; } = new List<Recordatorio>();

        public Cita()
        {
            Estado = EstadoCita.Programada;
        }

        public bool PuedeSerCancelada()
        {
            return Estado == EstadoCita.Programada || Estado == EstadoCita.Confirmada;
        }

        public bool PuedeSerConfirmada()
        {
            return Estado == EstadoCita.Programada;
        }

        public TimeSpan TiempoRestante()
        {
            return FechaHora - DateTime.Now;
        }

        public bool EsCitaPasada()
        {
            return FechaHora < DateTime.Now;
        }

        public void Confirmar()
        {
            if (PuedeSerConfirmada())
            {
                Estado = EstadoCita.Confirmada;
                FechaModificacion = DateTime.Now;
            }
        }

        public void Cancelar()
        {
            if (PuedeSerCancelada())
            {
                Estado = EstadoCita.Cancelada;
                FechaModificacion = DateTime.Now;
            }
        }

        public void IniciarConsulta()
        {
            Estado = EstadoCita.EnCurso;
            FechaModificacion = DateTime.Now;
        }

        public void Completar(string? observaciones = null)
        {
            Estado = EstadoCita.Completada;
            if (!string.IsNullOrEmpty(observaciones))
            {
                Observaciones = observaciones;
            }
            FechaModificacion = DateTime.Now;
        }
    }
}