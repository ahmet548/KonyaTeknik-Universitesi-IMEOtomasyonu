using System;
using Microsoft.Data.SqlClient;

namespace IMEAutomationDBOperations
{
    public interface IRepository
    {
        void ExecuteQuery(string query);
    }

    public class SqlRepository : IRepository
    {
        private string _connectionString;

        public SqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
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

    public class DatabaseService
    {
        private IRepository _repository;
        private string _connectionString;

        public DatabaseService(IRepository repository, string connectionString)
        {
            _repository = repository;
            _connectionString = connectionString;
        }

        public void CreateDatabase()
        {
            string createDbQuery = @"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InternshipDB') 
                BEGIN
                    CREATE DATABASE InternshipDB;
                END";
            _repository.ExecuteQuery(createDbQuery);
        }

        public void CreateTables()
        {
            string createTablesQuery = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
            BEGIN
                CREATE TABLE Roles (
                    RoleID INT IDENTITY(1,1) PRIMARY KEY,
                    RoleName NVARCHAR(50) UNIQUE NOT NULL
                );
            END

            IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName IN ('Admin', 'Student', 'Academician', 'InternshipSupervisor', 'Guest'))
            BEGIN
                INSERT INTO Roles (RoleName) VALUES 
                ('Admin'),
                ('Student'),
                ('Academician'),
                ('InternshipSupervisor'),
                ('Guest');
            END

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

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipSupervisors')
            BEGIN
                CREATE TABLE InternshipSupervisors (
                    SupervisorID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    ContactPhone NVARCHAR(15),
                    Expertise NVARCHAR(100),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

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

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipDetails')
            BEGIN
                CREATE TABLE InternshipDetails (
                    InternshipID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT FOREIGN KEY REFERENCES Students(StudentID) ON DELETE CASCADE,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
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
                ";
            _repository.ExecuteQuery(createTablesQuery);
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

    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=DESKTOP-JAU4GNF\MSSQLSERVER01; Initial Catalog=InternshipDB; Integrated Security=True; TrustServerCertificate=True;";

            IRepository repository = new SqlRepository(connectionString);
            DatabaseService dbService = new DatabaseService(repository, connectionString);

            dbService.CreateDatabase();
            dbService.CreateTables();
            dbService.GetUsersData();
        }
    }
}
