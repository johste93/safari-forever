using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class NotificationsLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationLinks",
                columns: table => new
                {
                    NotificationLinkId = table.Column<string>(nullable: false),
                    NotificationId = table.Column<string>(nullable: true),
                    ButtonText = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLinks", x => x.NotificationLinkId);
                    table.ForeignKey(
                        name: "FK_NotificationLinks_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLinks_NotificationId",
                table: "NotificationLinks",
                column: "NotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationLinks");
        }
    }
}
