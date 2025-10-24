﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SGMCJ.Domain.Entities.Appointments
{

    [Table("Appointments", Schema = "appointments")]
    public partial class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}