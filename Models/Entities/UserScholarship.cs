using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class UserScholarship : _Base
    {
        public required string ScholarshipId { get; set; }

        public required string UserId { get; set; }

    }
}
