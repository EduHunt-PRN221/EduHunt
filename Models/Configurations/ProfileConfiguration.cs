using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class ProfileConfiguration : _BaseConfiguration<Profile>
    {
        public override void Configure(EntityTypeBuilder<Profile> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.FirstName).HasMaxLength(100);
            builder.Property(c => c.LastName).HasMaxLength(100);
            builder.Property(c => c.UserName).HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.PhoneNumber).HasMaxLength(100);
            builder.Property(c => c.Country).HasMaxLength(100);
            builder.Property(c => c.City).HasMaxLength(100);
            builder.Property(c => c.Title).HasMaxLength(100);
            builder.Property(c => c.AvatarImage).HasMaxLength(100);


            builder.HasOne(c => c.ApplicationUser)
                .WithOne()
                .HasForeignKey<Profile>(c => c.ApplicationUserId);
        }
    }
}
