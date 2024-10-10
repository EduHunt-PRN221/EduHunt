using Eduhunt.DTOs;
using Eduhunt.Infrastructures.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.Scholarship
{
    public class ScholarshipService : Repository<Models.Entities.Scholarship>
    {
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

        public async Task DeleteScholarshipInfo(Guid id)
        {
            var scholarshipInfo = await _context.Scholarships.FindAsync(id);
            if (scholarshipInfo == null)
            {
                return;
            }

            _context.Scholarships.Remove(scholarshipInfo);
            await _context.SaveChangesAsync();

            return ;
        }

        public async Task<IEnumerable<Models.Entities.Scholarship>> GetScholarshipInfo()
        {
            return await _context.Scholarships.ToListAsync();
        }

        public async Task<Models.Entities.Scholarship> GetScholarshipInfoById(Guid id)
        {
            return await _context.Scholarships.FindAsync(id);
        }

        public async Task<IEnumerable<Models.Entities.UserScholarship>> GetUserScholarshipInfo()
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
                Title = us.ScholarshipInfo.Title,
                Budget = us.ScholarshipInfo.Budget,
                Location = us.ScholarshipInfo.Location,
                School_name = us.ScholarshipInfo.SchoolName,
                Level = us.ScholarshipInfo.Level,
                Url = us.ScholarshipInfo.Url
            });

            return scholarshipDtos;
        }

        public async Task PostScholarshipInfo(ScholarshipDto scholarshipInfo)
        {
            scholarshipInfo.Id = Guid.NewGuid().ToString();
            scholarshipInfo.IsInSite = true;
            var scholarship = new Models.Entities.Scholarship
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
                AuthorId = scholarshipInfo.AuthorId,
                ScholarshipCategories = [],
            };
            _context.Scholarships.Add(scholarship);

            await _context.SaveChangesAsync();

            return;
        }

        public async Task PutScholarshipInfo(string id, Models.Entities.Scholarship scholarshipInfo)
        {
            if (id != scholarshipInfo.Id)
            {
                return;
            }

            _context.Entry(scholarshipInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScholarshipInfoExists(id))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }

            return;
        }

        private bool ScholarshipInfoExists(string id)
        {
            return _context.Scholarships.Any(e => e.Id == id);
        }

    }
}
