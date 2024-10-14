using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class Profile : _Base
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!; 
        public string UserName { get; set; } = default!; 
        public string Email { get; set; } = default!; 
        public string PhoneNumber { get; set; } = default!; 
        public string Country { get; set; } = default!; 
        public string City { get; set; } = default!; 
        public string Title { get; set; } = default!; 
        public string AvatarImage { get; set; } = default!; 
        public string CertificateImage { get; set; } = default!; 

        public string ApplicationUserId { get; set; } = default!; 
        public virtual ApplicationUser? ApplicationUser { get; set; } = default!;

    }
}
