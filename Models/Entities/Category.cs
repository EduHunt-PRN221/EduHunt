using Eduhunt.Models.Contracts;

namespace EDUHUNT_BE.Models
{
    public class Category : _Base
    {
        public string? Name { get; set; }
        public virtual ICollection<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;
    }
}
