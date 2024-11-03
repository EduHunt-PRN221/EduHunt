using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.ProfileService;
using Eduhunt.DTOs;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        public List<UserDto> UserProfile = new List<UserDto>();
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public string UserId { get; set; } = default!;

        public IndexModel(IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            var ProfileService = _serviceProvider.GetService<ProfileService>();
            var UserService = _serviceProvider.GetService<ApplicationUserService>();
            var UserProfiles = await UserService.GetAllUser();

            foreach (var user in UserProfiles)
            {
                if (user != null)
                {
                    var role = await _userManager.GetRolesAsync(user);
                    if(role.FirstOrDefault() != "Admin" && user.IsNotDeleted != false)
                    {
                        UserProfile.Add(new UserDto()
                        {
                            Email = user.Email,
                            Id = user.Id,
                            Name = user.UserName,
                            Role = role.FirstOrDefault()
                        });
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var UserService = _serviceProvider.GetService<ApplicationUserService>();
            await UserService.DeleteAsync(UserId);
            return RedirectToPage("");
        }
    }
}
