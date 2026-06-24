using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IMEAutomationDBOperations.Models.Enums;

namespace IMEAutomationDBOperations.Models
{
    public class Internship
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int CompanyId { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTrainingDays { get; set; }
        public int LeaveDays { get; set; }
        public string WorkDays { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        
        public int? SupervisorId { get; set; }
        
        public string? InstructorFeedback { get; set; }
        public string? CommissionFeedback { get; set; }
        
        [NotMapped]
        public int StudentID { get => StudentId; set => StudentId = value; }
        
        public bool IsVideoUploadEnabled { get; set; } = false;
        
        public InternshipStatus Status { get; set; } = InternshipStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        
        public InternshipEvaluation? Evaluation { get; set; }
        
        [ForeignKey("SupervisorId")]
        public InternshipSupervisor? Supervisor { get; set; }
    }
}
