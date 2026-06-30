using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class highscoreChangedToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Milliseconds",
                table: "LevelUserStats",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "LevelUserStats",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Record_Milliseconds",
                table: "Levels",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Record_Seconds",
                table: "Levels",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Milliseconds",
                table: "DailyChallengeParticipants",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "DailyChallengeParticipants",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Milliseconds",
                table: "CampaignTimes",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "CampaignTimes",
                nullable: false,
                defaultValue: -1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Milliseconds",
                table: "LevelUserStats");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "LevelUserStats");

            migrationBuilder.DropColumn(
                name: "Record_Milliseconds",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "Record_Seconds",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "Milliseconds",
                table: "DailyChallengeParticipants");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "DailyChallengeParticipants");

            migrationBuilder.DropColumn(
                name: "Milliseconds",
                table: "CampaignTimes");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "CampaignTimes");
        }
    }
}
