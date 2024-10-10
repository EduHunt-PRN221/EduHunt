namespace Eduhunt.DTOs
{
    public class SurveyAnswerDto
    {
        public required string SurveyId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public string UserId { get; set; } = default!;
        public List<string> AnswerIds { get; set; } = default!;
    }
}
