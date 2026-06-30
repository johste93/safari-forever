using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class RecentPlaysAndTrendingLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecentsPlays",
                columns: table => new
                {
                    RecentPlayId = table.Column<string>(nullable: false),
                    LevelId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentsPlays", x => x.RecentPlayId);
                });

            migrationBuilder.CreateTable(
                name: "TrendingLevels",
                columns: table => new
                {
                    TrendingLevelId = table.Column<string>(nullable: false),
                    LevelId = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendingLevels", x => x.TrendingLevelId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecentsPlays");

            migrationBuilder.DropTable(
                name: "TrendingLevels");
        }
    }
}
