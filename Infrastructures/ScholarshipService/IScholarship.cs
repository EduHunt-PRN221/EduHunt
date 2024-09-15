using Eduhunt.DTOs;
using EDUHUNT_BE.Models;

namespace Eduhunt.Infrastructures.Repositories
{
    public interface IScholarship
    {
        Task<List<ScholarshipDto>> GetScholarships();
        Task AddScholarship(ScholarshipDto scholarship);
        Task<List<Scholarship>> GetRecommendedScholarships(string userId);
    }
}
