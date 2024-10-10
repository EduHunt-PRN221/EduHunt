using AutoMapper;
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
                AuthorId = scholarshipInfo.AuthorId,
                ScholarshipCategories = [],
            };
            _context.Scholarships.Add(scholarship);

            await _context.SaveChangesAsync();

            return;
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
