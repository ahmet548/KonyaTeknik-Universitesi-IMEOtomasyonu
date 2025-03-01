using System;
using Microsoft.Data.SqlClient;

public class CreateDatabase
{
    static void Main()
    {
        string connectionString = "Server=127.0.0.1,1433;Database=master;User Id=sa;Password=171230;TrustServerCertificate=True;";

        string createDbQuery = @"
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InternshipDB') 
        BEGIN
            CREATE DATABASE InternshipDB;
        END";

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(createDbQuery, conn);
                command.ExecuteNonQuery();
                Console.WriteLine("Veritabanı kontrol edildi veya oluşturuldu.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata oluştu: " + ex.Message);
            return;
        }

        string dbConnectionString = "Server=127.0.0.1,1433;Database=InternshipDB ;User Id=sa;Password=171230;TrustServerCertificate=True;";

        using (SqlConnection conn = new SqlConnection(dbConnectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("InternshipDB veritabanına bağlanıldı.");

               string createTablesQuery = @"
                CREATE TABLE Student (
                    StudentID INT IDENTITY(1,1) PRIMARY KEY,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    AcademicYear INT NOT NULL,
                    NationalID CHAR(11) UNIQUE NOT NULL,
                    BirthDate DATE NOT NULL,
                    SchoolNumber NVARCHAR(20) UNIQUE NOT NULL,
                    Department NVARCHAR(100) NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    Email NVARCHAR(100),
                    Address NVARCHAR(255)
                );

                CREATE TABLE Company (
                    CompanyID INT IDENTITY(1,1) PRIMARY KEY,
                    CompanyName NVARCHAR(255) NOT NULL,
                    TaxNumber NVARCHAR(20) UNIQUE NOT NULL,
                    EmployeeCount INT,
                    Departments NVARCHAR(255),
                    Address NVARCHAR(255),
                    PhoneOrWebsite NVARCHAR(100),
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

                CREATE TABLE InternshipSupervisor (
                    SupervisorID INT IDENTITY(1,1) PRIMARY KEY,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    ContactPhone NVARCHAR(15),
                    Expertise NVARCHAR(100)
                );

                CREATE TABLE InternshipDetails (
                    InternshipID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT FOREIGN KEY REFERENCES Student(StudentID) ON DELETE CASCADE,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
                    SupervisorID INT FOREIGN KEY REFERENCES InternshipSupervisor(SupervisorID) ON DELETE NO ACTION,
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
                
                CREATE TABLE LeaveDetails (
                    LeaveID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    LeaveStart DATE NOT NULL,
                    LeaveEnd DATE NOT NULL,
                    LeaveReason NVARCHAR(100) NOT NULL,
                    ReasonDetail NVARCHAR(MAX) NULL,
                    AddressDuringLeave NVARCHAR(255) NOT NULL,
                    CONSTRAINT FK_LeaveDetails_Student FOREIGN KEY (StudentID)
                    REFERENCES Student(StudentID) ON DELETE CASCADE
                );
                
                CREATE TABLE GradeDetails (
                    GradeID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    SupervisorID INT NOT NULL,
                    InternshipSupervisorEvaluation DECIMAL(5,2) NOT NULL,
                    InternshipInstructorEvaluation DECIMAL(5,2) NOT NULL,
                    WeeklyVideoPresentationScore DECIMAL(5,2) NOT NULL,
                    DepartmentInternshipCommissionScore DECIMAL(5,2) NOT NULL,
                    CONSTRAINT FK_StudentGradeInformation_Student FOREIGN KEY (StudentID)
                        REFERENCES Student(StudentID) ON DELETE CASCADE,
                    CONSTRAINT FK_StudentGradeInformation_InternshipSupervisor FOREIGN KEY (SupervisorID)
                        REFERENCES InternshipSupervisor(SupervisorID) ON DELETE NO ACTION
                );";

                using (SqlCommand cmd = new SqlCommand(createTablesQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Tablolar oluşturuldu veya zaten mevcut!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu (Tablo oluşturma): " + ex.Message);
            }
        }
    }
}
