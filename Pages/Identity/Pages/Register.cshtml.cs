using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon;
using Microsoft.Extensions.Configuration;
using Amazon.CognitoIdentityProvider.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Eduhunt.Pages.Identity.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public RegisterModel(IConfiguration configuration)
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
                // Prepare the request to sign up the user
                var signUpRequest = new SignUpRequest
                {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = Email,
                    Password = Password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType
                        {
                            Name = "email",
                            Value = Email
                        }
                    }
                };

                // Register the user in Cognito
                var response = await _provider.SignUpAsync(signUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Registration was successful, redirect to a confirmation page or login page
                    return RedirectToPage("/Identity/Pages/ConfirmRegistration", new { username = Username,mail = Email });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Registration failed.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., user already exists)
                ModelState.AddModelError(string.Empty, "An error occurred during the registration process.");
                return Page();
            }
        }
    }
}
