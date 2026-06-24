using System;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<InternshipSupervisor> InternshipSupervisors { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Internship> Internships { get; set; }
        public DbSet<InternshipEvaluation> Evaluations { get; set; }
        public DbSet<LeaveDetails> LeaveDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LeaveDetails>().HasKey(l => l.LeaveID);

            // Table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<InternshipSupervisor>().ToTable("InternshipSupervisors");
            modelBuilder.Entity<Company>().ToTable("Companies");
            modelBuilder.Entity<Internship>().ToTable("Internships");
            modelBuilder.Entity<InternshipEvaluation>().ToTable("InternshipEvaluations");
            modelBuilder.Entity<Note>().ToTable("Notes");
            modelBuilder.Entity<Video>().ToTable("Videos");

            // Relationships
            
            // User -> Student (1 to 1)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.StudentDetails)
                .HasForeignKey<Student>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Supervisor (1 to 1)
            modelBuilder.Entity<InternshipSupervisor>()
                .HasOne(s => s.User)
                .WithOne(u => u.SupervisorDetails)
                .HasForeignKey<InternshipSupervisor>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Supervisor -> Company (N to 1)
            modelBuilder.Entity<InternshipSupervisor>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Supervisors)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Internship -> Student
            modelBuilder.Entity<Internship>()
                .HasOne(i => i.Student)
                .WithMany(s => s.Internships)
                .HasForeignKey(i => i.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Internship -> Company
            modelBuilder.Entity<Internship>()
                .HasOne(i => i.Company)
                .WithMany(c => c.Internships)
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Internship -> Evaluation (1 to 1)
            modelBuilder.Entity<InternshipEvaluation>()
                .HasOne(e => e.Internship)
                .WithOne(i => i.Evaluation)
                .HasForeignKey<InternshipEvaluation>(e => e.InternshipId)
                .OnDelete(DeleteBehavior.Cascade);

            // Evaluation -> Supervisor
            modelBuilder.Entity<InternshipEvaluation>()
                .HasOne(e => e.Supervisor)
                .WithMany(s => s.Evaluations)
                .HasForeignKey(e => e.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Video -> Student
            modelBuilder.Entity<Video>()
                .HasOne(v => v.Student)
                .WithMany(s => s.Videos)
                .HasForeignKey(v => v.StudentID)
                .OnDelete(DeleteBehavior.Cascade);

            // Note -> Student
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Student)
                .WithMany(s => s.Notes)
                .HasForeignKey(n => n.StudentID)
                .OnDelete(DeleteBehavior.Cascade);


            // SEED DATA
            // Seed Roles & Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "ahmet.yavuz@konyateknik.edu.tr", FirstName = "Ahmet", LastName = "Yavuz", PasswordHash = "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", Role = Role.Student, IsActive = true, CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) },
                new User { Id = 2, Email = "ayse.demir@techcorp.com", FirstName = "Ayşe", LastName = "Demir", PasswordHash = "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", Role = Role.Supervisor, IsActive = true, CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) },
                new User { Id = 3, Email = "komisyon@konyateknik.edu.tr", FirstName = "Komisyon", LastName = "Başkanı", PasswordHash = "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", Role = Role.Commission, IsActive = true, CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) }
            );

            // Student
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, NationalID = "12345678901", SchoolNumber = "191234567", AcademicYear = 4, Department = "Bilgisayar Mühendisliği", BirthDate = new DateTime(2000, 5, 15), Address = "Selçuklu, Konya", PhoneNumber = "05551234567", CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) }
            );

            // Company
            modelBuilder.Entity<Company>().HasData(
                new Company { CompanyId = 1, CompanyName = "TechCorp Yazılım A.Ş.", TaxNumber = "1234567890", Industry = "Bilişim", Email = "info@techcorp.com", PhoneNumber = "03125555555", Address = "ODTÜ Teknokent, Ankara" }
            );

            // Supervisor
            modelBuilder.Entity<InternshipSupervisor>().HasData(
                new InternshipSupervisor { Id = 2, CompanyId = 1, Expertise = "Kıdemli Yazılım Geliştirici", PhoneNumber = "05321234567", CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) }
            );

            // Internship
            modelBuilder.Entity<Internship>().HasData(
                new Internship { Id = 1, StudentId = 1, CompanyId = 1, SupervisorId = 2, Title = "Full-Stack Web Geliştirme Stajı", StartDate = new DateTime(2023, 6, 15), EndDate = new DateTime(2023, 9, 15), TotalTrainingDays = 60, WorkDays = "Pazartesi-Salı-Çarşamba-Perşembe-Cuma", Status = InternshipStatus.Completed, CreatedAt = new DateTime(2023, 1, 1), UpdatedAt = new DateTime(2023, 1, 1) }
            );

            // Evaluation
            modelBuilder.Entity<InternshipEvaluation>().HasData(
                new InternshipEvaluation { Id = 1, InternshipId = 1, SupervisorId = 2, AttendanceScore = 95, ResponsibilityScore = 90, KnowledgeScore = 85, EvaluatedAt = new DateTime(2023, 9, 20) }
            );
        }
    }
}
