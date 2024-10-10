using AutoMapper;
using Eduhunt.Data;
using Eduhunt.DTOs;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.Survey
{
    public class SurveyService : Repository<Models.Entities.Survey>
    {
        public SurveyService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) :
                base(
                    context,
                    httpContextAccessor,
                    mapper)
        {
        }

        public async Task DeleteSurveyById(string surveyId)
        {
            if (_context == null)
            {
                return ;
            }

            try
            {
                var surveys = await _context.Surveys.FirstOrDefaultAsync(sa => sa.Id == surveyId);
                if (surveys == null)
                {
                    return ;
                }

                var existingSurveyAnswers = await _context.SurveyAnswers.Where(sa => sa.Id == surveyId).ToListAsync();
                _context.SurveyAnswers.RemoveRange(existingSurveyAnswers);
                _context.Surveys.Remove(surveys);
                await _context.SaveChangesAsync();

                return ;
            }
            catch (Exception ex)
            {
                return ;
            }
        }

        public async Task<SurveyDto> GetSurveyById(string id)
        {
            if (_context == null)
            {
                return null!;
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return null!;
            }

            return _mapper.Map<SurveyDto>(survey);
        }

        public async Task CreateSurvey(SurveyAnswerDto datasurvey)
        {
            if (_context == null)
            {
                return ;
            }

            try
            {
                var check_user = await _context.Users.FindAsync(datasurvey.UserId);
                if (check_user == null)
                {
                    return;
                }

                var surveycheck = await _context.Surveys.FirstOrDefaultAsync(sa => sa.UserId == check_user.Id);
                if (surveycheck != null)
                {
                    return;
                }

                datasurvey.CreateAt = DateTime.Now;
                var new_survey = _mapper.Map<Models.Entities.Survey>(datasurvey);

                _context.Surveys.Add(new_survey);
                await _context.SaveChangesAsync();

                foreach (var id in datasurvey.AnswerIds)
                {
                    var is_answer = await _context.Answers.FindAsync(id) != null;
                    if (is_answer)
                    {
                        var survey_answer = new SurveyAnswer
                        {
                            SurveyId = new_survey.Id,
                            AnswerId = id
                        };
                        _context.SurveyAnswers.Add(survey_answer);
                    }
                }
                await _context.SaveChangesAsync();

                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        //public async Task UpdateSurvey(SurveyAnswerDto surveyobj)
        //{
        //    if (_context == null)
        //    {
        //        return ;
        //    }

        //    try
        //    {
        //        var survey = await _context.Surveys.FirstOrDefaultAsync(sa => sa.Id == surveyobj.SurveyId);
        //        if (survey == null)
        //        {
        //            return;
        //        }

        //        survey.Title = surveyobj.Title;
        //        survey.Description = surveyobj.Description;
        //        survey.CreateAt = DateTime.Now;
        //        _context.Surveys.Update(survey);

        //        var existingSurveyAnswers = await _context.SurveyAnswers.Where(sa => sa.SurveyId == surveyobj.SurveyId).ToListAsync();
        //        _context.SurveyAnswers.RemoveRange(existingSurveyAnswers);

        //        foreach (string answerid in surveyobj.AnswerIds)
        //        {
        //            var answer = await _context.Answers.FirstOrDefaultAsync(a => a.Id == answerid);
        //            if (answer != null)
        //            {
        //                var new_answer = new SurveyAnswer
        //                {
        //                    AnswerId = answerid,
        //                    SurveyId = surveyobj.SurveyId,
        //                };
        //                _context.SurveyAnswers.Add(new_answer);
        //            }
        //        }
        //        await _context.SaveChangesAsync();

        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        return;
        //    }
        //}

        //public async Task<ICollection<SurveyDto>> GetAll()
        //{
        //    if (_context == null)
        //    {
        //        return null!;
        //    }

        //    var surveys = await _context.Surveys
        //        .Include(s => s.SurveyAnswers)
        //        .Select(s => new SurveyDto
        //        {
        //            UserId = s.UserId,
        //            SurveyAnswers = s.SurveyAnswers.Select(sa => new GetSurvey
        //            {
        //                Question = sa.Answer.Question.Content,
        //                Answer = sa.Answer.AnswerText
        //            }).ToList()
        //        }).ToListAsync();

        //    return surveys;
        //}

        public async Task<SurveyDto> GetByUserId(string userId)
        {
            var survey = await _context.Surveys
                .Include(s => s.SurveyAnswers)
                    .ThenInclude(sa => sa.Answer)
                        .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (survey == null)
            {
                return null;
            }

            var surveyDto = new SurveyDto
            {
                UserId = survey.UserId,
                SurveyAnswers = survey.SurveyAnswers.Select(sa => new GetSurvey
                {
                    Question = sa.Answer.Question.Content,
                    Answer = sa.Answer.AnswerText
                }).ToList()
            };

            return surveyDto;
        }
    }
}
