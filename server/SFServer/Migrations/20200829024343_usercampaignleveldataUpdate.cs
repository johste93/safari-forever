using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class usercampaignleveldataUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Milliseconds",
                table: "UserCampaignLevelData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "UserCampaignLevelData",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Milliseconds",
                table: "UserCampaignLevelData");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "UserCampaignLevelData");
        }
    }
}
