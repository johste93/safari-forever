using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class AddedHats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BeanieHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BootHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ComradeHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConicalHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CrownHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HaloHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HornsHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MustacheHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PirateHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrivateHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SantaHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShadesHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowbizHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SombreroHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SouWesterHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThinfoilHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TopHatHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VikingHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WitchHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WizzardHatUnlocked",
                table: "UserSaveData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HatUnlocked",
                table: "Transactions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeanieHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "BootHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "ComradeHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "ConicalHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "CrownHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "HaloHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "HornsHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "MustacheHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "PirateHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "PrivateHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "SantaHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "ShadesHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "ShowbizHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "SombreroHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "SouWesterHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "ThinfoilHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "TopHatHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "VikingHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "WitchHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "WizzardHatUnlocked",
                table: "UserSaveData");

            migrationBuilder.DropColumn(
                name: "HatUnlocked",
                table: "Transactions");
        }
    }
}
