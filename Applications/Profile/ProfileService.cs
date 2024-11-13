using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Eduhunt.Applications.ProfileService
{
    public class ProfileService : Repository<Profile>
    {

        public ProfileService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor) :
                base(
                    context,
                    httpContextAccessor)
        {
        }

        public string GetEmailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            return jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value ?? string.Empty;
        }

        public async Task<bool> IsUploadAsync(string email)
        {
            var upload = await _context.Profile.Where(x=>x.Email == email).Select(x=>x.CertificateImage).SingleOrDefaultAsync();
            if (upload != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsApprovedAsync(string email)
        {
            var approved = await _context.Profile.Where(x => x.Email == email).Select(x => x.IsApprove).SingleOrDefaultAsync();
            return approved;
        }

        //get profile by user id
        public async Task<Profile?> GetProfileByUserIdAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var entity = await _context.Profile.FirstOrDefaultAsync(x => x.ApplicationUserId == userId);

            return entity;
        }

        public async Task<Profile?> GetProfileByUserEmailAsync(string? userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            var entity = await _context.Profile.FirstOrDefaultAsync(x => x.Email == userEmail);

            return entity;
        }

        //get profile by user id
        public async Task<Profile?> GetProfileByUserNameAndEmailAsync(string username, string email)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
            {
                return null;
            }

            var entity = await _context.Profile.FirstOrDefaultAsync(x => (x.UserName == username) && (x.Email == email));

            return entity;
        }
    }
}
