namespace SGMCJ.Application.Dto.Medical
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public DateTime DateOfVisit { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public DateTime DateOfVisit { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        public int RecordId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public DateTime DateOfVisit { get; set; }
    }
}