using IMEAutomationDBOperations.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class DatabaseSetupService
    {
        private readonly IRepository _repository;

        public DatabaseSetupService(IRepository repository)
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
    }
}