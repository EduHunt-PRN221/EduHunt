using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Applications.ApplicactionUsers
{
    public class ApplicationUserService : Repository<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper) :
                base(context, httpContextAccessor, mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<SelectListItem>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync();
            return roles;
        }

        // Method to add a role to a user
        public async Task<bool> AddRoleForUserAsync(string userId, string roleName)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle case where user is not found
                return false;
            }

            // Check if the role exists, if not, create it
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                if (!roleResult.Succeeded)
                {
                    // Handle case where role creation failed
                    return false;
                }
            }

            // Add the user to the role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        // Method to get newest userId by email
        public async Task<string> GetNewestIdByEmailAsync(string email)
        {
            var userId = await _context.Users
            .Where(u => u.Email == email)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

            return userId!;
        }

        public async Task<bool> IsMentorAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                return role.Contains("Mentor");
            }
            return false;
        }

        public async Task<bool> GetVIPStatusByEmailAsync(string email)
        {
            bool? status = await _context.Users
                                 .Where(u => u.Email == email)
                                 .Select(u => u.IsVIP)
                                 .FirstOrDefaultAsync();

            return status ?? false;
        }

        // update user's VIP status
        public async Task UpdateVIPStatusByEmailAsync(string email, bool status)
        {
            var user = await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.IsVIP = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
