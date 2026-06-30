using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class RenameRecentsPlays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecentsPlays",
                table: "RecentsPlays");

            migrationBuilder.RenameTable(
                name: "RecentsPlays",
                newName: "RecentPlays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecentPlays",
                table: "RecentPlays",
                column: "RecentPlayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecentPlays",
                table: "RecentPlays");

            migrationBuilder.RenameTable(
                name: "RecentPlays",
                newName: "RecentsPlays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecentsPlays",
                table: "RecentsPlays",
                column: "RecentPlayId");
        }
    }
}
