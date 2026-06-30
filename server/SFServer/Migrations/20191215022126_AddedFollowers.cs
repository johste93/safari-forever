using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class AddedFollowers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowerCount",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Followings",
                columns: table => new
                {
                    FollowingId = table.Column<string>(nullable: false),
                    UserBeingFollowedId = table.Column<string>(nullable: true),
                    UserFollowingId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followings", x => x.FollowingId);
                    table.ForeignKey(
                        name: "FK_Followings_Users_UserFollowingId",
                        column: x => x.UserFollowingId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followings_UserFollowingId",
                table: "Followings",
                column: "UserFollowingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followings");

            migrationBuilder.DropColumn(
                name: "FollowerCount",
                table: "Users");
        }
    }
}
