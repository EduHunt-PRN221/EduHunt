using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{

    public partial class SurveyAnswer : _Base
    {
        public string SurveyId { get; set; }

        public string AnswerId { get; set; }

        public virtual Answer Answer { get; set; } = default!;

        public virtual Survey Survey { get; set; } = default!;
    }
}