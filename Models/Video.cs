namespace IMEAutomationDBOperations.Models
{
    public class Video
    {
        public int VideoID { get; set; }
        public int StudentID { get; set; }
        public required string Title { get; set; }
        public required string? Description { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public required string FilePath { get; set; }
    }
}