using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class ApplicationUserConfiguration : _BaseConfiguration<ApplicationUser>
    {
        public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.NormalizedUserName).HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.NormalizedEmail).HasMaxLength(100);
            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Property(c => c.IsNotDeleted).HasDefaultValue(true);
        }
    }
}
