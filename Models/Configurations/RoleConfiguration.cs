using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class RoleConfiguration : _BaseConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(255);
        }
    }
}
