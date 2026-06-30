using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class endlessChallenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndlessScore",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndlessScoreLastUpdated",
                table: "Users",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "LifetimeEndlessScore",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EndlessChallenge",
                columns: table => new
                {
                    endlessChallengeId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    LevelId = table.Column<string>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    Skipped = table.Column<bool>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndlessChallenge", x => x.endlessChallengeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndlessChallenge");

            migrationBuilder.DropColumn(
                name: "EndlessScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndlessScoreLastUpdated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LifetimeEndlessScore",
                table: "Users");
        }
    }
}
