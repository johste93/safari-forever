using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class savebackup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSaveData",
                columns: table => new
                {
                    UserSaveDataId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    PepeUnlocked = table.Column<bool>(nullable: false),
                    PatchyUnlocked = table.Column<bool>(nullable: false),
                    JawsUnlocked = table.Column<bool>(nullable: false),
                    OlipherUnlocked = table.Column<bool>(nullable: false),
                    KokoUnlocked = table.Column<bool>(nullable: false),
                    LeonUnlocked = table.Column<bool>(nullable: false),
                    DebraUnlocked = table.Column<bool>(nullable: false),
                    NuggetUnlocked = table.Column<bool>(nullable: false),
                    PerryUnlocked = table.Column<bool>(nullable: false),
                    RexUnlocked = table.Column<bool>(nullable: false),
                    PingoUnlocked = table.Column<bool>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSaveData", x => x.UserSaveDataId);
                    table.ForeignKey(
                        name: "FK_UserSaveData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCampaignLevelData",
                columns: table => new
                {
                    UserCampaignLevelDataId = table.Column<string>(nullable: false),
                    UserSaveDataId = table.Column<string>(nullable: true),
                    World = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Beaten = table.Column<bool>(nullable: false),
                    PersonalHighscore = table.Column<double>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCampaignLevelData", x => x.UserCampaignLevelDataId);
                    table.ForeignKey(
                        name: "FK_UserCampaignLevelData_UserSaveData_UserSaveDataId",
                        column: x => x.UserSaveDataId,
                        principalTable: "UserSaveData",
                        principalColumn: "UserSaveDataId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCampaignLevelData_UserSaveDataId",
                table: "UserCampaignLevelData",
                column: "UserSaveDataId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveData_UserId",
                table: "UserSaveData",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCampaignLevelData");

            migrationBuilder.DropTable(
                name: "UserSaveData");
        }
    }
}
