using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Eduhunt.Areas.Identity.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;

        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public ChangePasswordModel(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];

            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accessToken = HttpContext.Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                ModelState.AddModelError(string.Empty, "User is not authenticated.");
                return Page();
            }

            var changePasswordRequest = new ChangePasswordRequest
            {
                AccessToken = accessToken,
                PreviousPassword = OldPassword,
                ProposedPassword = NewPassword
            };

            try
            {
                var response = await _provider.ChangePasswordAsync(changePasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return LocalRedirect("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password change failed.");
                    return Page();
                }
            }
            catch (InvalidPasswordException)
            {
                ModelState.AddModelError(nameof(NewPassword), "The new password does not conform to the password policy.");
                return Page();
            }
            catch (NotAuthorizedException)
            {
                ModelState.AddModelError(nameof(OldPassword), "The current password is incorrect.");
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