using Eduhunt.Applications.ProfileService;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Areas.Admin
{
    public class ApproveCertificateModel : PageModel
    {
        public List<Profile> UserProfile = new List<Profile>();
        private readonly IServiceProvider _serviceProvider;
        [BindProperty]
        public string UserId { get; set; } = default!;

        public ApproveCertificateModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnGet()
        {
            var ProfileService = _serviceProvider.GetService<ProfileService>();
            var UserProfiles = ProfileService.GetAll();
            foreach (var user in UserProfiles)
            {
                if(user.CertificateImage != null && user.IsApprove != true)
                {
                    UserProfile.Add(user);
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ProfileService = _serviceProvider.GetService<ProfileService>();
            var UpdateUser = ProfileService.GetAll().Where(x => x.Id == UserId).FirstOrDefault();
            UpdateUser.IsApprove = true;
            await ProfileService.UpdateAsync(UpdateUser);
            return RedirectToPage("");
        }
    }
}
