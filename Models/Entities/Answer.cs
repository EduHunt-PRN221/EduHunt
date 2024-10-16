using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public partial class Answer : _Base
    {
        public required string QuestionId { get; set; }

        public string? AnswerText { get; set; }

        public virtual Question? Question { get; set; }

        public virtual ICollection<SurveyAnswer>? SurveyAnswers { get; set; }

        internal object Select(Func<object, Answer> value)
        {
            throw new NotImplementedException();
        }
    }
}