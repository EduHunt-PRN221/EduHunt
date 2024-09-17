using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Areas.Identity.Pages
{
    public class ConfirmRegistrationModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Username { get; set; } = default!;

        [BindProperty]
        public string Emailuser { get; set; } = default!;

        [BindProperty]
        public string ConfirmationCode { get; set; } = default!;

        public ConfirmRegistrationModel(IConfiguration configuration)
        {
            _configuration = configuration;

            // Initialize AmazonCognitoIdentityProviderClient with configuration
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = _configuration["AWS:Region"];

            _provider = new AmazonCognitoIdentityProviderClient(
                accessKey,
                secretKey,
                RegionEndpoint.GetBySystemName(region));
        }

        public void OnGet(string username, string mail)
        {
            // Pre-fill the username if it's passed in the query string
            Username = username;
            Emailuser = mail;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Create the request to confirm the user sign-up
                var confirmSignUpRequest = new ConfirmSignUpRequest
                {
                    ClientId = _configuration["AWS:ClientId"],
                    Username = Emailuser,
                    ConfirmationCode = ConfirmationCode
                };

                // Confirm the registration with AWS Cognito
                var response = await _provider.ConfirmSignUpAsync(confirmSignUpRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Confirmation was successful; redirect to login or a welcome page
                    return RedirectToPage("/Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Confirmation failed.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle errors such as invalid confirmation code or network issues
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
