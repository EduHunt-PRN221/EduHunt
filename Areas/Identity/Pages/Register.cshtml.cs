﻿
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Eduhunt.Applications.ApplicactionUsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eduhunt.Areas.Identity.Pages
{
    // create constructor have IServiceProvider
    // create constructor have IConfiguration
    // create constructor have CognitoUserPool



    public class RegisterModel : PageModel
    {
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        [BindProperty]
        public string Username { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = default!;

        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public string Role { get; set; } = default!;

        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>(); // Initialize RoleList with an empty list

        public RegisterModel(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
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
        }

        public async Task OnGetAsync()
        {
            var service= _serviceProvider.GetRequiredService<ApplicationUserService>();
            RoleList = await service.GetAllRolesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var UserService = _serviceProvider.GetRequiredService<ApplicationUserService>();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // checkuser is esxit if esxit but unconfirm so delete user


            try
            {

                // Check if the user already exists in Cognito
                try
                {
                    var getUserRequest = new AdminGetUserRequest
                    {
                        Username = Email, // Use email or username to identify the user
                        UserPoolId = _configuration["AWS:UserPoolId"]
                    };

                    var getUserResponse = await _provider.AdminGetUserAsync(getUserRequest);

                    // If user exists, check if they are unconfirmed
                    if (getUserResponse.UserStatus == UserStatusType.UNCONFIRMED)
                    {
                        // Delete the user if unconfirmed
                        var deleteUserRequest = new AdminDeleteUserRequest
                        {
                            Username = Email, // Use email or username to identify the user
                            UserPoolId = _configuration["AWS:UserPoolId"]
                        };

                        await _provider.AdminDeleteUserAsync(deleteUserRequest);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User already exists and is confirmed.");
                        return Page();
                    }
                }
                catch (UserNotFoundException)
                {
                    // User does not exist, proceed with registration
                }



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
                    return LocalRedirect($"/Identity/ConfirmRegistration?username={Username}&mail={Email}&role={Role}");

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
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
