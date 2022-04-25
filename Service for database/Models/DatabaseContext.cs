using EntityLib;
using Microsoft.EntityFrameworkCore;

namespace Service_for_database.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<MetabaseProperty> MetaProperties { get; set; } = null!;

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetabaseProperty>()
                .HasKey(nameof(MetabaseProperty.DBID),
                    nameof(MetabaseProperty.PropId));
        }
    }
}