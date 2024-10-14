using System.ComponentModel.DataAnnotations;

namespace Eduhunt.DTOs
{
    public class ScholarshipDto
    {
        public required string Id { get; set; }

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
        public string? AuthorId { get; set; }
        public bool IsInSite { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
    }
}