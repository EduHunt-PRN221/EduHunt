using Eduhunt.Models.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Models.Configurations
{
    public abstract class _BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IHasId, IHasSoftDelete
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            ConfigureBaseProperties(builder);
        }

        protected virtual void ConfigureBaseProperties(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.Id).HasMaxLength(50);

            builder.Property(e => e.IsNotDeleted).HasDefaultValue(true);
        }
    }
}
