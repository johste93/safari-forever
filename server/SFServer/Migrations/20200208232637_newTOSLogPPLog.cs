using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFServer.Migrations
{
    public partial class newTOSLogPPLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivacyPolicyAgreement",
                columns: table => new
                {
                    PrivacyPolicyAgreementId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Agreed = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicyAgreement", x => x.PrivacyPolicyAgreementId);
                });

            migrationBuilder.CreateTable(
                name: "TermsOfServiceAgreement",
                columns: table => new
                {
                    TermsOfServiceAgreementId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Agreed = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsOfServiceAgreement", x => x.TermsOfServiceAgreementId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivacyPolicyAgreement");

            migrationBuilder.DropTable(
                name: "TermsOfServiceAgreement");
        }
    }
}
