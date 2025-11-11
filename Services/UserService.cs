using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class UserService
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository)
        {
            _repository = repository;
        }

        public List<User> GetUsersData()
        {
            // Veritabanındaki 'passwordhash' (küçük harf) kolonunu okur.
            string query = "SELECT UserID, UserName, passwordhash, RoleID FROM Users";
            var users = new List<User>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                PasswordHash = reader.GetString(2), // Okunan düz metin şifreyi modeldeki 'PasswordHash' özelliğine atar.
                                RoleID = reader.GetInt32(3)
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public int AddUser(User user)
        {
            string query = "SELECT UserID FROM Users WHERE UserName = @UserName";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                int? existingUserId;
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    existingUserId = command.ExecuteScalar() as int?;
                }

                if (existingUserId.HasValue)
                {
                    // User exists, update password and return existing ID
                    string updateQuery = "UPDATE Users SET passwordhash = @passwordhash WHERE UserID = @UserID";
                    using (var updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                        updateCommand.Parameters.AddWithValue("@UserID", existingUserId.Value);
                        updateCommand.ExecuteNonQuery();
                    }
                    return existingUserId.Value;
                }
                else
                {
                    // User does not exist, insert new user
                    string insertQuery = @"
                        INSERT INTO Users (UserName, passwordhash, RoleID)
                        VALUES (@UserName, @passwordhash, @RoleID);
                        SELECT SCOPE_IDENTITY();";

                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@UserName", user.UserName);
                        insertCommand.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                        insertCommand.Parameters.AddWithValue("@RoleID", user.RoleID);

                        return Convert.ToInt32(insertCommand.ExecuteScalar());
                    }
                }
            }
        }

        public int AddUserAndReturnId(string userName, string passwordHash, int roleId)
        {
            // Verilen düz metin şifreyi (passwordHash) 'passwordhash' kolonuna yazar.
            string insertQuery = @"
                INSERT INTO Users (UserName, passwordhash, RoleID)
                VALUES (@UserName, @passwordhash, @RoleID);
                SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    // BURASI ÖNEMLİ: 'passwordHash' parametresindeki '12345' gibi düz metni alır ve veritabanına yazar.
                    command.Parameters.AddWithValue("@passwordhash", passwordHash);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public User? GetUserByUsername(string username)
        {
            string query = "SELECT UserID, UserName, passwordhash, RoleID FROM Users WHERE UserName = @UserName";
            User user = null;

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                PasswordHash = reader.GetString(2),
                                RoleID = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }

            return user;
        }
    }
}