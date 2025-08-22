using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class update_room_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestCount",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Area",
                table: "RoomTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAirCondition",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBalcony",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasExtraTowels",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasGardenView",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasJacuzzi",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasKettle",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasKitchenette",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasMiniFridge",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSeaView",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasTV",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasVault",
                table: "RoomTypes",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasAirCondition",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasBalcony",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasExtraTowels",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasGardenView",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasJacuzzi",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasKettle",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasKitchenette",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasMiniFridge",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasSeaView",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasTV",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "HasVault",
                table: "RoomTypes");

            migrationBuilder.AddColumn<int>(
                name: "GuestCount",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
