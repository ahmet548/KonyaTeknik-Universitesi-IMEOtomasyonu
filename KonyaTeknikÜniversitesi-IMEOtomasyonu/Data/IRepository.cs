namespace IMEAutomationDBOperations.Data
{
    public interface IRepository
    {
        void ExecuteQuery(string query);
        void GetUsersData();
    }
}
