using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace Eduhunt.Areas.Admin.Pages
{
    public class ManageCertificateModel : PageModel
    {
        public List<Profile> UserProfile = new List<Profile>();
        private readonly IServiceProvider _serviceProvider;
        [BindProperty]
        public string UserId { get; set; } = default!;

        public ManageCertificateModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnGet()
        {
            var ProfileServices = _serviceProvider.GetService<ProfileService>();
            var UserProfiles = ProfileServices.GetAll();
            foreach (var user in UserProfiles)
            {
                if (user.CertificateImage != null)
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

        public async Task<IActionResult> OnPostDenyAsync()
        {
            var ProfileService = _serviceProvider.GetService<ProfileService>();
            var UpdateUser = ProfileService.GetAll().Where(x => x.Id == UserId).FirstOrDefault();
            UpdateUser.CertificateImage = null;
            await ProfileService.UpdateAsync(UpdateUser);
            return RedirectToPage("");
        }
    }
}
