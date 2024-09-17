using Eduhunt.Models.Contracts;

namespace EDUHUNT_BE.Models
{
    public class ScholarshipCategory : _Base
    {
        public Guid ScholarshipId { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
