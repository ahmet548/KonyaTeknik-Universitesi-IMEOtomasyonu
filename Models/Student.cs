using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMEAutomationDBOperations.Models
{
    public class Student
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }
        
        [Required]
        public string NationalID { get; set; } = string.Empty;
        
        [Required]
        public string SchoolNumber { get; set; } = string.Empty;
        
        public int AcademicYear { get; set; }
        
        [Required]
        public string Department { get; set; } = string.Empty;
        
        public DateTime BirthDate { get; set; }
        
        public string? Address { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("Id")]
        public User? User { get; set; }

        public ICollection<Internship>? Internships { get; set; }
        public ICollection<Video>? Videos { get; set; }
        public ICollection<Note>? Notes { get; set; }

        // Backward compatibility properties for UI layer
        [NotMapped]
        public string FirstName 
        { 
            get => User?.FirstName ?? string.Empty; 
            set { if (User != null) User.FirstName = value; } 
        }

        [NotMapped]
        public string LastName 
        { 
            get => User?.LastName ?? string.Empty; 
            set { if (User != null) User.LastName = value; } 
        }

        [NotMapped]
        public string Email 
        { 
            get => User?.Email ?? string.Empty; 
            set { if (User != null) User.Email = value; } 
        }

        [NotMapped]
        public string PasswordHash 
        { 
            get => User?.PasswordHash ?? string.Empty; 
            set { if (User != null) User.PasswordHash = value; } 
        }

        [NotMapped]
        public int StudentID { get => Id; set => Id = value; }
        
        [NotMapped]
        public int UserID { get => Id; set => Id = value; }
    }
}
