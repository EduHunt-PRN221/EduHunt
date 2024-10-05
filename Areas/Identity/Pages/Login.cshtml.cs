using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;

namespace Eduhunt.Areas.Identity.Pages
{
    public class LoginModel : PageModel
    {
        private readonly CognitoUserPool _userPool;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public string Username { get; set; } = default!;  // This should be email used as username

        [BindProperty]
        public string Password { get; set; } = default!;

        public LoginModel(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

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
            // Here, we assume Username is the email, which Cognito can treat as the username
            var user = new CognitoUser(Username, _configuration["AWS:ClientId"], _userPool, _provider);

            var authRequest = new InitiateSrpAuthRequest
            {
                Password = Password
            };

            try
            {
                // Start the authentication process using SRP (Secure Remote Password) protocol
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                if (authResponse.AuthenticationResult != null)
                {

                    var accessToken = authResponse.AuthenticationResult.AccessToken;
                    var idToken = authResponse.AuthenticationResult.IdToken;
                    var userEmail = GetUserEmailFromAccessToken(idToken);
                    // set the userId in a cookie
                    HttpContext.Response.Cookies.Append("userEmail", userEmail, new CookieOptions
                    {
                        HttpOnly = true, 
                        Secure = true,  
                        SameSite = SameSiteMode.Strict, 
                        Expires = DateTimeOffset.UtcNow.AddDays(30) 
                    });


                    // Retrieve the ApplicationUser using UserManager
                    var applicationUser = await _userManager.FindByEmailAsync(Username);

                    if (applicationUser != null)
                    {
                        var userId = applicationUser.Id;

                        // You can now use the userId as needed
                        // For example, you could add it to the query string:
                        return LocalRedirect($"/Index?userId={userId}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found in local database.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., incorrect login details, network issues)
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private string GetUserEmailFromAccessToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return string.Empty;
            }

            var email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            return email ?? string.Empty;
        }
    }
}
