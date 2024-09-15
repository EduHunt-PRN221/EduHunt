using Eduhunt.DTOs;
using static Eduhunt.DTOs.ServiceResponses;

namespace Eduhunt.Infrastructures.Repositories
{
    public interface ISurvey
    {
        Task<GeneralResponse> CreateSurvey(SurveyAnswerDto survey);
        Task<SurveyDto> Get(int id);
        Task<GeneralResponse> UpdateSurvey(SurveyAnswerDto surveyObj);
        Task<GeneralResponse> Delete(int surveyId);
        Task<ICollection<SurveyDto>> GetAll();
        Task<SurveyDto> GetByUserId(string userId);
    }
}