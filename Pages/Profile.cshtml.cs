using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Infrastructures.Cloud;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Web;

namespace Eduhunt.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public Profile Profile { get; set; } = new Profile();

        [BindProperty]
        public IFormFile? UploadAvatar { get; set; }

        [BindProperty]
        public IFormFile? UploadCertificate { get; set; }

        public bool IsVIP { get; set; }
        public bool IsMentor { get; set; }

        private readonly ProfileService _profileService;
        private readonly ApplicationUserService _userService;
        private readonly PaymentService _paymentService;
        private readonly CloudinaryService _cloudinaryService;

        public ProfileModel(ProfileService profileService, ApplicationUserService userService,
                            PaymentService paymentService, CloudinaryService cloudinaryService)
        {
            _profileService = profileService;
            _userService = userService;
            _paymentService = paymentService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IActionResult> OnGetAsync(string? status, string? email)
        {
            var userEmail = GetUserEmailFromToken() ?? email;
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToPage("/Identity/Login");

            if (status == "PAID")
            {

                await _userService.UpdateVIPStatusByEmailAsync(userEmail, true);
                TempData["success"] = "Paid VIP successfully";

            }

            await LoadUserProfile(userEmail);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userEmail = GetUserEmailFromToken();
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToPage("/Identity/Login");

            if (!ModelState.IsValid)
                return Page();

            var profileDb = await _profileService.GetProfileByUserEmailAsync(userEmail);
            if (profileDb == null)
                return NotFound("Profile not found");

            await UpdateProfileImages(profileDb);
            UpdateProfileProperties(profileDb);

            await _profileService.UpdateAsync(profileDb);
            TempData["success"] = "Profile Updated successfully.";
            return RedirectToPage("/Profile");
        }

        public async Task<IActionResult> OnPostPayVIPAsync()
        {
            var userEmail = GetUserEmailFromToken();
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToPage("/Identity/Login");

            var encodedEmail = HttpUtility.UrlEncode(userEmail);
            var baseUrl = $"https://localhost:7099/Profile?email={encodedEmail}";
            var returnUrl = await _paymentService.PaymentProcessing(
                $"{baseUrl}",
                $"{baseUrl}&status=PAID",
                "VIP", 1, 4000);

            return Redirect(returnUrl);
        }

        private string? GetUserEmailFromToken()
        {
            if (HttpContext.Request.Cookies.TryGetValue("IdToken", out var idToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(idToken) as JwtSecurityToken;
                return jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            }
            return null;
        }

        private async Task LoadUserProfile(string userEmail)
        {
            Profile = await _profileService.GetProfileByUserEmailAsync(userEmail) ?? new Profile();
            IsVIP = await _userService.GetVIPStatusByEmailAsync(userEmail);
            IsMentor = await _userService.IsMentorAsync(userEmail);
        }

        private async Task UpdateProfileImages(Profile profileDb)
        {
            Profile.AvatarImage = UploadAvatar != null
                ? await _cloudinaryService.UploadSingleAsync(UploadAvatar)
                : profileDb.AvatarImage;

            Profile.CertificateImage = UploadCertificate != null
                ? await _cloudinaryService.UploadSingleAsync(UploadCertificate)
                : profileDb.CertificateImage;
        }

        private void UpdateProfileProperties(Profile profileDb)
        {
            profileDb.FirstName = Profile.FirstName?.Trim();
            profileDb.LastName = Profile.LastName?.Trim();
            profileDb.UserName = Profile.UserName?.Trim();
            profileDb.Email = Profile.Email?.Trim();
            profileDb.PhoneNumber = Profile.PhoneNumber?.Trim();
            profileDb.Country = Profile.Country?.Trim();
            profileDb.City = Profile.City?.Trim();
            profileDb.Title = Profile.Title?.Trim();
            profileDb.AvatarImage = Profile.AvatarImage;
            profileDb.CertificateImage = Profile.CertificateImage;
        }
    }
}