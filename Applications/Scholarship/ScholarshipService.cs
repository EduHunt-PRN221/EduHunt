using Eduhunt.Data;
using Eduhunt.DTOs;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.Scholarships
{
    public class ScholarshipService : Repository<Scholarship>
    {
        public ScholarshipService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor) :
                base(
                    context,
                    httpContextAccessor)
        {
        }

        public async Task ApproveScholarship(Guid id, [FromBody] bool isApproved)
        {
            var scholarship = await _context.Scholarships.FindAsync(id);
            if (scholarship == null)
            {
                return;
            }

            if (isApproved)
            {
                scholarship.IsApproved = true;
                _context.Entry(scholarship).State = EntityState.Modified;
            }
            else
            {
                _context.Scholarships.Remove(scholarship);
            }
            await _context.SaveChangesAsync();

            return;
        }

        //public async Task<IEnumerable<Models.Entities.Scholarship>> GetScholarshipInfo()
        //{
        //    return await _context.Scholarships.ToListAsync();
        //}

        //public async Task<Models.Entities.Scholarship> GetScholarshipInfoById(Guid id)
        //{
        //    return await _context.Scholarships.FindAsync(id);
        //}

        public async Task<IEnumerable<UserScholarship>> GetUserScholarshipInfo()
        {
            return await _context.UserScholarships.ToListAsync();
        }

        public async Task<IEnumerable<ScholarshipInfoDto>> GetUserScholarshipInfoByUserId(string userId)
        {
            var userScholarships = await _context.UserScholarships
                .Where(us => us.UserId == userId)
                .Join(_context.Scholarships,
                      us => us.ScholarshipId,
                      si => si.Id,
                      (us, si) => new { UserScholarship = us, ScholarshipInfo = si })
                .ToListAsync();

            var scholarshipDtos = userScholarships.Select(us => new ScholarshipInfoDto
            {
                Id = us.ScholarshipInfo.Id,
                Title = us.ScholarshipInfo.Title ?? "Undefined Title",
                Budget = us.ScholarshipInfo.Budget ?? "Undefined Budget",
                Location = us.ScholarshipInfo.Location ?? "Undefined Location",
                School_name = us.ScholarshipInfo.SchoolName ?? "Undefined SchoolName",
                Level = us.ScholarshipInfo.Level ?? "Undefined Level",
                Url = us.ScholarshipInfo.Url ?? "Undefined Url"
            });

            return scholarshipDtos;
        }

        public async Task PostScholarshipInfo(ScholarshipDto scholarshipInfo)
        {
            scholarshipInfo.Id = Guid.NewGuid().ToString();
            scholarshipInfo.IsInSite = true;
            var scholarship = new Scholarship
            {
                Id = scholarshipInfo.Id,
                Title = scholarshipInfo.Title,
                Budget = scholarshipInfo.Budget,
                Location = scholarshipInfo.Location,
                SchoolName = scholarshipInfo.SchoolName,
                Level = scholarshipInfo.Level,
                Url = scholarshipInfo.Url,
                IsApproved = false,
                IsInSite = scholarshipInfo.IsInSite,
                ImageUrl = scholarshipInfo.ImageUrl,
                Description = scholarshipInfo.Description,
                CreatedAt = DateTime.UtcNow,
                AuthorId = scholarshipInfo.AuthorId!,
                ScholarshipCategories = [],
            };
            _context.Scholarships.Add(scholarship);

            await _context.SaveChangesAsync();

            return;
        }

        //get scholarship by userid
        public async Task<IEnumerable<Scholarship>> GetScholarshipByUserId(string userId)
        {
            var questionIds = _context.Questions
                .Where(q => q.IsNotDeleted)
                .Select(q => q.Id)
                .ToList();

            //get list survey id by userid
            var surveys = _context.Surveys
                .Where(survey => survey.UserId == userId && survey.IsNotDeleted)
                .Select(survey => survey.Id)
                .ToList();
            if (surveys.Count == 0)
            {
                //return top 4 scholarships
                return await _context.Scholarships
                    .Where(scholarship => scholarship.IsApproved && scholarship.IsInSite)
                    .OrderBy(scholarship => scholarship.Budget)
                    .Take(4)
                    .ToListAsync();
            }
            //get survey answers by surveyid
            var surveyAnswers = _context.SurveyAnswers
                 .Where(surveyAnswer => surveys.Contains(surveyAnswer.SurveyId) && surveyAnswer.IsNotDeleted)
                 .Select(surveyAnswer => new
                 {
                     surveyAnswer.SurveyId,
                     surveyAnswer.AnswerId
                 })
                 .ToList();
            //get answers by surveyanswers
            var answers = _context.Answers
                .Where(answer => surveyAnswers.Select(sa => sa.AnswerId).Contains(answer.Id)
                                && questionIds.Contains(answer.QuestionId)
                                && answer.IsNotDeleted)
                .Select(answer => answer.AnswerText)
                .ToList();

            //get scholarship by answers
            var lowerCaseAnswers = answers
                .Select(a => a!.ToLower())
                .ToList();


            var scholarships = await _context.Scholarships
                .Where(scholarship =>
                    (scholarship.Level != null && lowerCaseAnswers.Contains(scholarship.Level.ToLower())
                     || (scholarship.Budget != null && lowerCaseAnswers.Contains(scholarship.Budget.ToLower()))
                     || (scholarship.Location != null && lowerCaseAnswers.Contains(scholarship.Location.ToLower()))
                     || (scholarship.SchoolName != null && lowerCaseAnswers.Contains(scholarship.SchoolName.ToLower())))
                     && scholarship.IsApproved
                     && scholarship.IsInSite)
                .ToListAsync();

            return scholarships;

        }

        //public async Task PutScholarshipInfo(string id, Models.Entities.Scholarship scholarshipInfo)
        //{
        //    if (id != scholarshipInfo.Id)
        //    {
        //        return;
        //    }

        //    _context.Entry(scholarshipInfo).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ScholarshipInfoExists(id))
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return;
        //}

        private bool ScholarshipInfoExists(string id)
        {
            return _context.Scholarships.Any(e => e.Id == id);
        }

    }
}
