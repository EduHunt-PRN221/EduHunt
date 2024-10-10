using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class Category : _Base
    {
        public string? Name { get; set; }
        public virtual ICollection<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;
    }
}
