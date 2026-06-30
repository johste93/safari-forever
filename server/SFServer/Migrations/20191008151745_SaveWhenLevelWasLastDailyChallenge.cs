using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class SaveWhenLevelWasLastDailyChallenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DailyChallengeOn",
                table: "Levels",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyChallengeOn",
                table: "Levels");
        }
    }
}
