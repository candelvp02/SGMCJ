namespace SGMCJ.Application.Dto.Base
{
    public abstract class RegisterPersonBaseDto : PersonBaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}