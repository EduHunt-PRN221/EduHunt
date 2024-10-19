using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.ProfileService;
using System.IdentityModel.Tokens.Jwt;

namespace Eduhunt.Infrastructures.Common
{
    public class CommonService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public CommonService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetUserId()
        {
            var profileService = _serviceProvider.GetService<ProfileService>();
            var userService = _serviceProvider.GetService<ApplicationUserService>();
            var httpContext = _httpContextAccessor.HttpContext;
            string userId = "";
            if (httpContext != null && profileService != null && userService != null)
            {
                if (httpContext.Request.Cookies.TryGetValue("IdToken", out var idToken))
                {
                    string email = profileService.GetEmailFromToken(idToken);
                    userId = await userService.GetNewestIdByEmailAsync(email);
                }
            }
            return userId;
        }

        public bool IsUserLoggedIn()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext != null)
            {
                if (httpContext.Request.Cookies.TryGetValue("IdToken", out var idToken))
                {
                    // Validate the token (e.g., decode, check expiry)
                    var isTokenValid = ValidateToken(idToken);

                    return isTokenValid;
                }
            }
            return false;
        }

        private bool ValidateToken(string token)
        {

            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var expiry = jwtToken.ValidTo;

                // Check if the token is expired
                if (expiry > DateTime.UtcNow)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
