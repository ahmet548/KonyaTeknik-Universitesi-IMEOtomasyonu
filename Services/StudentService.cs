using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class StudentService
    {
        private readonly IRepository _repository;

        public StudentService(IRepository repository)
        {
            _repository = repository;
        }

        public List<Student> GetStudentsData()
        {
            string query = @"
                SELECT
                    s.StudentID,
                    s.UserID,
                    s.FirstName,
                    s.LastName,
                    s.AcademicYear,
                    s.NationalID,
                    s.BirthDate,
                    s.SchoolNumber,
                    s.Department,
                    s.PhoneNumber,
                    s.Email,
                    s.Address,
                    u.PasswordHash
                FROM
                    Students s
                LEFT JOIN
                    Users u ON s.UserID = u.UserID";
            var students = new List<Student>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var student = new Student
                            {
                                StudentID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                UserID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                AcademicYear = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                NationalID = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                BirthDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                                SchoolNumber = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Department = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                PhoneNumber = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                Email = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                Address = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                PasswordHash = reader.IsDBNull(12) ? string.Empty : reader.GetString(12)
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public void UpdateStudent(Student student)
        {
            string query = @"
            UPDATE Students
            SET 
                FirstName = @FirstName,
                LastName = @LastName,
                AcademicYear = @AcademicYear,
                NationalID = @NationalID,
                BirthDate = @BirthDate,
                SchoolNumber = @SchoolNumber,
                PhoneNumber = @PhoneNumber,
                Address = @Address,
                Department = @Department,
                Email = @Email
            WHERE StudentID = @StudentID";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", student.LastName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AcademicYear", student.AcademicYear);
                    command.Parameters.AddWithValue("@NationalID", student.NationalID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                    command.Parameters.AddWithValue("@SchoolNumber", student.SchoolNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Department", student.Department ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Veritabanı Hatası: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public Student GetStudentById(int studentId)
        {
            string query = @"
                SELECT
                    s.StudentID,
                    s.UserID,
                    s.FirstName,
                    s.LastName,
                    s.AcademicYear,
                    s.NationalID,
                    s.BirthDate,
                    s.SchoolNumber,
                    s.Department,
                    s.PhoneNumber,
                    s.Email,
                    s.Address,
                    u.PasswordHash
                FROM
                    Students s
                LEFT JOIN
                    Users u ON s.UserID = u.UserID
                WHERE s.StudentID = @StudentID";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                StudentID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                UserID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                AcademicYear = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                NationalID = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                BirthDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                                SchoolNumber = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Department = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                PhoneNumber = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                Email = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                Address = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                PasswordHash = reader.IsDBNull(12) ? string.Empty : reader.GetString(12)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int AddStudentAndReturnId(Student student)
        {
            string query = @"
                INSERT INTO Students (UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, SchoolNumber, Department, PhoneNumber, Email, Address, PasswordHash)
                VALUES (@UserID, @FirstName, @LastName, @AcademicYear, @NationalID, @BirthDate, @SchoolNumber, @Department, @PhoneNumber, @Email, @Address, @PasswordHash);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", student.UserID);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@AcademicYear", student.AcademicYear);
                    command.Parameters.AddWithValue("@NationalID", student.NationalID);
                    command.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                    command.Parameters.AddWithValue("@SchoolNumber", student.SchoolNumber);
                    command.Parameters.AddWithValue("@Department", student.Department);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@Address", student.Address);
                    command.Parameters.AddWithValue("@PasswordHash", student.PasswordHash);

                    return (int)command.ExecuteScalar();
                }
            }
        }
    }
}