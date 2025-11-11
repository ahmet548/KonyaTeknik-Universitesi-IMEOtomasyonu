using IMEAutomationDBOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<InternshipSupervisor> InternshipSupervisors { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasKey(s => s.UserID);

            modelBuilder.Entity<Video>()
                .HasOne<Student>()
                .WithMany()
                .HasForeignKey(v => v.StudentID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>().ToTable("Company");
        }
    }
}
