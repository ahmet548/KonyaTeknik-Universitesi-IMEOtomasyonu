using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMEAutomationDBOperations.Models
{
    public class InternshipSupervisor
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }
        
        public int CompanyId { get; set; }
        
        [Required]
        public string Expertise { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("Id")]
        public User? User { get; set; }
        
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public ICollection<InternshipEvaluation>? Evaluations { get; set; }

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
        public int SupervisorID { get => Id; set => Id = value; }

        [NotMapped]
        public int SupervisorId { get => Id; set => Id = value; }

        [NotMapped]
        public int UserID { get => Id; set => Id = value; }

        [NotMapped]
        public string ContactPhone { get => PhoneNumber ?? string.Empty; set => PhoneNumber = value; }
    }
}
