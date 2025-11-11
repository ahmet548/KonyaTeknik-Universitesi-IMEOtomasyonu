
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class StandardizePasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename Password to PasswordHash in Users table
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            // Rename Password to PasswordHash in Students table
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Students",
                newName: "PasswordHash");

            // Add PasswordHash column to InternshipSupervisors table
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "InternshipSupervisors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""); // You should update existing records with hashed passwords
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert PasswordHash to Password in Users table
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            // Revert PasswordHash to Password in Students table
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Students",
                newName: "Password");

            // Remove PasswordHash column from InternshipSupervisors table
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "InternshipSupervisors");
        }
    }
}
