using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class moreNotificationToggles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notification_FollowerPlayedLevel",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Notification_FollowedLikedLevel",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Notification_FollowedPlayedLevel",
                table: "Users",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Followings",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                table: "Followings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notification_FollowedLikedLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Notification_FollowedPlayedLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Followings");

            migrationBuilder.AddColumn<bool>(
                name: "Notification_FollowerPlayedLevel",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
