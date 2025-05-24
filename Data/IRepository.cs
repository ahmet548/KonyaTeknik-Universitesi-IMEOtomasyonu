namespace IMEAutomationDBOperations.Data
{
    public interface IRepository
    {
        string ConnectionString { get; }
        void ExecuteQuery(string query);
    }
}
