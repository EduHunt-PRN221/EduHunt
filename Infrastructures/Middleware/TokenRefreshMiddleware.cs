using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.IdentityModel.Tokens.Jwt;
namespace Eduhunt.Infrastructures.Middleware
{

    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;
        private readonly IConfiguration _configuration;

        public TokenRefreshMiddleware(RequestDelegate next, IAmazonCognitoIdentityProvider cognitoProvider, IConfiguration configuration)
        {
            _next = next;
            _cognitoProvider = cognitoProvider;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var idToken = context.Request.Cookies["IdToken"];
            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(idToken) as JwtSecurityToken;

                if (jsonToken != null && jsonToken.ValidTo < DateTime.UtcNow)
                {
                    var refreshedTokens = await RefreshTokensAsync(refreshToken);

                    if (refreshedTokens != null)
                    {
                        void AddCookie(string key, string value)
                        {
                            context.Response.Cookies.Append(key, value, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddHours(1)
                            });
                        }

                        AddCookie("IdToken", idToken);
                        AddCookie("AccessToken", accessToken);
                    }
                    else
                    {
                        context.Response.Cookies.Delete("IdToken");
                        context.Response.Cookies.Delete("AccessToken");
                        context.Response.Cookies.Delete("RefreshToken");
                    }
                }
            }

            await _next(context);
        }

        private async Task<AuthenticationResultType> RefreshTokensAsync(string refreshToken)
        {
            try
            {
                var request = new InitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                    ClientId = _configuration["AWS:ClientId"],
                    AuthParameters = new Dictionary<string, string>
                    {
                        { "REFRESH_TOKEN", refreshToken }
                    }
                };

                var response = await _cognitoProvider.InitiateAuthAsync(request);

                return response.AuthenticationResult;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static class TokenRefreshMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRefreshMiddleware>();
        }
    }
}

