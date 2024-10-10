using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class ScholarshipConfiguration : _BaseConfiguration<Scholarship>
    {
        public override void Configure(EntityTypeBuilder<Scholarship> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Budget).HasMaxLength(255).IsRequired(false);
            builder.Property(e => e.Title).HasMaxLength(255).IsRequired();
            builder.Property(e => e.Location).HasMaxLength(255).IsRequired(false);
            builder.Property(e => e.SchoolName).HasMaxLength(255).IsRequired(false);
            builder.Property(e => e.IsInSite);
            builder.Property(e => e.Description).HasColumnType("text").IsRequired(false);
            builder.Property(e => e.Url).HasMaxLength(255).IsRequired(false);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.ImageUrl).IsRequired(false);
            builder.Property(e => e.Level).HasMaxLength(50).IsRequired();
            builder.Property(e => e.IsApproved).IsRequired();

            builder.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(e => e.AuthorId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
