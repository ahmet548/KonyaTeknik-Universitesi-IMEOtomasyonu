namespace IMEAutomationDBOperations.Models
{
    public class User
    {
        public int UserID { get; set; }
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
        public int RoleID { get; set; }
    }

}