using Eduhunt.DTOs;
using Eduhunt.Models.Entities;

namespace Eduhunt.Infrastructures.Repositories
{
    public interface IScholarship
    {
        Task<List<ScholarshipInfoDto>> GetScholarships();
        //Task AddScholarship(ScholarshipInfoDto scholarship);
        Task<List<Scholarship>> GetRecommendedScholarships(string userId);
    }
}
