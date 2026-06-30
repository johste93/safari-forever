using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class notificationOnFollowedPublishLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Notification_FollowedPublishedLevel",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notification_FollowedPublishedLevel",
                table: "Users");
        }
    }
}
