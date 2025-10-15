﻿using SGMCJ.Domain.Base;
using SGMCJ.Domain.Entities.Appointments;
using SGMCJ.Domain.Entities.Insurance;
using SGMCJ.Domain.Entities.Medical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SGMCJ.Domain.Entities.Users
{
    [Table("Patients", Schema = "users")]
    public partial class Patient
    {
        public int PatientId { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string EmergencyContactName { get; set; } = string.Empty;

        public string EmergencyContactPhone { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        public string Allergies { get; set; } = string.Empty;

        public int InsuranceProviderId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public virtual InsuranceProvider? InsuranceProvider { get; set; }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public virtual Person? PatientNavigation { get; set; }
    }
}