using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class SupervisorService
    {
        private readonly IRepository _repository;

        public SupervisorService(IRepository repository)
        {
            _repository = repository;
        }

        public List<InternshipSupervisor> GetSupervisorsData()
        {
            string query = @"
                SELECT s.SupervisorID, s.FirstName, s.LastName, s.ContactPhone, s.Expertise, s.Email, s.CompanyID, u.passwordhash
                FROM InternshipSupervisors s
                LEFT JOIN Users u ON s.UserID = u.UserID";
            var supervisors = new List<InternshipSupervisor>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var supervisor = new InternshipSupervisor
                            {
                                SupervisorID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                ContactPhone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Expertise = reader.GetString(4),
                                Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CompanyID = reader.GetInt32(6),
                                PasswordHash = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };
                            supervisors.Add(supervisor);
                        }
                    }
                }
            }

            return supervisors;
        }

        public List<Student> GetStudentsBySupervisorEmail(string supervisorEmail)
        {
            string query = @"
                SELECT s.UserID, s.FirstName, s.LastName, s.AcademicYear, s.NationalID, s.BirthDate, 
                       s.SchoolNumber, s.Department, s.PhoneNumber, s.Email, s.Address
                FROM Students s
                INNER JOIN InternshipDetails i ON s.UserID = i.StudentID
                INNER JOIN InternshipSupervisors sup ON i.SupervisorID = sup.SupervisorID
                WHERE sup.Email = @SupervisorEmail";

            var students = new List<Student>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupervisorEmail", supervisorEmail);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var student = new Student
                            {
                                UserID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                AcademicYear = reader.GetInt32(3),
                                NationalID = reader.GetString(4),
                                BirthDate = reader.GetDateTime(5),
                                SchoolNumber = reader.GetString(6),
                                Department = reader.GetString(7),
                                PhoneNumber = reader.GetString(8),
                                Email = reader.GetString(9),
                                Address = reader.GetString(10),
                                PasswordHash = "123456"
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public InternshipSupervisor? GetSupervisorByEmail(string email)
        {
            string query = @"
                SELECT s.SupervisorID, s.FirstName, s.LastName, s.ContactPhone, s.Expertise, s.Email, s.CompanyID, u.PasswordHash
                FROM InternshipSupervisors s
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new InternshipSupervisor
                        {
                            SupervisorID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            ContactPhone = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Expertise = reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CompanyID = reader.GetInt32(6),
                            PasswordHash = reader.IsDBNull(7) ? null : reader.GetString(7)
                        };
                    }
                }
            }
            return null;
        }

        public Company? GetCompanyById(int companyId)
        {
            string query = "SELECT CompanyID, CompanyName, TaxNumber, Address, PhoneNumber, Email, Website FROM Company WHERE CompanyID = @CompanyID";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CompanyID", companyId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Company
                        {
                            CompanyID = reader.GetInt32(0),
                            CompanyName = reader.GetString(1),
                            TaxNumber = reader.GetString(2),
                            Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                            PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Website = reader.IsDBNull(6) ? null : reader.GetString(6)
                        };
                    }
                }
            }
            return null;
        }

        public void AddSupervisor(InternshipSupervisor supervisor)

        {

            string query = @"

                        INSERT INTO InternshipSupervisors (UserID, FirstName, LastName, Email, CompanyID, Expertise, ContactPhone)

                        VALUES (@UserID, @FirstName, @LastName, @Email, @CompanyID, @Expertise, @ContactPhone)";



            using (var connection = new SqlConnection(_repository.ConnectionString))

            {

                connection.Open();

                using (var command = new SqlCommand(query, connection))

                {

                    command.Parameters.AddWithValue("@UserID", (object)supervisor.UserID ?? DBNull.Value);

                    command.Parameters.AddWithValue("@FirstName", supervisor.FirstName);

                    command.Parameters.AddWithValue("@LastName", supervisor.LastName);

                    command.Parameters.AddWithValue("@Email", supervisor.Email);

                    command.Parameters.AddWithValue("@CompanyID", supervisor.CompanyID);

                    command.Parameters.AddWithValue("@Expertise", supervisor.Expertise);

                    command.Parameters.AddWithValue("@ContactPhone", supervisor.ContactPhone);



                    command.ExecuteNonQuery();

                }

            }

        }



        public Student? SearchStudentByName(string searchTerm, string supervisorEmail)

        {

            string query = @"

                        SELECT s.UserID, s.FirstName, s.LastName, s.AcademicYear, s.NationalID, s.BirthDate, 
                               s.SchoolNumber, s.Department, s.PhoneNumber, s.Email, s.Address
                        FROM Students s

                        INNER JOIN InternshipDetails i ON s.UserID = i.StudentID

                        INNER JOIN InternshipSupervisors sup ON i.SupervisorID = sup.SupervisorID

                        WHERE sup.Email = @SupervisorEmail AND (s.FirstName LIKE @SearchTerm OR s.LastName LIKE @SearchTerm OR (s.FirstName + ' ' + s.LastName) LIKE @SearchTerm)";



            using (var connection = new SqlConnection(_repository.ConnectionString))

            {

                connection.Open();

                using (var command = new SqlCommand(query, connection))

                {

                    command.Parameters.AddWithValue("@SupervisorEmail", supervisorEmail);

                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");



                    using (var reader = command.ExecuteReader())

                    {

                        if (reader.Read())

                        {

                            return new Student

                            {

                                UserID = reader.GetInt32(0),

                                FirstName = reader.GetString(1),

                                LastName = reader.GetString(2),

                                AcademicYear = reader.GetInt32(3),

                                NationalID = reader.GetString(4),

                                BirthDate = reader.GetDateTime(5),

                                SchoolNumber = reader.GetString(6),

                                Department = reader.GetString(7),

                                PhoneNumber = reader.GetString(8),

                                Email = reader.GetString(9),

                                Address = reader.GetString(10),

                                PasswordHash = "123456"

                            };

                        }

                    }

                }

            }



            return null;

        }

        public void UpdateSupervisor(InternshipSupervisor supervisor)
        {
            string query = @"
                UPDATE InternshipSupervisors
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    ContactPhone = @ContactPhone,
                    Expertise = @Expertise,
                    Email = @Email
                WHERE SupervisorID = @SupervisorID";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", supervisor.FirstName);
                    command.Parameters.AddWithValue("@LastName", supervisor.LastName);
                    command.Parameters.AddWithValue("@ContactPhone", (object)supervisor.ContactPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Expertise", supervisor.Expertise);
                    command.Parameters.AddWithValue("@Email", supervisor.Email);
                    command.Parameters.AddWithValue("@SupervisorID", supervisor.SupervisorID);

                    command.ExecuteNonQuery();
                }
            }
        }

        public EvaluationPersonel? GetEvaluation(int studentId, int supervisorId)
        {
            string query = "SELECT * FROM Evaluations WHERE StudentID = @StudentID AND SupervisorID = @SupervisorID";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentID", studentId);
                command.Parameters.AddWithValue("@SupervisorID", supervisorId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EvaluationPersonel
                        {
                            EvaluationID = reader.GetInt32(reader.GetOrdinal("EvaluationID")),
                            StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                            SupervisorID = reader.GetInt32(reader.GetOrdinal("SupervisorID")),
                            AttendanceScore = reader.GetInt32(reader.GetOrdinal("AttendanceScore")),
                            ResponsibilityScore = reader.GetInt32(reader.GetOrdinal("ResponsibilityScore")),
                            KnowledgeScore = reader.GetInt32(reader.GetOrdinal("KnowledgeScore")),
                            ProblemSolvingScore = reader.GetInt32(reader.GetOrdinal("ProblemSolvingScore")),
                            EquipmentUsageScore = reader.GetInt32(reader.GetOrdinal("EquipmentUsageScore")),
                            CommunicationScore = reader.GetInt32(reader.GetOrdinal("CommunicationScore")),
                            MotivationScore = reader.GetInt32(reader.GetOrdinal("MotivationScore")),
                            ReportingScore = reader.GetInt32(reader.GetOrdinal("ReportingScore")),
                            TeamworkScore = reader.GetInt32(reader.GetOrdinal("TeamworkScore")),
                            ExpressionScore = reader.GetInt32(reader.GetOrdinal("ExpressionScore"))
                        };
                    }
                }
            }
            return null;
        }
    }

}

