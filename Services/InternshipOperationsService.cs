using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class InternshipOperationsService
    {
        private readonly IRepository _repository;

        public InternshipOperationsService(IRepository repository)
        {
            _repository = repository;
        }

        public (InternshipSupervisor?, Company?, InternshipDetails?, EvaluationPersonel?) GetSupervisorCompanyAndInternshipDetailsByStudentEmail(string studentEmail)
        {
            string query = @"
                SELECT 
                    s.SupervisorID, s.FirstName, s.LastName, ISNULL(s.ContactPhone, '') AS ContactPhone, ISNULL(s.Expertise, '') AS Expertise,
                    c.CompanyID, c.CompanyName, c.TaxNumber, c.Address, c.PhoneNumber, c.Email,
                    ISNULL(c.Departments, '') AS Departments, ISNULL(c.Website, '') AS Website, ISNULL(c.Industry, '') AS Industry,
                    ISNULL(c.ManagerFirstName, '') AS ManagerFirstName, ISNULL(c.ManagerLastName, '') AS ManagerLastName,
                    ISNULL(c.ManagerPhone, '') AS ManagerPhone, ISNULL(c.ManagerEmail, '') AS ManagerEmail,
                    ISNULL(c.BankName, '') AS BankName, ISNULL(c.BankBranch, '') AS BankBranch, ISNULL(c.BankIbanNo, '') AS BankIbanNo,
                    i.InternshipID, i.StudentID, i.CompanyID, i.SupervisorID, i.InternshipTitle, 
                    i.StartDate, i.EndDate, i.TotalTrainingDays, i.LeaveDays, i.WorkDays, 
                    i.PaidAmount, i.CreatedAt, i.UpdatedAt,
                    e.AttendanceScore, e.ResponsibilityScore, e.KnowledgeScore, e.ProblemSolvingScore, 
                    e.EquipmentUsageScore, e.CommunicationScore, e.MotivationScore, e.ReportingScore, 
                    e.TeamworkScore, e.ExpressionScore
                FROM Students st
                INNER JOIN InternshipDetails i ON st.StudentID = i.StudentID
                LEFT JOIN InternshipSupervisors s ON i.SupervisorID = s.SupervisorID
                LEFT JOIN Company c ON i.CompanyID = c.CompanyID
                LEFT JOIN Evaluations e ON st.StudentID = e.StudentID
                WHERE st.Email = @StudentEmail";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentEmail", studentEmail);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var supervisor = reader.IsDBNull(0) ? null : new InternshipSupervisor
                            {
                                SupervisorID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                ContactPhone = reader.GetString(3),
                                Expertise = reader.GetString(4)
                            };

                            var company = reader.IsDBNull(5) ? null : new Company
                            {
                                CompanyID = reader.GetInt32(5),
                                CompanyName = reader.GetString(6),
                                TaxNumber = reader.GetString(7),
                                Address = reader.GetString(8),
                                PhoneNumber = reader.GetString(9),
                                Email = reader.GetString(10),
                                Departments = reader.GetString(11),
                                Website = reader.GetString(12),
                                Industry = reader.GetString(13),
                                ManagerFirstName = reader.GetString(14),
                                ManagerLastName = reader.GetString(15),
                                ManagerPhone = reader.GetString(16),
                                ManagerEmail = reader.GetString(17),
                                BankName = reader.GetString(18),
                                BankBranch = reader.GetString(19),
                                BankIbanNo = reader.GetString(20)
                            };


                            var internshipDetails = new InternshipDetails
                            {
                                InternshipID = reader.GetInt32(21),
                                StudentID = reader.GetInt32(22),
                                CompanyID = reader.IsDBNull(23) ? (int?)null : reader.GetInt32(23),
                                SupervisorID = reader.IsDBNull(24) ? (int?)null : reader.GetInt32(24),
                                InternshipTitle = reader.GetString(25),
                                StartDate = reader.GetDateTime(26),
                                EndDate = reader.GetDateTime(27),
                                TotalTrainingDays = reader.GetInt32(28),
                                LeaveDays = reader.GetInt32(29),
                                WorkDays = reader.IsDBNull(30) ? string.Empty : reader.GetString(30),
                                PaidAmount = reader.GetDecimal(31),
                                CreatedAt = reader.GetDateTime(32),
                                UpdatedAt = reader.GetDateTime(33)
                            };

                            var evaluation = reader.IsDBNull(34) ? null : new EvaluationPersonel
                            {
                                AttendanceScore = reader.GetInt32(34),
                                ResponsibilityScore = reader.GetInt32(35),
                                KnowledgeScore = reader.GetInt32(36),
                                ProblemSolvingScore = reader.GetInt32(37),
                                EquipmentUsageScore = reader.GetInt32(38),
                                CommunicationScore = reader.GetInt32(39),
                                MotivationScore = reader.GetInt32(40),
                                ReportingScore = reader.GetInt32(41),
                                TeamworkScore = reader.GetInt32(42),
                                ExpressionScore = reader.GetInt32(43)
                            };

                            return (supervisor, company, internshipDetails, evaluation);
                        }
                    }
                }
            }

            return (null, null, null, null);
        }

        public void SaveEvaluation(EvaluationPersonel evaluation)
        {
            string query = @"
                INSERT INTO Evaluations (
                    StudentID, SupervisorID, AttendanceScore, ResponsibilityScore, KnowledgeScore, 
                    ProblemSolvingScore, EquipmentUsageScore, CommunicationScore, MotivationScore, 
                    ReportingScore, TeamworkScore, ExpressionScore
                ) VALUES (
                    @StudentID, @SupervisorID, @AttendanceScore, @ResponsibilityScore, @KnowledgeScore, 
                    @ProblemSolvingScore, @EquipmentUsageScore, @CommunicationScore, @MotivationScore, 
                    @ReportingScore, @TeamworkScore, @ExpressionScore
                );

                UPDATE InternshipDetails
                SET InstructorFeedback = @Feedback
                WHERE StudentID = @StudentID;
            ";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", evaluation.StudentID);
                    command.Parameters.AddWithValue("@SupervisorID", 3); // Bu değeri dinamik hale getirmeyi düşünebilirsiniz
                    command.Parameters.AddWithValue("@AttendanceScore", evaluation.AttendanceScore);
                    command.Parameters.AddWithValue("@ResponsibilityScore", evaluation.ResponsibilityScore);
                    command.Parameters.AddWithValue("@KnowledgeScore", evaluation.KnowledgeScore);
                    command.Parameters.AddWithValue("@ProblemSolvingScore", evaluation.ProblemSolvingScore);
                    command.Parameters.AddWithValue("@EquipmentUsageScore", evaluation.EquipmentUsageScore);
                    command.Parameters.AddWithValue("@CommunicationScore", evaluation.CommunicationScore);
                    command.Parameters.AddWithValue("@MotivationScore", evaluation.MotivationScore);
                    command.Parameters.AddWithValue("@ReportingScore", evaluation.ReportingScore);
                    command.Parameters.AddWithValue("@TeamworkScore", evaluation.TeamworkScore);
                    command.Parameters.AddWithValue("@ExpressionScore", evaluation.ExpressionScore);
                    command.Parameters.AddWithValue("@Feedback", evaluation.Feedback ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddStudent(Student student, int supervisorId, int companyId)
        {
            string query = @"
                INSERT INTO Students (UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, 
                                      SchoolNumber, Department, PhoneNumber, Email, Address, PasswordHash)
                VALUES (@UserID, @FirstName, @LastName, @AcademicYear, @NationalID, @BirthDate, 
                        @SchoolNumber, @Department, @PhoneNumber, @Email, @Address, @PasswordHash);

                -- Staj detaylarını ekle
                INSERT INTO InternshipDetails (StudentID, SupervisorID, CompanyID, InternshipTitle, StartDate, EndDate, 
                                               TotalTrainingDays, LeaveDays, WorkDays, PaidAmount, CreatedAt, UpdatedAt)
                VALUES (@UserID, @SupervisorID, @CompanyID, 'Staj Başlığı', GETDATE(), GETDATE(), 
                        30, 0, '', 0, GETDATE(), GETDATE());

                -- Sorumlu personele öğrenciyi ekle
                UPDATE InternshipSupervisors
                SET StudentID = @UserID
                WHERE SupervisorID = @SupervisorID;
            ";

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
                    command.Parameters.AddWithValue("@SupervisorID", supervisorId);
                    command.Parameters.AddWithValue("@CompanyID", companyId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public EvaluationPersonel? GetEvaluationByStudentId(int studentId)
        {
            string query = "SELECT * FROM Evaluations WHERE StudentID = @StudentID";
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
            }
            return null;
        }

        public InternshipDetails? GetInternshipDetailsByStudentId(int studentId)
        {
            string query = "SELECT * FROM InternshipDetails WHERE StudentID = @StudentID";
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
                            return new InternshipDetails
                            {
                                InternshipID = reader.GetInt32(reader.GetOrdinal("InternshipID")),
                                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                                CompanyID = reader.IsDBNull(reader.GetOrdinal("CompanyID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CompanyID")),
                                SupervisorID = reader.IsDBNull(reader.GetOrdinal("SupervisorID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SupervisorID")),
                                InternshipTitle = reader.GetString(reader.GetOrdinal("InternshipTitle")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                TotalTrainingDays = reader.GetInt32(reader.GetOrdinal("TotalTrainingDays")),
                                LeaveDays = reader.GetInt32(reader.GetOrdinal("LeaveDays")),
                                WorkDays = reader.IsDBNull(reader.GetOrdinal("WorkDays")) ? string.Empty : reader.GetString(reader.GetOrdinal("WorkDays")),
                                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                InstructorFeedback = reader.IsDBNull(reader.GetOrdinal("InstructorFeedback")) ? null : reader.GetString(reader.GetOrdinal("InstructorFeedback")),
                                CommissionFeedback = reader.IsDBNull(reader.GetOrdinal("CommissionFeedback")) ? null : reader.GetString(reader.GetOrdinal("CommissionFeedback"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AddInternshipDetails(
            int studentId, int supervisorId, int companyId,
            DateTime startDate, DateTime endDate, string[] workDays, string internshipTitle)
        {
            string workDaysString = string.Join(",", workDays);

            string query = @"
                INSERT INTO InternshipDetails (
                    StudentID, SupervisorID, CompanyID, InternshipTitle, StartDate, EndDate, 
                    TotalTrainingDays, LeaveDays, WorkDays, PaidAmount, CreatedAt, UpdatedAt
                ) VALUES (
                    @StudentID, @SupervisorID, @CompanyID, @InternshipTitle, @StartDate, @EndDate, 
                    @TotalTrainingDays, @LeaveDays, @WorkDays, @PaidAmount, @CreatedAt, @UpdatedAt
                )";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@SupervisorID", supervisorId);
                    command.Parameters.AddWithValue("@CompanyID", companyId);
                    command.Parameters.AddWithValue("@InternshipTitle", internshipTitle);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@TotalTrainingDays", (endDate - startDate).Days + 1); // Simple calculation
                    command.Parameters.AddWithValue("@LeaveDays", 0); // Default to 0
                    command.Parameters.AddWithValue("@WorkDays", workDaysString);
                    command.Parameters.AddWithValue("@PaidAmount", 0); // Default to 0
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}