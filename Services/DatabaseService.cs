using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

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
            string query = "SELECT UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, SchoolNumber, Department, PhoneNumber, Email, Address FROM Students";
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
                    Department = @Department,
                    SchoolNumber = @SchoolNumber,
                    PhoneNumber = @PhoneNumber,
                    Email = @Email,
                    Address = @Address
                WHERE NationalID = @NationalID";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@AcademicYear", student.AcademicYear);
                    command.Parameters.AddWithValue("@Department", student.Department);
                    command.Parameters.AddWithValue("@SchoolNumber", student.SchoolNumber);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@Address", student.Address);
                    command.Parameters.AddWithValue("@NationalID", student.NationalID);

                    command.ExecuteNonQuery();
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
                    command.Parameters.AddWithValue("@SupervisorID", 1);
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

            return null; // Öğrenci bulunamazsa null döndür
        }
    }
}
