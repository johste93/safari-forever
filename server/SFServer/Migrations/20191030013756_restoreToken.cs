using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class restoreToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestoreToken",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestoreToken",
                table: "Users");
        }
    }
}
