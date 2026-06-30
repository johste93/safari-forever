using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class DeathsAndJumps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Deaths",
                table: "LevelUserStats",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Jumps",
                table: "LevelUserStats",
                nullable: false,
                defaultValue: -1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deaths",
                table: "LevelUserStats");

            migrationBuilder.DropColumn(
                name: "Jumps",
                table: "LevelUserStats");
        }
    }
}
