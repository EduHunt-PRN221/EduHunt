using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Eduhunt.Applications.ApplicactionUsers;
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
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private async Task<IActionResult> HandlePasswordReset()
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                ModelState.AddModelError(nameof(NewPassword), "New password is required.");
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
                    ModelState.AddModelError(string.Empty, "Password reset failed.");
                    return Page();
                }
            }
            catch (InvalidPasswordException)
            {
                ModelState.AddModelError(nameof(NewPassword), "The password does not conform to policy.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private async Task<IActionResult> HandleRegistrationConfirmation()
        {
            if (string.IsNullOrEmpty(Username))
            {
                ModelState.AddModelError(nameof(Username), "Username is required for registration confirmation.");
                return Page();
            }

            if (string.IsNullOrEmpty(Role))
            {
                ModelState.AddModelError(nameof(Role), "Role is required for registration confirmation.");
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

                    ApplicationUser newUser = new() { Email = Emailuser, UserName = Username, EmailConfirmed = true };
                    await applicationUser.AddAsync(newUser);
                    var userId = newUser.Id;
                    await applicationUser.AddRoleForUserAsync(userId, Role);

                    return LocalRedirect($"/Identity/Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Confirmation failed.");
                    return Page();
                }
            }
            catch (CodeMismatchException)
            {
                ModelState.AddModelError(nameof(ConfirmationCode), "Invalid confirmation code.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}