using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class RewardType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RewardType",
                table: "Transactions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardType",
                table: "Transactions");
        }
    }
}
