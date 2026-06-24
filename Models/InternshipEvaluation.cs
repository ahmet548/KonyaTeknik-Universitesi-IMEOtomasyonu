using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMEAutomationDBOperations.Models
{
    public class InternshipEvaluation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InternshipId { get; set; }
        
        [Required]
        public int SupervisorId { get; set; }
        
        [Range(0, 100)]
        public int AttendanceScore { get; set; }
        
        [Range(0, 100)]
        public int ResponsibilityScore { get; set; }
        
        [Range(0, 100)]
        public int KnowledgeScore { get; set; }
        
        [Range(0, 100)]
        public int ProblemSolvingScore { get; set; }
        
        [Range(0, 100)]
        public int EquipmentUsageScore { get; set; }
        
        [Range(0, 100)]
        public int CommunicationScore { get; set; }
        
        [Range(0, 100)]
        public int MotivationScore { get; set; }
        
        [Range(0, 100)]
        public int ReportingScore { get; set; }
        
        [Range(0, 100)]
        public int TeamworkScore { get; set; }
        
        [Range(0, 100)]
        public int ExpressionScore { get; set; }
        
        public string? Feedback { get; set; }
        
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
        
        [NotMapped]
        public int EvaluationID { get => Id; set => Id = value; }

        [NotMapped]
        public int StudentID { get => Internship?.StudentId ?? 0; set { /* read-only essentially, but provide a dummy setter if needed */ } }
        
        // Navigation properties
        [ForeignKey("InternshipId")]
        public Internship? Internship { get; set; }
        
        [ForeignKey("SupervisorId")]
        public InternshipSupervisor? Supervisor { get; set; }
    }
}
