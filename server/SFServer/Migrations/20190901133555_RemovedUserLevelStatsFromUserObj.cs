using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class RemovedUserLevelStatsFromUserObj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelUserStats_Users_UserId",
                table: "LevelUserStats");

            migrationBuilder.DropIndex(
                name: "IX_LevelUserStats_UserId",
                table: "LevelUserStats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LevelUserStats_UserId",
                table: "LevelUserStats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LevelUserStats_Users_UserId",
                table: "LevelUserStats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
