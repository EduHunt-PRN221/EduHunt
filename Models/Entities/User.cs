using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class User : _Base
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
