namespace IMEAutomationDBOperations.Models
{
    public class EvaluationPersonel
    {
        public int EvaluationID { get; set; }
        public int StudentID { get; set; }
        public int SupervisorID { get; set; }
        public int AttendanceScore { get; set; }
        public int ResponsibilityScore { get; set; }
        public int KnowledgeScore { get; set; }
        public int ProblemSolvingScore { get; set; }
        public int EquipmentUsageScore { get; set; }
        public int CommunicationScore { get; set; }
        public int MotivationScore { get; set; }
        public int ReportingScore { get; set; }
        public int TeamworkScore { get; set; }
        public int ExpressionScore { get; set; }
        public string? Feedback { get; set; }
    }
}