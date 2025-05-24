namespace IMEAutomationDBOperations.Models
{
    public class InternshipDetails
    {
        public int InternshipID { get; set; }
        public int StudentID { get; set; }
        public int CompanyID { get; set; }
        public int? SupervisorID { get; set; }
        public string InternshipTitle { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTrainingDays { get; set; }
        public int LeaveDays { get; set; }
        public string WorkDays { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
