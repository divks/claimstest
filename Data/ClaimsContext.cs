using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data
{
    /// <summary>
    /// Database context for Claims application
    /// </summary>
    public class ClaimsContext : DbContext
    {
        public DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public ClaimsContext(DbContextOptions<ClaimsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Only configure MongoDB collections if we're using MongoDB
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                modelBuilder.Entity<Claim>().ToCollection("claims");
                modelBuilder.Entity<Cover>().ToCollection("covers");
            }
        }
    }
}
