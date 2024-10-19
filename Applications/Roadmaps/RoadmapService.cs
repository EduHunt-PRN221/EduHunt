using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.Roadmaps
{
    public class RoadmapService : Repository<Roadmap>
    {
        public RoadmapService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor) :
                base(
                    context,
                    httpContextAccessor)
        {
        }

        public async Task<Roadmap?> GetByUserIdAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var entity = await _context.Roadmaps.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);

            return entity;
        }
    }
}
