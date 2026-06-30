using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class cards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardTable",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CoinsSpentOnCards",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardTable",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CoinsSpentOnCards",
                table: "Users");
        }
    }
}
