using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
namespace Eduhunt.Applications.ApplicactionUsers
{
    public class ApplicationUserService : Repository<ApplicationUser>
    {
        public ApplicationUserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor) :
                base(
                    context,
                    httpContextAccessor)
        {
        }
    }
}
