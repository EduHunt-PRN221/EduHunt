using Eduhunt.DTOs;
using Eduhunt.Models;
using Microsoft.AspNetCore.Mvc;
using static Eduhunt.DTOs.ServiceResponses;

namespace Eduhunt.Interfaces
{
    public interface IScholarshipInfoes
    {
        public Task<IEnumerable<ScholarshipInfo>> GetScholarshipInfo();
        public Task<IEnumerable<UserScholarship>> GetUserScholarshipInfo();
        public Task<IEnumerable<ScholarshipDto>> GetUserScholarshipInfoByUserId(string userId);
        public Task<ScholarshipInfo> GetScholarshipInfoById(Guid id);
        public Task<GeneralResponse> PutScholarshipInfo(Guid id, ScholarshipInfo scholarshipInfo);
        public Task<GeneralResponse> PostScholarshipInfo(ScholarshipInfoDto scholarshipInfo);
        public Task<GeneralResponse> DeleteScholarshipInfo(Guid id);
        public Task<GeneralResponse> ApproveScholarship(Guid id, [FromBody] bool isApproved);
    }
}
