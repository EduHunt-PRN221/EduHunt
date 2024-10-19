using Amazon.CognitoIdentityProvider;
using Eduhunt.Applications.Answers;
using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.Cvs;
using Eduhunt.Applications.Payment;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Applications.Questions;
using Eduhunt.Applications.Scholarships;
using Eduhunt.Applications.Surveys;
using Eduhunt.Infrastructures.Common;

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
            services.AddScoped<SurveyService>();
            services.AddScoped<ScholarshipService>();
            services.AddScoped<OpenAIService>();
            services.AddScoped<CommonService>();
            services.AddScoped<QuestionService>();
            services.AddScoped<AnswerService>();
            return services;
        }
    }
}
