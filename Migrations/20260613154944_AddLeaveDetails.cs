using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveDetails",
                columns: table => new
                {
                    LeaveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    LeaveReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressDuringLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaveEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveDetails", x => x.LeaveID);
                    table.ForeignKey(
                        name: "FK_LeaveDetails_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Internships_SupervisorId",
                table: "Internships",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveDetails_StudentID",
                table: "LeaveDetails",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Internships_InternshipSupervisors_SupervisorId",
                table: "Internships",
                column: "SupervisorId",
                principalTable: "InternshipSupervisors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Internships_InternshipSupervisors_SupervisorId",
                table: "Internships");

            migrationBuilder.DropTable(
                name: "LeaveDetails");

            migrationBuilder.DropIndex(
                name: "IX_Internships_SupervisorId",
                table: "Internships");
        }
    }
}
