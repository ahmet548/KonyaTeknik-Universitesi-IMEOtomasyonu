using System;

namespace IMEAutomationDBOperations.Models
{
    public class SupervisorStudentViewModel
    {
        public Student Student { get; set; } = null!;
        public int TotalDays { get; set; }
        public int FilledDays { get; set; }
        public double CompletionRate { get; set; }
        public bool IsWeeklyVideoUploaded { get; set; }
        public bool IsVideoUploadEnabled { get; set; }
    }
}
