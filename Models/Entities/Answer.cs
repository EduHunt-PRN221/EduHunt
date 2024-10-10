using Eduhunt.Models.Contracts;

namespace Eduhunt.Models.Entities
{
    public partial class Answer : _Base
    {
        public string QuestionId { get; set; }

        public string AnswerText { get; set; } = default!;

        public virtual Question Question { get; set; } = default!;

        public virtual ICollection<SurveyAnswer> SurveyAnswers { get; set; } = default!;

        internal object Select(Func<object, Answer> value)
        {
            throw new NotImplementedException();
        }
    }
}