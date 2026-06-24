using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    Departments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankIbanNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternshipSupervisors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Expertise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipSupervisors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternshipSupervisors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipSupervisors_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NationalID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchoolNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Internships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalTrainingDays = table.Column<int>(type: "int", nullable: false),
                    LeaveDays = table.Column<int>(type: "int", nullable: false),
                    WorkDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: true),
                    InstructorFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Internships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Internships_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Internships_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NoteID);
                    table.ForeignKey(
                        name: "FK_Notes_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoID);
                    table.ForeignKey(
                        name: "FK_Videos_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InternshipEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternshipId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: false),
                    AttendanceScore = table.Column<int>(type: "int", nullable: false),
                    ResponsibilityScore = table.Column<int>(type: "int", nullable: false),
                    KnowledgeScore = table.Column<int>(type: "int", nullable: false),
                    ProblemSolvingScore = table.Column<int>(type: "int", nullable: false),
                    EquipmentUsageScore = table.Column<int>(type: "int", nullable: false),
                    CommunicationScore = table.Column<int>(type: "int", nullable: false),
                    MotivationScore = table.Column<int>(type: "int", nullable: false),
                    ReportingScore = table.Column<int>(type: "int", nullable: false),
                    TeamworkScore = table.Column<int>(type: "int", nullable: false),
                    ExpressionScore = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternshipEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternshipEvaluations_InternshipSupervisors_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "InternshipSupervisors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternshipEvaluations_Internships_InternshipId",
                        column: x => x.InternshipId,
                        principalTable: "Internships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "Address", "BankBranch", "BankIbanNo", "BankName", "CompanyName", "Departments", "Email", "EmployeeCount", "Industry", "ManagerEmail", "ManagerFirstName", "ManagerLastName", "ManagerPhone", "PhoneNumber", "TaxNumber", "Website" },
                values: new object[] { 1, "ODTÜ Teknokent, Ankara", null, null, null, "TechCorp Yazılım A.Ş.", null, "info@techcorp.com", null, "Bilişim", null, null, null, null, "03125555555", "1234567890", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ahmet.yavuz@konyateknik.edu.tr", "Ahmet", true, "Yavuz", "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ayse.demir@techcorp.com", "Ayşe", true, "Demir", "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "komisyon@konyateknik.edu.tr", "Komisyon", true, "Başkanı", "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a", 4, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "InternshipSupervisors",
                columns: new[] { "Id", "CompanyId", "CreatedAt", "Expertise", "PhoneNumber", "UpdatedAt" },
                values: new object[] { 2, 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kıdemli Yazılım Geliştirici", "05321234567", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AcademicYear", "Address", "BirthDate", "CreatedAt", "Department", "NationalID", "PhoneNumber", "SchoolNumber", "UpdatedAt" },
                values: new object[] { 1, 4, "Selçuklu, Konya", new DateTime(2000, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bilgisayar Mühendisliği", "12345678901", "05551234567", "191234567", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Internships",
                columns: new[] { "Id", "CommissionFeedback", "CompanyId", "CreatedAt", "EndDate", "InstructorFeedback", "LeaveDays", "PaidAmount", "StartDate", "Status", "StudentId", "SupervisorId", "Title", "TotalTrainingDays", "UpdatedAt", "WorkDays" },
                values: new object[] { 1, null, 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0m, new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, 2, "Full-Stack Web Geliştirme Stajı", 60, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pazartesi-Salı-Çarşamba-Perşembe-Cuma" });

            migrationBuilder.InsertData(
                table: "InternshipEvaluations",
                columns: new[] { "Id", "AttendanceScore", "CommunicationScore", "EquipmentUsageScore", "EvaluatedAt", "ExpressionScore", "Feedback", "InternshipId", "KnowledgeScore", "MotivationScore", "ProblemSolvingScore", "ReportingScore", "ResponsibilityScore", "SupervisorId", "TeamworkScore" },
                values: new object[] { 1, 95, 0, 0, new DateTime(2023, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, null, 1, 85, 0, 0, 0, 90, 2, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_InternshipEvaluations_InternshipId",
                table: "InternshipEvaluations",
                column: "InternshipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternshipEvaluations_SupervisorId",
                table: "InternshipEvaluations",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_CompanyId",
                table: "Internships",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_StudentId",
                table: "Internships",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipSupervisors_CompanyId",
                table: "InternshipSupervisors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_StudentID",
                table: "Notes",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_StudentID",
                table: "Videos",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternshipEvaluations");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "InternshipSupervisors");

            migrationBuilder.DropTable(
                name: "Internships");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
