using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Infrastructures.Cloud;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Eduhunt.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public Profile? profile { get; set; }
        [BindProperty]
        public IFormFile? Upload { get; set; }
        [BindProperty]
        public bool? IsVIP { get; set; } = false;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProfileService _profileService;
        private readonly ApplicationUserService _userService;
        private readonly PaymentService _paymentService;

        public ProfileModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _profileService = _serviceProvider.GetRequiredService<ProfileService>();
            _userService = _serviceProvider.GetRequiredService<ApplicationUserService>();
            _paymentService = _serviceProvider.GetRequiredService<PaymentService>();


        }
        public async Task<IActionResult> OnGet(string status, string email)
        {

            // Get the user ID from the cookie

            var idToken = _userService.GetIdTokenFromCookie(HttpContext);
            if (idToken == null)
            {
                return RedirectToPage("/Index");
            }
            var userEmail = _userService.GetUserEmailFromIdToken(idToken);
            if (userEmail == null)
            {
                userEmail = email;
            }

            if (status == "PAID")
            {
                await _userService.UpdateVIPStatusByEmailAsync(userEmail, true);
            }

            // get profile by user id
            profile = await _profileService.GetProfileByUserEmailAsync(userEmail);
            IsVIP = await _userService.GetVIPStatusByEmailAsync(userEmail);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var idToken = _userService.GetIdTokenFromCookie(HttpContext);
            if (idToken == null)
            {
                return RedirectToPage("/Index");
            }
            var userEmail = _userService.GetUserEmailFromIdToken(idToken);
            var profileDb = await _profileService.GetProfileByUserEmailAsync(userEmail);
            if (profileDb == null)
            {
                return RedirectToPage("/Index");
            }

            var _cloudinaryService = _serviceProvider.GetRequiredService<CloudinaryService>();

            string? url = null;

            if (Upload != null)
            {
                url = await _cloudinaryService.UploadSingleAsync(Upload);
            }

            if (url != null)
            {
                profile!.AvatarImage = url;
            }
            else
            {
                profile!.AvatarImage = profileDb.AvatarImage;

            }
            //validation
            if (profile.FirstName != null && profile.FirstName.Any(char.IsDigit))
            {
                ModelState.AddModelError("profile.FirstName", "First name cannot contain numbers");
            }
            if (profile.LastName != null && profile.LastName.Any(char.IsDigit))
            {
                ModelState.AddModelError("profile.LastName", "Last name cannot contain numbers");
            }
            if (profile.UserName != null && profile.UserName.Any(char.IsWhiteSpace))
            {
                ModelState.AddModelError("profile.UserName", "Username cannot contain spaces");
            }
            // Update the PhoneNumber validation to allow null values
            if (profile.PhoneNumber != null && profile.PhoneNumber.Any(char.IsLetter))
            {
                ModelState.AddModelError("profile.PhoneNumber", "Phone number cannot contain letters");
            }

            // check email format 
            if (!new EmailAddressAttribute().IsValid(profile.Email))
            {
                ModelState.AddModelError("profile.Email", "Invalid email format");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            //update profile
            profileDb!.FirstName = profile!.FirstName;
            profileDb!.LastName = profile!.LastName;
            profileDb!.UserName = profile!.UserName;
            profileDb!.Email = profile!.Email;
            profileDb!.PhoneNumber = profile!.PhoneNumber;
            profileDb!.Country = profile!.Country;
            profileDb!.City = profile!.City;
            profileDb!.Title = profile!.Title;

            if (profile!.AvatarImage != null)
            {
                profileDb!.AvatarImage = profile!.AvatarImage;

            }
            //trim the string
            profileDb!.FirstName = profileDb!.FirstName != null ? profileDb!.FirstName.Trim() : null;
            profileDb!.LastName = profileDb!.LastName != null ? profileDb!.LastName.Trim() : null;
            profileDb!.UserName = profileDb!.UserName != null ? profileDb!.UserName.Trim() : null;
            profileDb!.Email = profileDb!.Email != null ? profileDb!.Email.Trim() : null;
            profileDb!.PhoneNumber = profileDb!.PhoneNumber != null ? profileDb!.PhoneNumber.Trim() : null;
            profileDb!.Country = profileDb!.Country != null ? profileDb!.Country.Trim() : null;
            profileDb!.City = profileDb!.City != null ? profileDb!.City.Trim() : null;

            await _profileService.UpdateAsync(profileDb);
            return RedirectToPage("/Profile");
        }


        public async Task<IActionResult> OnPostPayVIP()
        {
            var idToken = _userService.GetIdTokenFromCookie(HttpContext);
            if (idToken == null)
            {
                return RedirectToPage("/Index");
            }
            var userEmail = _userService.GetUserEmailFromIdToken(idToken);
            var encodedEmail = HttpUtility.UrlEncode(userEmail);
            var cancelURL = $"https://localhost:7099/Profile?email={encodedEmail}";
            var successURL = $"https://localhost:7099/Profile?email={encodedEmail}";

            var returnUrl = await _paymentService.PaymentProcessing(cancelURL, successURL, "VIP", 1, 4000);
            return Redirect(returnUrl);
        }
    }
}
