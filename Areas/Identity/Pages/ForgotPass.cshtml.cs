using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Areas.Identity.Pages
{
    public class ForgotPassModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public string Email { get; set; } = default!;

        public ForgotPassModel(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];
            var userPoolId = _configuration["AWS:UserPoolId"];
            var clientId = _configuration["AWS:ClientId"];

            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));

            _userPool = new CognitoUserPool(userPoolId, clientId, _provider);
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (_userManager.FindByEmailAsync(Email).Result == null)
                {
                    TempData["error"] = "User not found.";
                    return Page();
                }

                var forgotPasswordRequest = new ForgotPasswordRequest
                {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = Email
                };

                var response = await _provider.ForgotPasswordAsync(forgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return LocalRedirect($"/Identity/ConfirmRegistration?mail={Email}&isPasswordReset=true");
                }
                else
                {
                    TempData["error"] = "Failed to send reset instructions.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Page();
            }
        }
    }
}