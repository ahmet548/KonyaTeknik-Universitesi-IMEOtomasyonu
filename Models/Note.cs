namespace IMEAutomationDBOperations.Models
{
    public class Note
    {
        public int NoteID { get; set; }
        public int StudentID { get; set; }
        public required string Title { get; set; }
        public required string SubTitle { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}