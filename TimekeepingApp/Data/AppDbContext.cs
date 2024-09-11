using Microsoft.EntityFrameworkCore;
using TimekeepingApp.Models;

namespace TimekeepingApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TimesheetModel> Timesheets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Ensure the connection string is correct and points to the appropriate SQL Server instance
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            modelBuilder.Entity<UserModel>()
                        .HasMany(u => u.Timesheets)
                        .WithOne(t => t.User)
                        .HasForeignKey(t => t.UserId)
                        .OnDelete(DeleteBehavior.Cascade); // Optional: Define delete behavior if needed

            base.OnModelCreating(modelBuilder);
        }
    }
}
