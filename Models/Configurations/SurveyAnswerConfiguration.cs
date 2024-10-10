using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class SurveyAnswerConfiguration : _BaseConfiguration<SurveyAnswer>
    {
        public override void Configure(EntityTypeBuilder<SurveyAnswer> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

            builder.ToTable("SurveyAnswer");

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");
            builder.Property(e => e.AnswerId).HasColumnName("AnswerID");
            builder.Property(e => e.SurveyId).HasColumnName("SurveyID");

            builder.HasOne(d => d.Answer).WithMany(p => p.SurveyAnswers)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("FK__SurveyAns__Answe__32E0915F")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Survey).WithMany(p => p.SurveyAnswers)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK__SurveyAns__Surve__31EC6D26")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
