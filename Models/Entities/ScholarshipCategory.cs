using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class ScholarshipCategory : _Base
    {
        public required string ScholarshipId { get; set; }

        public required string CategoryId { get; set; }

        public virtual Scholarship? Scholarship { get; set; }

        public virtual Category? Category { get; set; }
    }
}
