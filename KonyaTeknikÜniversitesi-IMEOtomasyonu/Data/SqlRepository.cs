using System;
using Microsoft.Data.SqlClient;

namespace IMEAutomationDBOperations.Data
{
    public class SqlRepository : IRepository
    {
        private readonly string _connectionString;

        public SqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            using (var conn = new SqlConnection(_connectionString))
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

        public void GetUsersData()
        {
            string query = "SELECT * FROM Users";
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"UserID: {reader["UserID"]}, UserName: {reader["UserName"]}");
                    }
                }
            }
        }
    }
}
