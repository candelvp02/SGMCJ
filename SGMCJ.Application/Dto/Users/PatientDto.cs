﻿using SGMCJ.Application.Dto.Base;

namespace SGMCJ.Application.Dto.Users
{
    public class PatientDto : PersonBaseDto
    {
        public int PatientId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public int InsuranceProviderId { get; set; }
        public string InsuranceProviderName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class RegisterPatientDto : RegisterPersonBaseDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public int InsuranceProviderId { get; set; }
    }

    public class UpdatePatientDto
    {
        public int PatientId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public int InsuranceProviderId { get; set; }
    }
}