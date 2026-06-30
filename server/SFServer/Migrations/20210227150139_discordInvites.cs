using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class discordInvites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordInvites",
                columns: table => new
                {
                    PersonalDiscordInviteId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Joined = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordInvites", x => x.PersonalDiscordInviteId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordInvites");
        }
    }
}
