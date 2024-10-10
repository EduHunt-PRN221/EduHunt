using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class UserScholarshipConfiguration : _BaseConfiguration<UserScholarship>
    {
        public override void Configure(EntityTypeBuilder<UserScholarship> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.ScholarshipId).IsRequired();
            builder.Property(e => e.UserId).IsRequired();
        }
    }
}
