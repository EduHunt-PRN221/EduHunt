using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;

namespace Eduhunt.Applications.Answers
{
    public class AnswerService : Repository<Answer>
    {
        public AnswerService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
        }
    }
}
