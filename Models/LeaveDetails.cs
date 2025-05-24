namespace IMEAutomationDBOperations.Models
{
    public class LeaveDetails
    {
        public int LeaveID { get; set; }
        public int StudentID { get; set; }
        public string? LeaveReason { get; set; }
        public string? ReasonDetail { get; set; }
        public string? AddressDuringLeave { get; set; }
        public string? LeaveStatus { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }
    }
}