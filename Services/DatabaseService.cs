using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace IMEAutomationDBOperations.Services
{
    public class DatabaseService
    {
        private readonly IRepository _repository;

        public DatabaseService(IRepository repository)
        {
            _repository = repository;
        }

        public void CreateDatabase()
        {
            string createDbQuery = @"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InternshipDB1') 
                BEGIN
                    CREATE DATABASE InternshipDB1;
                END";
            _repository.ExecuteQuery(createDbQuery);
        }

        public void CreateTables()
        {
            string createTablesQuery = @"
            -- Roles Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
            BEGIN
                CREATE TABLE Roles (
                    RoleID INT IDENTITY(1,1) PRIMARY KEY,
                    RoleName NVARCHAR(50) UNIQUE NOT NULL
                );
            END

            -- Rollerin eklenmesi
            IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName IN ('Admin', 'Student', 'Academician', 'InternshipSupervisor', 'Guest'))
            BEGIN
                INSERT INTO Roles (RoleName) VALUES 
                ('Admin'),
                ('Student'),
                ('Academician'),
                ('InternshipSupervisor'),
                ('Guest');
            END

            -- Users Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    UserID INT IDENTITY(1,1) PRIMARY KEY,
                    UserName NVARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(255) NOT NULL,  
                    RoleID INT NOT NULL,
                    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE CASCADE
                );
            END

            -- Company Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Company')
            BEGIN
                CREATE TABLE Company (
                    CompanyID INT IDENTITY(1,1) PRIMARY KEY,
                    CompanyName NVARCHAR(255) NOT NULL,
                    TaxNumber NVARCHAR(20) UNIQUE NOT NULL,
                    EmployeeCount INT,
                    Departments NVARCHAR(255),
                    Address NVARCHAR(255),
                    PhoneNumber NVARCHAR(15),
                    Website NVARCHAR(255),
                    Industry NVARCHAR(100),
                    Email NVARCHAR(100),
                    ManagerFirstName NVARCHAR(50),
                    ManagerLastName NVARCHAR(50),
                    ManagerPhone NVARCHAR(15),
                    ManagerEmail NVARCHAR(100),
                    BankName NVARCHAR(100),
                    BankBranch NVARCHAR(100),
                    BankIbanNo NVARCHAR(30)
                );
            END

            -- Students Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
            BEGIN
                CREATE TABLE Students (
                    StudentID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    AcademicYear INT NOT NULL,
                    NationalID CHAR(11) UNIQUE NOT NULL,
                    BirthDate DATE NOT NULL,
                    SchoolNumber NVARCHAR(20) UNIQUE NOT NULL,
                    Department NVARCHAR(100) NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    Email NVARCHAR(100),
                    Address NVARCHAR(255),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Academicians Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Academicians')
            BEGIN
                CREATE TABLE Academicians (
                    AcademicianID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    Department NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- InternshipSupervisors Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipSupervisors')
            BEGIN
                CREATE TABLE InternshipSupervisors (
                    SupervisorID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
					StudentID INT FOREIGN KEY REFERENCES Students(StudentID)  ON DELETE NO ACTION,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    ContactPhone NVARCHAR(15),
                    Expertise NVARCHAR(100),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Admins Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Admins')
            BEGIN
                CREATE TABLE Admins (
                    AdminID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FullName NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Guests Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
            BEGIN
                CREATE TABLE Guests (
                    GuestID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    Name NVARCHAR(100) NOT NULL,
                    VisitReason NVARCHAR(255),
                    ContactEmail NVARCHAR(100),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- InternshipDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipDetails')
            BEGIN
                CREATE TABLE InternshipDetails (
                    InternshipID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT FOREIGN KEY REFERENCES Students(StudentID)  ON DELETE NO ACTION,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID)  ON DELETE NO ACTION,
                    SupervisorID INT FOREIGN KEY REFERENCES InternshipSupervisors(SupervisorID) ON DELETE SET NULL,
                    InternshipTitle NVARCHAR(100) NOT NULL,
                    StartDate DATE NOT NULL,
                    EndDate DATE NOT NULL,
                    TotalTrainingDays INT NOT NULL CHECK (TotalTrainingDays > 0),
                    LeaveDays INT DEFAULT 0 CHECK (LeaveDays >= 0),
                    WorkDays NVARCHAR(100),
                    PaidAmount DECIMAL(10,2) DEFAULT 0 CHECK (PaidAmount >= 0),
                    CreatedAt DATETIME DEFAULT GETDATE(),
                    UpdatedAt DATETIME DEFAULT GETDATE()
                );
            END

            -- LeaveDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LeaveDetails')
            BEGIN
                CREATE TABLE LeaveDetails (
                    LeaveID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    LeaveStart DATE NOT NULL,
                    LeaveEnd DATE NOT NULL,
                    LeaveReason NVARCHAR(100) NOT NULL,
                    ReasonDetail NVARCHAR(MAX) NULL,
                    AddressDuringLeave NVARCHAR(255) NOT NULL,
                    LeaveStatus NVARCHAR(50) NOT NULL DEFAULT 'Onay Bekliyor',
                    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
                );
            END

            -- GradeDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GradeDetails')
            BEGIN
                CREATE TABLE GradeDetails (
                    GradeID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    SupervisorID INT NOT NULL,
                    InternshipSupervisorEvaluation DECIMAL(5,2) NOT NULL,
                    InternshipInstructorEvaluation DECIMAL(5,2) NOT NULL,
                    WeeklyVideoPresentationScore DECIMAL(5,2) NOT NULL,
                    DepartmentInternshipCommissionScore DECIMAL(5,2) NOT NULL,
                    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
                    FOREIGN KEY (SupervisorID) REFERENCES InternshipSupervisors(SupervisorID) ON DELETE NO ACTION
                );
            END

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GradeDetails')
            BEGIN
                CREATE TABLE Evaluations (
                EvaluationID INT IDENTITY(1,1) PRIMARY KEY,
                StudentID INT NOT NULL,
                SupervisorID INT NOT NULL,
                AttendanceScore INT NOT NULL,
                ResponsibilityScore INT NOT NULL,
                KnowledgeScore INT NOT NULL,
                ProblemSolvingScore INT NOT NULL,
                EquipmentUsageScore INT NOT NULL,
                CommunicationScore INT NOT NULL,
                MotivationScore INT NOT NULL,
                ReportingScore INT NOT NULL,
                TeamworkScore INT NOT NULL,
                xpressionScore INT NOT NULL,
                FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
                FOREIGN KEY (SupervisorID) REFERENCES InternshipSupervisors(SupervisorID)
                ON DELETE CASCADE);
            END
            ";

            _repository.ExecuteQuery(createTablesQuery);
        }

        public List<User> GetUsersData()
        {
            string query = "SELECT UserID, UserName, PasswordHash, RoleID FROM Users";
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
                                PasswordHash = reader.GetString(2),
                                RoleID = reader.GetInt32(3)
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public List<Student> GetStudentsData()
        {
            string query = "SELECT StudentID, FirstName, LastName, AcademicYear, NationalID, BirthDate, SchoolNumber, Department, PhoneNumber, Email, Address FROM Students";
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
                                UserID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                AcademicYear = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                NationalID = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                BirthDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                                SchoolNumber = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Department = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                PhoneNumber = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                Address = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                Password = "123456"
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public List<InternshipSupervisor> GetSupervisorsData()
        {
            string query = "SELECT SupervisorID, FirstName, LastName, ContactPhone, Expertise, Email FROM InternshipSupervisors";
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
                                Password = "123456"
                            };
                            supervisors.Add(supervisor);
                        }
                    }
                }
            }

            return supervisors;
        }

        public List<Company> GetCompaniesData()
        {
            var companies = new List<Company>();
            string query = @"SELECT CompanyID, CompanyName, TaxNumber, EmployeeCount, Departments, Address, PhoneNumber, Website, Industry, Email,
                            ManagerFirstName, ManagerLastName, ManagerPhone, ManagerEmail, BankName, BankBranch, BankIbanNo
                     FROM Company";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            companies.Add(new Company
                            {
                                CompanyID = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                TaxNumber = reader.GetString(2),
                                EmployeeCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Departments = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PhoneNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Website = reader.IsDBNull(7) ? null : reader.GetString(7),
                                Industry = reader.IsDBNull(8) ? null : reader.GetString(8),
                                Email = reader.IsDBNull(9) ? null : reader.GetString(9),
                                ManagerFirstName = reader.IsDBNull(10) ? null : reader.GetString(10),
                                ManagerLastName = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ManagerPhone = reader.IsDBNull(12) ? null : reader.GetString(12),
                                ManagerEmail = reader.IsDBNull(13) ? null : reader.GetString(13),
                                BankName = reader.IsDBNull(14) ? null : reader.GetString(14),
                                BankBranch = reader.IsDBNull(15) ? null : reader.GetString(15),
                                BankIbanNo = reader.IsDBNull(16) ? null : reader.GetString(16)
                            });
                        }
                    }
                }
            }
            return companies;
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
                INNER JOIN InternshipSupervisors s ON st.StudentID = s.StudentID
                INNER JOIN Company c ON s.CompanyID = c.CompanyID
                INNER JOIN InternshipDetails i ON st.StudentID = i.StudentID
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
                            var supervisor = new InternshipSupervisor
                            {
                                SupervisorID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                ContactPhone = reader.GetString(3),
                                Expertise = reader.GetString(4)
                            };

                            var company = new Company
                            {
                                CompanyID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
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
                                CompanyID = reader.GetInt32(23),
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
            WHERE Email = @Email";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
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
                )";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", evaluation.StudentID);
                    command.Parameters.AddWithValue("@SupervisorID", 3);
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

                    command.ExecuteNonQuery();

                }
            }
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
                                Password = "123456"
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public Student GetStudentById(int studentId)
        {
            string query = @"
                SELECT UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, 
                       SchoolNumber, Department, PhoneNumber, Email, Address
                FROM Students
                WHERE UserID = @StudentID";

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
                                Password = "123456"
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void AddStudent(Student student, int supervisorId, int companyId)
        {
            string query = @"
        DECLARE @NewStudentID INT;

        -- Öğrenciyi ekle ve yeni StudentID'yi al
        INSERT INTO Students (UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, 
                              SchoolNumber, Department, PhoneNumber, Email, Address)
        OUTPUT INSERTED.StudentID INTO @NewStudentID
        VALUES (@UserID, @FirstName, @LastName, @AcademicYear, @NationalID, @BirthDate, 
                @SchoolNumber, @Department, @PhoneNumber, @Email, @Address);

        -- Staj detaylarını ekle
        INSERT INTO InternshipDetails (StudentID, SupervisorID, CompanyID, InternshipTitle, StartDate, EndDate, 
                                       TotalTrainingDays, LeaveDays, WorkDays, PaidAmount, CreatedAt, UpdatedAt)
        VALUES (@NewStudentID, @SupervisorID, @CompanyID, 'Staj Başlığı', GETDATE(), GETDATE(), 
                30, 0, '', 0, GETDATE(), GETDATE());

        -- Sorumlu personele öğrenciyi ekle
        UPDATE InternshipSupervisors
        SET StudentID = @NewStudentID
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
                    command.Parameters.AddWithValue("@SupervisorID", supervisorId);
                    command.Parameters.AddWithValue("@CompanyID", companyId); // <-- EKLENDİ

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddInternshipDetails(int studentId, int supervisorId, int companyId, DateTime startDate, DateTime endDate, string[] workDays, string internshipTitle)
        {
            int totalTrainingDays = (endDate - startDate).Days;
            int leaveDays = 0;
            decimal paidAmount = 0;

            string query = @"
        INSERT INTO InternshipDetails (StudentID, SupervisorID, CompanyID, StartDate, EndDate, WorkDays, InternshipTitle, TotalTrainingDays, LeaveDays, PaidAmount)
        VALUES (@StudentID, @SupervisorID, @CompanyID, @StartDate, @EndDate, @WorkDays, @InternshipTitle, @TotalTrainingDays, @LeaveDays, @PaidAmount)";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@SupervisorID", supervisorId);
                    command.Parameters.AddWithValue("@CompanyID", companyId); // <-- EKLENDİ
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@WorkDays", string.Join("-", workDays));
                    command.Parameters.AddWithValue("@InternshipTitle", internshipTitle);
                    command.Parameters.AddWithValue("@TotalTrainingDays", totalTrainingDays);
                    command.Parameters.AddWithValue("@LeaveDays", leaveDays);
                    command.Parameters.AddWithValue("@PaidAmount", paidAmount);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int AddStudentAndReturnId(Student student)
        {
            string query = @"
        INSERT INTO Students (UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, SchoolNumber, Department, PhoneNumber, Email, Address, Password)
        VALUES (@UserID, @FirstName, @LastName, @AcademicYear, @NationalID, @BirthDate, @SchoolNumber, @Department, @PhoneNumber, @Email, @Address, @Password);
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
                    command.Parameters.AddWithValue("@Password", student.Password);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public int AddUser(User user)
        {
            string checkQuery = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();

                // Kullanıcı adı kontrolü
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserName", user.UserName);
                    int userExists = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userExists > 0)
                    {
                        throw new Exception("Bu kullanıcı adı zaten mevcut.");
                    }
                }

                // Yeni kullanıcı ekleme
                string insertQuery = @"
                    INSERT INTO Users (UserName, PasswordHash, RoleID)
                    VALUES (@UserName, @PasswordHash, @RoleID);
                    SELECT SCOPE_IDENTITY();";

                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserName", user.UserName);
                    insertCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    insertCommand.Parameters.AddWithValue("@RoleID", user.RoleID);

                    return Convert.ToInt32(insertCommand.ExecuteScalar());
                }
            }
        }

        public int AddUserAndReturnId(string userName, string passwordHash, int roleId)
        {
            string insertQuery = @"
        INSERT INTO Users (UserName, PasswordHash, RoleID)
        VALUES (@UserName, @PasswordHash, @RoleID);
        SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public List<Note> GetUserNotes(string email)
        {
            var notes = new List<Note>();

            string query = @"
        SELECT n.NoteID, n.StudentID, n.Title, n.SubTitle, n.Content, n.CreatedAt, n.UpdatedAt
        FROM Notes n
        INNER JOIN Students s ON n.StudentID = s.StudentID
        WHERE s.Email = @Email
        ORDER BY n.CreatedAt DESC";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var note = new Note
                            {
                                NoteID = reader.GetInt32(0),
                                StudentID = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                SubTitle = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Content = reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5),
                                UpdatedAt = reader.GetDateTime(6)
                            };
                            notes.Add(note);
                        }
                    }
                }
            }

            return notes;
        }

        public List<Video> GetUserVideos(string email)
        {
            var videos = new List<Video>();

            string query = @"
        SELECT v.VideoID, v.StudentID, v.Title, v.Description, v.FilePath, v.UploadDate
        FROM Videos v
        INNER JOIN Students s ON v.StudentID = s.StudentID
        WHERE s.Email = @Email
        ORDER BY v.UploadDate DESC";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var video = new Video
                            {
                                VideoID = reader.GetInt32(0),
                                StudentID = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                FilePath = reader.GetString(4),
                                UploadDate = reader.GetDateTime(5)
                            };
                            videos.Add(video);
                        }
                    }
                }
            }

            return videos;
        }

        public void AddUserNote(string email, Note note)
        {
            string query = @"
                INSERT INTO Notes (StudentID, Title, SubTitle, Content, CreatedAt, UpdatedAt)
                SELECT s.StudentID, @Title, @SubTitle, @Content, @CreatedAt, @UpdatedAt
                FROM Students s WHERE s.Email = @Email";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Title", note.Title);
                    command.Parameters.AddWithValue("@SubTitle", (object)note.SubTitle ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Content", note.Content);
                    command.Parameters.AddWithValue("@CreatedAt", note.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", note.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUserNote(string email, int noteId)
        {
            string query = @"
        DELETE FROM Notes
        WHERE NoteID = @NoteID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NoteID", noteId);
                    command.Parameters.AddWithValue("@Email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUserVideo(string email, int videoId)
        {
            string query = @"
        DELETE FROM Videos
        WHERE VideoID = @VideoID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VideoID", videoId);
                    command.Parameters.AddWithValue("@Email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddUserVideo(string email, Video video)
        {
            string query = @"
        INSERT INTO Videos (StudentID, Title, Description, FilePath, UploadDate)
        SELECT s.StudentID, @Title, @Description, @FilePath, @UploadDate
        FROM Students s WHERE s.Email = @Email";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Title", video.Title);
                    command.Parameters.AddWithValue("@Description", video.Description != null ? video.Description : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FilePath", video.FilePath);
                    command.Parameters.AddWithValue("@UploadDate", video.UploadDate);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUserNote(string email, int noteId, string title, string subTitle, string content)
        {
            string query = @"UPDATE Notes
                             SET Title = @Title, SubTitle = @SubTitle, Content = @Content, UpdatedAt = @UpdatedAt
                             WHERE NoteID = @NoteID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@SubTitle", (object)subTitle ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@NoteID", noteId);
                    command.Parameters.AddWithValue("@Email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUserVideo(string email, int videoId, string title, string description)
        {
            string query = @"UPDATE Videos
                             SET Title = @Title, Description = @Description
                             WHERE VideoID = @VideoID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@VideoID", videoId);
                    command.Parameters.AddWithValue("@Email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddLeaveDetail(string email, DateTime leaveStart, DateTime leaveEnd, string leaveReason, string reasonDetail, string addressDuringLeave)
        {
            var student = GetStudentsData().FirstOrDefault(s => s.Email == email);
            if (student == null) return;

            DateTime minSqlDate = new DateTime(1753, 1, 1);
            if (leaveStart < minSqlDate || leaveEnd < minSqlDate)
                throw new ArgumentException("Geçersiz izin tarihi!");

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
            INSERT INTO LeaveDetails (StudentID, LeaveStart, LeaveEnd, LeaveReason, ReasonDetail, AddressDuringLeave)
            VALUES (@StudentID, @LeaveStart, @LeaveEnd, @LeaveReason, @ReasonDetail, @AddressDuringLeave)", connection);

                command.Parameters.AddWithValue("@StudentID", student.UserID);
                command.Parameters.AddWithValue("@LeaveStart", leaveStart);
                command.Parameters.AddWithValue("@LeaveEnd", leaveEnd);
                command.Parameters.AddWithValue("@LeaveReason", leaveReason ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ReasonDetail", reasonDetail ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AddressDuringLeave", addressDuringLeave ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public List<LeaveDetails> GetLeaveDetailsByEmail(string email)
        {
            var student = GetStudentsData().FirstOrDefault(s => s.Email == email);
            if (student == null) return new List<LeaveDetails>();

            var list = new List<LeaveDetails>();
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM LeaveDetails WHERE StudentID = @StudentID ORDER BY LeaveStart DESC", connection);
                command.Parameters.AddWithValue("@StudentID", student.UserID);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LeaveDetails
                        {
                            LeaveID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            LeaveStart = reader.GetDateTime(2),
                            LeaveEnd = reader.GetDateTime(3),
                            LeaveReason = reader.GetString(4),
                            ReasonDetail = reader.IsDBNull(5) ? null : reader.GetString(5),
                            AddressDuringLeave = reader.GetString(6),
                            LeaveStatus = reader.IsDBNull(7) ? "Onay Bekliyor" : reader.GetString(7)
                        });
                    }
                }
            }
            return list;
        }

        public void DeleteLeaveDetail(string email, int leaveId)
        {
            var student = GetStudentsData().FirstOrDefault(s => s.Email == email);
            if (student == null) return;

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "DELETE FROM LeaveDetails WHERE LeaveID = @LeaveID AND StudentID = @StudentID AND LeaveStatus = 'Onay Bekliyor'",
                    connection);
                command.Parameters.AddWithValue("@LeaveID", leaveId);
                command.Parameters.AddWithValue("@StudentID", student.UserID);
                command.ExecuteNonQuery();
            }
        }
    }
}
