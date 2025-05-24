namespace IMEAutomationDBOperations.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public required string CompanyName { get; set; }
        public required string TaxNumber { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? EmployeeCount { get; set; } 
        public string? Departments { get; set; }
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? ManagerFirstName { get; set; }
        public string? ManagerLastName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? BankIbanNo { get; set; }
    }
}