using Eduhunt.Models.Entities;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Data;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.Questions
{
    public class QuestionService : Repository<Question>
    {
        public QuestionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        public IQueryable<Question> GetAllQuestionOption()
        {
            IQueryable<Question> newList = _context.Questions.Include(x => x.Answers);
            return newList;
        }
    }
}
