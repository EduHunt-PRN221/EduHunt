using Eduhunt.Models.Configurations;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cv> Cv { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public DbSet<Profile> Profile { get; set; } = default!;
        public DbSet<Survey> Surveys { get; set; } = default!;
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; } = default!;
        public DbSet<Answer> Answers { get; set; } = default!;
        public DbSet<Scholarship> Scholarships { get; set; } = default!;
        public DbSet<UserScholarship> UserScholarships { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AnswerConfiguration());
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CvConfiguration());
            builder.ApplyConfiguration(new ProfileConfiguration());
            builder.ApplyConfiguration(new QuestionConfiguration());
            builder.ApplyConfiguration(new ScholarshipConfiguration());
            builder.ApplyConfiguration(new ScholarshipCategoryConfiguration());
            builder.ApplyConfiguration(new SurveyAnswerConfiguration());
            builder.ApplyConfiguration(new SurveyConfiguration());
            builder.ApplyConfiguration(new UserScholarshipConfiguration());
        }
    }
}
