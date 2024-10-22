using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Areas.Identity.Pages
{
    public class LoginModel : PageModel
    {
        private readonly CognitoUserPool _userPool;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        [BindProperty]
        public string Username { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = default!;

        public LoginModel(IConfiguration configuration, UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _userManager = userManager;
            _serviceProvider = serviceProvider;

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
            _serviceProvider = serviceProvider;
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
                var service =_serviceProvider.GetRequiredService<ProfileService>();

                if (authResponse.AuthenticationResult != null)
                {
                    var idToken = authResponse.AuthenticationResult.IdToken;
                    var accessToken = authResponse.AuthenticationResult.AccessToken;
                    var refreshToken = authResponse.AuthenticationResult.RefreshToken;
                    var userEmail = service.GetEmailFromToken(idToken);

                    void AddCookie(string key, string value)
                    {
                        HttpContext.Response.Cookies.Append(key, value, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });
                    }

                    AddCookie("IdToken", idToken);
                    AddCookie("AccessToken", accessToken);
                    AddCookie("RefreshToken", refreshToken);

                    var applicationUser = await _userManager.FindByEmailAsync(userEmail);

                    if (applicationUser != null)
                    {
                        AddCookie("email", userEmail);
                        return LocalRedirect("/Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return Page();
            }
        }
    }
}
