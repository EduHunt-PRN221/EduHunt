using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eduhunt.Models.Configurations
{
    public class SurveyConfiguration : _BaseConfiguration<Survey>
    {
        public override void Configure(EntityTypeBuilder<Survey> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id).HasName("PK__Survey__A5481F9D0CCB73F0");
            builder.ToTable("Survey");
            builder.HasIndex(e => e.UserId, "UQ__Survey__1788CCAD0C472ADB").IsUnique();
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");
            builder.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_at");
            builder.Property(e => e.Description).HasColumnType("text");
            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.UserId).HasColumnName("UserID");
            builder.HasOne<ApplicationUser>()
              .WithOne()
              .HasForeignKey<Survey>(d => d.UserId)
              .HasConstraintName("FK__Survey__UserID__2A4B4B5E")
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
