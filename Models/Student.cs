using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMEAutomationDBOperations.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public required string? FirstName { get; set; }
        public required string? LastName { get; set; }
        public required int AcademicYear { get; set; }
        public required string NationalID { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string SchoolNumber { get; set; }
        public required string Department { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public required string PasswordHash { get; set; }
    }
}