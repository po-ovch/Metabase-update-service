using EntityLib;
using Microsoft.EntityFrameworkCore;

namespace Service_for_database.Model;

    public class DatabaseContext : DbContext
    {
        public DbSet<MetabaseProperty> PropertiesInfo { get; set; } = null!;
        public DbSet<MetabaseSystem> SystemInfo { get; set; } = null!;

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
            modelBuilder.Entity<MetabaseSystem>()
                .HasKey(nameof(MetabaseSystem.DBID),
                    nameof(MetabaseSystem.SystemId));
        }
    }
