using Amazon.CognitoIdentityProvider;
using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Cvs;
using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;

namespace Eduhunt
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllCustomServices(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonCognitoIdentityProvider>();
            services.AddScoped<CvService>();
            services.AddScoped<ApplicationUserService>();
            services.AddScoped<ProfileService>();
            services.AddScoped<PaymentService>();

            return services;
        }
    }
}
