using Eduhunt.Models.Contracts;
namespace Eduhunt.Models.Entities
{
    public class Roadmap : _Base
    {
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public int Status { get; set; } = 0;

        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; } = default!;
    }
}
