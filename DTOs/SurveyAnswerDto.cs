namespace Eduhunt.DTOs
{
    public class SurveyAnswerDto
    {
        public int SurveyId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public string UserId { get; set; } = default!;
        public List<int> AnswerIds { get; set; } = default!;
    }
}
