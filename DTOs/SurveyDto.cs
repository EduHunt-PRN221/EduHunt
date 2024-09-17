namespace Eduhunt.DTOs
{
    public class SurveyDto
    {
        public string UserId { get; set; } = default!;
        public List<GetSurvey> SurveyAnswers { get; set; } = new List<GetSurvey>();
    }
    public class GetSurvey
    {
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
    }
}