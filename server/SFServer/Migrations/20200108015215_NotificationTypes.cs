using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class NotificationTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Notification_DailyChallengeResults",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_FollowerPlayedLevel",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_MyLevelIsLevelOfTheWeek",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_NewDailyChallenge",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_NewFollower",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_WorldRecordBeaten",
                table: "Users",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notification_DailyChallengeResults",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_FollowerPlayedLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_MyLevelIsLevelOfTheWeek",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_NewDailyChallenge",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_NewFollower",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_WorldRecordBeaten",
                table: "Users");
        }
    }
}
