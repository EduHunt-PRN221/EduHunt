using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace Eduhunt.Infrastructures.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;
        private readonly IConfiguration _configuration;

        public TokenValidationMiddleware(RequestDelegate next, IAmazonCognitoIdentityProvider cognitoProvider, IConfiguration configuration)
        {
            _next = next;
            _cognitoProvider = cognitoProvider;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var request = new GetUserRequest
                    {
                        AccessToken = accessToken
                    };

                    await _cognitoProvider.GetUserAsync(request);
                }
                catch (NotAuthorizedException)
                {
                    ClearAuthCookies(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error validating token: {ex.Message}");
                }
            }

            await _next(context);
        }

        private void ClearAuthCookies(HttpContext context)
        {
            context.Response.Cookies.Delete("IdToken");
            context.Response.Cookies.Delete("AccessToken");
            context.Response.Cookies.Delete("RefreshToken");
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}