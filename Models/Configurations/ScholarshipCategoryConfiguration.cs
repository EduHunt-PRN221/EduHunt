using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class ScholarshipCategoryConfiguration : _BaseConfiguration<ScholarshipCategory>
    {
        public override void Configure(EntityTypeBuilder<ScholarshipCategory> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.ScholarshipId).IsRequired();
            builder.Property(e => e.CategoryId).IsRequired();

            builder.HasOne(d => d.Scholarship)
                  .WithMany(p => p.ScholarshipCategories)
                  .HasForeignKey(d => d.ScholarshipId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Category)
                  .WithMany(p => p.ScholarshipCategories)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
