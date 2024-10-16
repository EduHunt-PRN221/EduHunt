using Eduhunt.Models.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduhunt.Models.Entities
{
    public partial class Survey : _Base
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public required string UserId { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateAt { get; set; }

        public virtual ICollection<SurveyAnswer>? SurveyAnswers { get; set; }
    }
}
