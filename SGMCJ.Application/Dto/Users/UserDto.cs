using SGMCJ.Application.Dto.Base;

namespace SGMCJ.Application.Dto.Users
{
    public class UserDto : PersonBaseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class RegisterUserDto : RegisterPersonBaseDto
    {
        public int RoleId { get; set; }
    }
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}