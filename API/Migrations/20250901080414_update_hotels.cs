using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class update_hotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HolidaysTime",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SaturdayTime",
                table: "Hotels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeekdayTime",
                table: "Hotels",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HolidaysTime",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "SaturdayTime",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "WeekdayTime",
                table: "Hotels");
        }
    }
}
