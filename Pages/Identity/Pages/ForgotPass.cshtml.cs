using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Eduhunt.Pages.Identity.Pages
{
    public class ForgotPassModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Email { get; set; }

        public ForgotPassModel(IConfiguration configuration)
        {
            _configuration = configuration;

            // Retrieve configuration values from appsettings.json
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];
            var userPoolId = _configuration["AWS:UserPoolId"];
            var clientId = _configuration["AWS:ClientId"];

            // Initialize AmazonCognitoIdentityProviderClient with configuration
            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));

            // Initialize CognitoUserPool with UserPoolId and ClientId
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
                // Create Forgot Password request
                var forgotPasswordRequest = new ForgotPasswordRequest
                {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = Email
                };

                // Initiate the forgot password process
                var response = await _provider.ForgotPasswordAsync(forgotPasswordRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Redirect to a page where the user can input the confirmation code and new password
                    return RedirectToPage("/ConfirmForgotPassword", new { email = Email });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to send reset instructions.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions such as user not found or service issues
                ModelState.AddModelError(string.Empty, "An error occurred while trying to reset the password.");
                return Page();
            }
        }
    }
}
