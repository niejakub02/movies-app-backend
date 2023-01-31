using app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace app_backend.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(e => e.Username).IsUnique();
            modelBuilder.UseSerialColumns();
        }
    }
}
