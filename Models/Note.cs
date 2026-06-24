using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMEAutomationDBOperations.Models
{
    public class Note
    {
        public int NoteID { get; set; }
        public int StudentID { get; set; }
        public required string Title { get; set; }
        public string? SubTitle { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("StudentID")]
        public Student? Student { get; set; }
    }
}
