using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public partial class Survey : _Base
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public DateTime CreateAt { get; set; }

        public virtual ICollection<SurveyAnswer> SurveyAnswers { get; set; } = default!;
    }
}
