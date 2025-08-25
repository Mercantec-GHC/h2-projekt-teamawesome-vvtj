using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Update_User_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserInfos_UserInfoId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserInfoId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserInfos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "UserInfos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialRequests",
                table: "UserInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_Users_UserId",
                table: "UserInfos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_Users_UserId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "SpecialRequests",
                table: "UserInfos");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserInfos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserInfoId",
                table: "Users",
                column: "UserInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserInfos_UserInfoId",
                table: "Users",
                column: "UserInfoId",
                principalTable: "UserInfos",
                principalColumn: "UserId");
        }
    }
}
