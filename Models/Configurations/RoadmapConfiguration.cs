using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class RoadmapConfiguration : _BaseConfiguration<Roadmap>
    {
        public override void Configure(EntityTypeBuilder<Roadmap> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

            builder.Property(c => c.Title).HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(255);
        }
    }
}
