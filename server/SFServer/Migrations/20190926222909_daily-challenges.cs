using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class dailychallenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyChallengeParticipants",
                columns: table => new
                {
                    DailyChallengeParticipantId = table.Column<string>(nullable: false),
                    DailyChallengeId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    Time = table.Column<double>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallengeParticipants", x => x.DailyChallengeParticipantId);
                });

            migrationBuilder.CreateTable(
                name: "DailyChallenges",
                columns: table => new
                {
                    DailyChallengeId = table.Column<string>(nullable: false),
                    LevelId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallenges", x => x.DailyChallengeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyChallengeParticipants");

            migrationBuilder.DropTable(
                name: "DailyChallenges");
        }
    }
}
