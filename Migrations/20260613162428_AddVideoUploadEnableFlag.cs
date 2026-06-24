using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUploadEnableFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVideoUploadEnabled",
                table: "Internships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Internships",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsVideoUploadEnabled",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVideoUploadEnabled",
                table: "Internships");
        }
    }
}
