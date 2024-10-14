using Eduhunt.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Eduhunt.Models.Entities
{
    public class Scholarship : _Base
    {
        [Required]
        public string Budget { get; set; } = default!;

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Location { get; set; } = default!;

        [Required]
        public string SchoolName { get; set; } = default!;

        [Required]
        public string Level { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string Url { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;

        public string AuthorId { get; set; } = default!;

        public bool IsInSite { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;

        public virtual ICollection<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;

    }
}