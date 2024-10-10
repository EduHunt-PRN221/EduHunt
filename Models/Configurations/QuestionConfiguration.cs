
using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class QuestionConfiguration : _BaseConfiguration<Question>
    {
        public override void Configure(EntityTypeBuilder<Question> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id).HasName("PK__Question__0DC06F8C4764F5B1");

            builder.ToTable("Question");

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");
            builder.Property(e => e.Content).HasMaxLength(255);
        }
    }
}
