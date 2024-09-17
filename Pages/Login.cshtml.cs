using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentityProvider;
using Amazon;
using Microsoft.Extensions.Configuration;

namespace Eduhunt.Pages
{
    public class LoginModel : PageModel
    {
        private readonly CognitoUserPool _userPool;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IConfiguration configuration)
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
            var user = new CognitoUser(Username, _configuration["AWS:ClientId"], _userPool, _provider);

            var authRequest = new InitiateSrpAuthRequest
            {
                Password = Password
            };

            try
            {
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                if (authResponse.AuthenticationResult != null)
                {
                    // Handle successful login (e.g., set cookies, redirect)
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., incorrect login details)
                ModelState.AddModelError(string.Empty, "An error occurred during the login process.");
                return Page();
            }
        }
    }
}
