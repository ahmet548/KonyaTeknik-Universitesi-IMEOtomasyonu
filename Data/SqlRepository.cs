using System;
using Microsoft.Data.SqlClient;

namespace IMEAutomationDBOperations.Data
{
    public class SqlRepository : IRepository
    {
        public string ConnectionString { get; }

        public SqlRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"UserID: {reader["UserID"]}, UserName: {reader["UserName"]}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: " + ex.Message);
                }
            }
        }
    }
}
