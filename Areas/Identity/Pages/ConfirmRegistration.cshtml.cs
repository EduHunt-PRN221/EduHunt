using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Eduhunt.Areas.Identity.Pages
{
    public class ConfirmRegistrationModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Role { get; set; }

        [BindProperty]
        [Required]
        public string Emailuser { get; set; } = default!;

        [BindProperty]
        [Required]
        public string ConfirmationCode { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }

        [BindProperty]
        public bool IsPasswordReset { get; set; }

        public ConfirmRegistrationModel(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];

            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));
        }

        public void OnGet(string? username, string mail, string? role, bool isPasswordReset = false)
        {
            Username = username;
            Emailuser = mail;
            Role = role;
            IsPasswordReset = isPasswordReset;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (IsPasswordReset)
                {
                    return await HandlePasswordReset();
                }
                else
                {
                    return await HandleRegistrationConfirmation();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Page();
            }
        }

        private async Task<IActionResult> HandlePasswordReset()
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                TempData["error"] = "New password is required.";
                return Page();
            }

            var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest
            {
                ClientId = _configuration["AWS:ClientId"],
                Username = Emailuser,
                ConfirmationCode = ConfirmationCode,
                Password = NewPassword
            };

            try
            {
                var response = await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return LocalRedirect($"/Identity/Login");
                }
                else
                {
                    TempData["error"] = "Password reset failed.";
                    return Page();
                }
            }
            catch (InvalidPasswordException)
            {
                TempData["error"] = "The password does not conform to policy.";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Page();
            }
        }

        private async Task<IActionResult> HandleRegistrationConfirmation()
        {
            if (string.IsNullOrEmpty(Username))
            {
                TempData["error"] = "Username is required for registration confirmation.";
                return Page();
            }

            if (string.IsNullOrEmpty(Role))
            {
                TempData["error"] = "Role is required for registration confirmation.";
                return Page();
            }

            var confirmSignUpRequest = new ConfirmSignUpRequest
            {
                ClientId = _configuration["AWS:ClientId"],
                Username = Emailuser,
                ConfirmationCode = ConfirmationCode
            };

            try
            {
                var response = await _provider.ConfirmSignUpAsync(confirmSignUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var applicationUser = _serviceProvider.GetRequiredService<ApplicationUserService>();
                    var profileService = _serviceProvider.GetRequiredService<ProfileService>();
                    ApplicationUser newUser = new() { Email = Emailuser, UserName = Username, EmailConfirmed = true };
                    await applicationUser.AddAsync(newUser);
                    var userId = newUser.Id;
                    await applicationUser.AddRoleForUserAsync(userId, Role);
                    await profileService.AddAsync(new Models.Entities.Profile() { UserName = Username, Email = Emailuser, ApplicationUserId = userId, AvatarImage = "https://shorturl.at/cHJyG" });

                    return LocalRedirect($"/Identity/Login");
                }
                else
                {
                    TempData["error"] = "Confirmation failed.";
                    return Page();
                }
            }
            catch (CodeMismatchException)
            {
                TempData["error"] = "Invalid confirmation code.";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Page();
            }
        }
    }
}