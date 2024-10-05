using Eduhunt.Models.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduhunt.Models.Entities
{
    public class Profile : _Base
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Title { get; set; }
        public string? AvatarImage { get; set; }

        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } = default!;

    }
}
