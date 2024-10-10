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
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) :
                base(
                    context,
                    httpContextAccessor,
                    mapper)
        {
        }
    }
}
