using AutoMapper;
using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;

namespace Eduhunt.Applications.Cvs
{
    public class CvService : Repository<Cv>
    {
        public CvService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor) :
                base(
                    context,
                    httpContextAccessor)
        {
        }
    }
}
