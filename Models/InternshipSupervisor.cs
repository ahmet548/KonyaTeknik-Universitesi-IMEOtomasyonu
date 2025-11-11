namespace IMEAutomationDBOperations.Models
{
    public class InternshipSupervisor
    {
        public int? UserID { get; set; }
        public int SupervisorID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ContactPhone { get; set; }
        public required string Expertise { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int CompanyID { get; set; }
        public Company? Company { get; set; }
    }
}