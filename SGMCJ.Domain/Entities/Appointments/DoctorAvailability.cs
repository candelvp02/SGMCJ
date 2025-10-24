﻿using SGMCJ.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMCJ.Domain.Entities.Appointments
{
    [Table("DoctorAvailability", Schema = "appointments")]
    public partial class DoctorAvailability
    {
        public readonly int Id;

        public int AvailabilityId { get; set; }

        public int DoctorId { get; set; }

        public DateOnly AvailableDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public virtual Doctor? Doctor { get; set; }
        public bool IsActive { get; set; }
    }
}
