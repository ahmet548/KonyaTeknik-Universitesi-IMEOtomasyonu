using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IMEAutomationDBOperations.Models.Enums;

namespace IMEAutomationDBOperations.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public Role Role { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int RoleID { get => (int)Role; set => Role = (Role)value; }
        
        [NotMapped]
        public string UserName 
        { 
            get => FirstName + " " + LastName; 
            set 
            {
                if (string.IsNullOrEmpty(value)) return;
                var parts = value.Split(' ');
                FirstName = parts[0];
                if (parts.Length > 1) LastName = parts[parts.Length - 1];
            }
        }

        [NotMapped]
        public int UserID { get => Id; set => Id = value; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public Student? StudentDetails { get; set; }
        public InternshipSupervisor? SupervisorDetails { get; set; }
    }
}
