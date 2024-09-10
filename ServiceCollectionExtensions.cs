using Eduhunt.Applications.Cvs;

namespace Eduhunt
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllCustomServices(this IServiceCollection services)
        {
            services.AddScoped<CvService>();

            return services;
        }
    }
}
