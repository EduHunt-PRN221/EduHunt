using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{

    public partial class SurveyAnswer : _Base
    {
        public required string SurveyId { get; set; }

        public required string AnswerId { get; set; }

        public virtual Answer? Answer { get; set; }

        public virtual Survey? Survey { get; set; }
    }
}