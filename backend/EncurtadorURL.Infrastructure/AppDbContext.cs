using EncurtadorURL.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EncurtadorURL.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UrlRecord> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlRecord>().HasIndex(u => u.ShortCode).IsUnique();
        }

    }
}
