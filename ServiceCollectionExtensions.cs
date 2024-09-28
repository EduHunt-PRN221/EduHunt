using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Cvs;
using Eduhunt.Infrastructures.Cloud;

namespace Eduhunt
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllCustomServices(this IServiceCollection services)
        {
            services.AddScoped<CvService>();
            services.AddScoped<ApplicationUserService>();
           // services.AddScoped<CloudinaryService>();

            return services;
        }
    }
}
