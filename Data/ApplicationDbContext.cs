using Eduhunt.Models.Configurations;
using Eduhunt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cv> Cv { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CvConfiguration());
        }
    }
}
