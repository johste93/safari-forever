using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class recordHolderChangedToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordHolder",
                table: "Levels",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Time",
                table: "CampaignTimes",
                nullable: false,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordHolder",
                table: "Levels");

            migrationBuilder.AlterColumn<float>(
                name: "Time",
                table: "CampaignTimes",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
