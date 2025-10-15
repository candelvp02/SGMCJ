using SGMCJ.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMCJ.Domain.Entities.Medical
{
    [Table("Specialties", Schema = "medical")]
    public partial class Specialty
    {
        public short SpecialtyId { get; set; }

        public string SpecialtyName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}