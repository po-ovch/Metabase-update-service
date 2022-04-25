

using EntityLib;
using Microsoft.EntityFrameworkCore;

namespace Service_for_metabase.Models
{
    public class MetabaseContext : DbContext
    {
        public DbSet<MetabaseProperty> PropertiesInfo { get; set; } = null!;

        public MetabaseContext(DbContextOptions<MetabaseContext> options)
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