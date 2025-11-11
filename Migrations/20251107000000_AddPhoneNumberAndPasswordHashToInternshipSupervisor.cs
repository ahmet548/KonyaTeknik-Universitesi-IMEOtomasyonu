using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberAndPasswordHashToInternshipSupervisor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "InternshipSupervisors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "InternshipSupervisors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "InternshipSupervisor");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "InternshipSupervisor");
        }
    }
}