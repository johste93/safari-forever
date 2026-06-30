using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class LevelOfTheWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelsOfTheWeek",
                columns: table => new
                {
                    LevelOfTheWeekId = table.Column<string>(nullable: false),
                    LevelId = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelsOfTheWeek", x => x.LevelOfTheWeekId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelsOfTheWeek");
        }
    }
}
