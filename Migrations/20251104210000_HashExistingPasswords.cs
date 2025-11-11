
using Microsoft.EntityFrameworkCore.Migrations;
using BCrypt.Net;

#nullable disable

namespace IMEAutomationDBOperations.Migrations
{
    /// <inheritdoc />
    public partial class HashExistingPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                UPDATE Students
                SET PasswordHash = '$2a$11$your_default_hashed_password_here'
                WHERE PasswordHash IS NULL OR PasswordHash = '' OR PasswordHash NOT LIKE '$2a$%'
                "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is a one-way migration, but you can add a down method if needed.
        }
    }
}
