using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public class Cv : _Base
    {
        public Guid UserId { get; set; }
        public string? UrlCV { get; set; }
    }
}
