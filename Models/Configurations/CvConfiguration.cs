using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class CvConfiguration : _BaseConfiguration<Cv>
    {
        public override void Configure(EntityTypeBuilder<Cv> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.UrlCV).HasMaxLength(100).IsRequired();
        }


    }
}
