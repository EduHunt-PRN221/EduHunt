#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using Amazon.Extensions.CognitoAuthentication;
using CloudinaryDotNet;
using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Data;
using Eduhunt.Infrastructures.Cloud;
using Eduhunt.Infrastructures.Repositories;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web;

namespace Eduhunt.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public Profile? profile { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; }
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

            var userEmail = GetUserEmailFromCookie();
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
            var _cloudinaryService = _serviceProvider.GetRequiredService<CloudinaryService>();
            var url = await _cloudinaryService.UploadSingleAsync(Upload);

            if (url != null)
            {
                profile!.AvatarImage = url;

            }
            //validation
            if (profile.FirstName.Any(char.IsDigit))
            {
                ModelState.AddModelError(profile.FirstName, "First name cannot contain numbers");
            }
            if (profile.LastName.Any(char.IsDigit))
            {
                ModelState.AddModelError(profile.LastName, "Last name cannot contain numbers");
            }
            if (profile.UserName.Any(char.IsWhiteSpace))
            {
                ModelState.AddModelError(profile.UserName, "Username cannot contain spaces");
            }
            if (profile.PhoneNumber.Any(char.IsLetter))
            {
                ModelState.AddModelError(profile.PhoneNumber, "Phone number cannot contain letters");
            }
            // check email format 
            if (!new EmailAddressAttribute().IsValid(profile.Email))
            {
                ModelState.AddModelError(profile.Email, "Invalid email format");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            var userEmail = GetUserEmailFromCookie();
            var profileDb = await _profileService.GetProfileByUserEmailAsync(userEmail);
            //update profile
            profileDb!.FirstName = profile!.FirstName;
            profileDb!.LastName = profile!.LastName;
            profileDb!.UserName = profile!.UserName.Trim();
            profileDb!.Email = profile!.Email.Trim();
            profileDb!.PhoneNumber = profile!.PhoneNumber;
            profileDb!.Country = profile!.Country;
            profileDb!.City = profile!.City;
            profileDb!.Title = profile!.Title;
            profileDb!.AvatarImage = profile!.AvatarImage;

            await _profileService.UpdateAsync(profileDb);
            return Page();
        }

        public string GetUserEmailFromCookie()
        {
            if (HttpContext.Request.Cookies.TryGetValue("userEmail", out var userEmail))
            {
                return userEmail;
            }

            return null;
        }

        public async Task<IActionResult> OnPostPayVIP()
        {
            var userEmail = GetUserEmailFromCookie();
            var encodedEmail = HttpUtility.UrlEncode(userEmail);
            var cancelURL = $"https://localhost:7099/Profile?email={encodedEmail}";
            var successURL = $"https://localhost:7099/Profile?email={encodedEmail}";

            var returnUrl = await _paymentService.PaymentProcessing(cancelURL, successURL, "VIP", 1, 4000);
            return Redirect(returnUrl);
        }
    }
}
