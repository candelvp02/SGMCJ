namespace SGMCJ.Application.Dto.Insurance
{
    public class InsuranceProviderDto
    {
        public int InsuranceProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string CoverageDetails { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public bool IsPreferred { get; set; }
        public int NetworkTypeId { get; set; }
        public string NetworkTypeName { get; set; } = string.Empty;
        public decimal? MaxCoverageAmount { get; set; }
        public bool IsActive { get; set; }
        public object Id { get; internal set; }
    }
    public class CreateInsuranceProviderDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateInsuranceProviderDto
    {
        public int InsuranceProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}