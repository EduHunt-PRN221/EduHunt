using Eduhunt.Data;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
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
            RoleManager<IdentityRole> roleManager) :
                base(context, httpContextAccessor)
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
    }
}
