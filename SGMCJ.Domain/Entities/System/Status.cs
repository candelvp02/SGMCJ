using SGMCJ.Domain.Entities.Appointments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SGMCJ.Domain.Entities.System
{
    [Table("Status", Schema = "system")]
    public partial class Status
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}