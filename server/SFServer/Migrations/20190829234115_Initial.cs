using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignTimes",
                columns: table => new
                {
                    CampaignTimeId = table.Column<string>(nullable: false),
                    World = table.Column<int>(nullable: false),
                    LevelIndex = table.Column<int>(nullable: false),
                    Time = table.Column<float>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTimes", x => x.CampaignTimeId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    Banned = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Read = table.Column<bool>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Nickname = table.Column<string>(nullable: true),
                    Identifier = table.Column<int>(nullable: false),
                    Banned = table.Column<bool>(nullable: false),
                    AgreedToTermsOfService = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    LastActive = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    LevelId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SerializedLevel = table.Column<string>(nullable: true),
                    Plays = table.Column<int>(nullable: false),
                    Deaths = table.Column<int>(nullable: false),
                    Wins = table.Column<int>(nullable: false),
                    Likes = table.Column<int>(nullable: false),
                    Dislikes = table.Column<int>(nullable: false),
                    Record = table.Column<float>(nullable: false),
                    GameVersion = table.Column<string>(nullable: true),
                    CreatorUserId = table.Column<string>(nullable: true),
                    Thumbnail = table.Column<byte[]>(nullable: true),
                    MainColor = table.Column<string>(nullable: true),
                    SubColor = table.Column<string>(nullable: true),
                    WallColor = table.Column<string>(nullable: true),
                    PatternColor = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.LevelId);
                    table.ForeignKey(
                        name: "FK_Levels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LevelUserStats",
                columns: table => new
                {
                    LevelUserStatsId = table.Column<string>(nullable: false),
                    LevelId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Opinion = table.Column<int>(nullable: false),
                    bestTime = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelUserStats", x => x.LevelUserStatsId);
                    table.ForeignKey(
                        name: "FK_LevelUserStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    TokenId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    TokenString = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Levels_UserId",
                table: "Levels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelUserStats_UserId",
                table: "LevelUserStats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignTimes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "LevelUserStats");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
