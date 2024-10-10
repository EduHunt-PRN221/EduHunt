﻿using Eduhunt.Data;
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
