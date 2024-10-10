using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class AnswerConfiguration : _BaseConfiguration<Answer>
    {
        public override void Configure(EntityTypeBuilder<Answer> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id).HasName("PK__Answer__D4825024E9F7E377");

            builder.ToTable("Answer");

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");
            builder.Property(e => e.AnswerText).HasMaxLength(255);
            builder.Property(e => e.QuestionId).HasColumnName("QuestionID");

            builder.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Answer__Question__2F10007B")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
